using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using AutoMapper;
using Openbrew.Web.Core.Configuration;
using ctorx.Core.Collections;
using ctorx.Core.Conversion;
using ctorx.Core.Data;
using ctorx.Core.Formatting;
using ctorx.Core.Messaging;
using ctorx.Core.Serialization;
using ctorx.Core.Web;
using Openbrew.Web.Core.Data;
using Openbrew.Web.Core.Service;
using Openbrew.Web.Models;
using Openbrew.Web.Core.Model;

namespace Openbrew.Web.Controllers
{
	public class RecipeController : BrewgrController
	{
		static readonly object BaCatalogLock = new object();
		static IList<object> Ba2026CatalogCache;
		static string Ba2026GuidelineHtml;
		static IList<BaStyleChartRange> Ba2026StyleComparisonCache;
		const string BeerXmlImportQueueKey = "BeerXmlImportQueue";
		readonly IUnitOfWorkFactory<BrewgrContext> UnitOfWorkFactory;
		readonly IRecipeService RecipeService;
		readonly IBeerStyleService BeerStyleService;
        readonly IStaticContentService StaticContentService;
        readonly IUserService UserService;
		readonly INotificationService NotificationService;
		readonly IPartnerIdResolver PartnerIdResolver;
		readonly IBeerXmlRecipeExporter BeerXmlExporter;
		readonly IBeerXmlRecipeImporter BeerXmlImporter;
		readonly IPartnerService PartnerService;
		readonly ISendToShopService SendToShopService;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public RecipeController(IUnitOfWorkFactory<BrewgrContext> unitOfWorkFactory, IRecipeService recipeService, IBeerStyleService beerStyleService, 
			IStaticContentService staticContentService, IUserService userService, INotificationService notificationService, IPartnerIdResolver partnerIdResolver,
			IBeerXmlRecipeExporter beerXmlExporter, IBeerXmlRecipeImporter beerXmlImporter, IPartnerService partnerService, ISendToShopService sendToShopService)
		{
			this.UnitOfWorkFactory = unitOfWorkFactory;
			this.RecipeService = recipeService;
			this.BeerStyleService = beerStyleService;
			this.StaticContentService = staticContentService;
            this.UserService = userService;
			this.NotificationService = notificationService;
			this.PartnerIdResolver = partnerIdResolver;
			this.BeerXmlExporter = beerXmlExporter;
			this.BeerXmlImporter = beerXmlImporter;
			this.PartnerService = partnerService;
			this.SendToShopService = sendToShopService;
		}

		#region BROWSE RECIPES / STYLE DETAIL

		/// <summary>
		/// Executes the View for Recipes
		/// </summary>
		[ActionName("homebrew-recipes")]
		public ViewResult Recipes()
		{
			var styles = this.BeerStyleService.GetStyleSummaries();
			var uncategorizedCount = this.BeerStyleService.GetUnCategorizedRecipeCount();

			var model = Mapper.Map(styles, new BrowseRecipesViewModel());
			model.UnCategorizedRecipeCount = uncategorizedCount;

			return View("Recipes", model);
		}

		[ActionName("other-homebrew-recipes")]
		public ActionResult UnCategorizedRecipes(int? page)
		{
			var pager = new Pager { CurrentPage = page ?? 1, ItemsPerPage = this.WebSettings.DefaultRecipesPerPage };

			var recipes = this.BeerStyleService.GetUnCategorizedRecipesPage(pager);

			if (recipes.Any() && !pager.IsInRange())
			{
				return this.Issue404();
			}

			return View("UnCategorized", new UnCategorizedRecipesViewModel { Recipes = recipes, Pager = pager, 
				BaseUrl = Url.Action("other-homebrew-recipes", "Recipe", new { page = (int?)null }, "http")});
		}

		/// <summary>
		/// Executes the View for StyleDetail
		/// </summary>
		public ActionResult StyleDetail(string urlFriendlyName, int? page)
		{
			// 301 for old page 1 URL from previous button
			if(page != null && page.Value == 1)
			{
				return this.RedirectPermanent(Request.Url.ToString().Replace("/1", ""));
			}

			var style = this.BeerStyleService.GetStyleByUrlFriendlyName(urlFriendlyName.ToLower().Replace("-recipes", ""));

			if(style == null)
			{
				return this.Issue404();
			}

			var pager = new Pager { CurrentPage = page ?? 1, ItemsPerPage = this.WebSettings.DefaultRecipesPerPage };
			
			var styleRecipes = this.BeerStyleService.GetStyleRecipesPage(style.SubCategoryId, pager);

			if(styleRecipes.Any() && !pager.IsInRange())
			{
				return this.Issue404();
			}

			var topRatedRecipes = this.BeerStyleService.GetTopRatedRecipes(style.SubCategoryId, 5);

			var model = new StyleDetailViewModel
			{
				BjcpStyle = style, 
				Recipes = styleRecipes, 
				Pager = pager, 
				BaseUrl = Url.StyleDetailUrl(urlFriendlyName),
				TopRatedRecipes = topRatedRecipes
			};

			return View(model);
		}

		#endregion
		
		#region SAVE RECIPE 

		[HttpPost]
		[Authorize]
		[ForceHttps]
		public ActionResult SaveRecipe(PostedRecipeViewModel postedRecipeViewModel)
		{
			// NOTE: This action handles saving for both new recipes
			// and edited recipes.  New Recipes post to this action while
			// edits post to this action via Ajax.

			var isNewRecipe = false;

			// Hydrate JSON ReceipeViewModel
			var recipeViewModel = postedRecipeViewModel.HydrateRecipeJson();
			isNewRecipe = recipeViewModel.IsNewRecipe();

			// Validation (client validates also ... this to ensure data consistency)
			var validator = new RecipeViewModelValidator();
			var validationResult = validator.Validate(recipeViewModel);
			if(!validationResult.IsValid)
			{
				if(isNewRecipe)
				{
					this.AppendMessage(new ErrorMessage { Text = "Did you leave something blank?  Please check your entries and try again."});
					ViewBag.RecipeCreationOptions = this.RecipeService.GetRecipeCreationOptions();
					return this.View("NewRecipe", recipeViewModel);
				}

				// Signals Invalid
				return this.Content("0");
			}

			using (var unitOfWork = this.UnitOfWorkFactory.NewUnitOfWork())
			{				
				try
				{
					var recipe = recipeViewModel.IsNewRecipe() ? new Recipe() : this.RecipeService.GetRecipeById(recipeViewModel.RecipeId);

					// Issue 404 if recipe does not exists or not owned by user
					if (!isNewRecipe && !recipe.WasCreatedBy(this.ActiveUser.UserId))
					{
						return this.Issue404();
					}

					// Map Recipe
					Mapper.Map(recipeViewModel, recipe);

					#region INGREDIENT DELETIONS

					if(!isNewRecipe)
					{
						// Delete Fermentables that were removed
						var fermentablesForDeletion = recipe.Fermentables.Except(recipe.Fermentables.Join(recipeViewModel.Fermentables ?? new List<RecipeFermentableViewModel>(),
							x => x.RecipeFermentableId, y => Converter.Convert<int>(y.Id), (x, y) => x)).ToList();
						this.RecipeService.MarkRecipeIngredientsForDeletion<RecipeFermentable>(fermentablesForDeletion);

						// Delete Hops that were removed
						var hopsForDeletion = recipe.Hops.Except(recipe.Hops.Join(recipeViewModel.Hops ?? new List<RecipeHopViewModel>(),
							x => x.RecipeHopId, y => Converter.Convert<int>(y.Id), (x, y) => x)).ToList();
						this.RecipeService.MarkRecipeIngredientsForDeletion<RecipeHop>(hopsForDeletion);

						// Delete Yeasts that were removed
						var yeastsForDeletion = recipe.Yeasts.Except(recipe.Yeasts.Join(recipeViewModel.Yeasts ?? new List<RecipeYeastViewModel>(),
							x => x.RecipeYeastId, y => Converter.Convert<int>(y.Id), (x, y) => x)).ToList();
						this.RecipeService.MarkRecipeIngredientsForDeletion<RecipeYeast>(yeastsForDeletion);

						// Delete Adjuncts that were removed
						var adjunctsForDeletion = recipe.Adjuncts.Except(recipe.Adjuncts.Join(recipeViewModel.Others ?? new List<RecipeOtherViewModel>(),
							x => x.RecipeAdjunctId, y => Converter.Convert<int>(y.Id), (x, y) => x)).ToList();
						this.RecipeService.MarkRecipeIngredientsForDeletion<RecipeAdjunct>(adjunctsForDeletion);

                        // Delete MashSteps that were removed
                        var mashStepsForDeletion = recipe.MashSteps.Except(recipe.MashSteps.Join(recipeViewModel.MashSteps ?? new List<RecipeMashStepViewModel>(),
                            x => x.RecipeMashStepId, y => Converter.Convert<int>(y.Id), (x, y) => x)).ToList();
                        this.RecipeService.MarkRecipeIngredientsForDeletion<RecipeMashStep>(mashStepsForDeletion);
					}

					#endregion

					#region INGREDIENT ADDITIONS

					// Add Fermentables
					if (recipeViewModel.Fermentables != null)
					{
						recipeViewModel.Fermentables.Where(x => Converter.Convert<int>(x.Id) == 0)
							.ForEach(x => recipe.Fermentables.Add(Mapper.Map(x, new RecipeFermentable())));
					}

					// Add Hops
					if (recipeViewModel.Hops != null)
					{
						recipeViewModel.Hops.Where(x => Converter.Convert<int>(x.Id) == 0)
							.ForEach(x => recipe.Hops.Add(Mapper.Map(x, new RecipeHop())));
					}

					// Add Yeasts
					if (recipeViewModel.Yeasts != null)
					{
						recipeViewModel.Yeasts.Where(x => Converter.Convert<int>(x.Id) == 0)
							.ForEach(x => recipe.Yeasts.Add(Mapper.Map(x, new RecipeYeast())));
					}

					// Add Adjuncts
					if (recipeViewModel.Others != null)
					{
						recipeViewModel.Others.Where(x => Converter.Convert<int>(x.Id) == 0)
							.ForEach(x => recipe.Adjuncts.Add(Mapper.Map(x, new RecipeAdjunct())));
					}

                    // Add MashStep
                    if (recipeViewModel.MashSteps != null)
                    {
                        recipeViewModel.MashSteps.Where(x => Converter.Convert<int>(x.Id) == 0)
                            .ForEach(x => recipe.MashSteps.Add(Mapper.Map(x, new RecipeMashStep())));
                    }

					#endregion

					#region INGREDIENT UPDATES

					if (!isNewRecipe)
					{
						// Update Fermentables
						if (recipeViewModel.Fermentables != null)
						{
							foreach (var recipeFermentableViewModel in recipeViewModel.Fermentables.Where(x => Converter.Convert<int>(x.Id) > 0))
							{
								var match = recipe.Fermentables.FirstOrDefault(x => x.RecipeFermentableId == Converter.Convert<int>(recipeFermentableViewModel.Id));
								if (match == null)
								{
									throw new InvalidOperationException("Unable to find matching fermentable");
								}

								Mapper.Map(recipeFermentableViewModel, match);
							}
						}

						// Update Hops
						if (recipeViewModel.Hops != null)
						{
							foreach (var recipeHopViewModel in recipeViewModel.Hops.Where(x => Converter.Convert<int>(x.Id) > 0))
							{
								var match = recipe.Hops.FirstOrDefault(x => x.RecipeHopId == Converter.Convert<int>(recipeHopViewModel.Id));
								if (match == null)
								{
									throw new InvalidOperationException("Unable to find matching Hop");
								}

								Mapper.Map(recipeHopViewModel, match);
							}
						}

						// Update Yeasts
						if (recipeViewModel.Yeasts != null)
						{
							foreach (var recipeYeastViewModel in recipeViewModel.Yeasts.Where(x => Converter.Convert<int>(x.Id) > 0))
							{
								var match = recipe.Yeasts.FirstOrDefault(x => x.RecipeYeastId == Converter.Convert<int>(recipeYeastViewModel.Id));
								if (match == null)
								{
									throw new InvalidOperationException("Unable to find matching Yeast");
								}

								Mapper.Map(recipeYeastViewModel, match);
							}
						}

						// Update Adjuncts
						if (recipeViewModel.Others != null)
						{
							foreach (var recipeAdjunctViewModel in recipeViewModel.Others.Where(x => Converter.Convert<int>(x.Id) > 0))
							{
								var match = recipe.Adjuncts.FirstOrDefault(x => x.RecipeAdjunctId == Converter.Convert<int>(recipeAdjunctViewModel.Id));
								if (match == null)
								{
									throw new InvalidOperationException("Unable to find matching Adjunct");
								}

								Mapper.Map(recipeAdjunctViewModel, match);
							}
						}

                        // Update MashSteps
                        if (recipeViewModel.MashSteps != null)
                        {
                            foreach (var recipeMashStepViewModel in recipeViewModel.MashSteps.Where(x => Converter.Convert<int>(x.Id) > 0))
                            {
                                var match = recipe.MashSteps.FirstOrDefault(x => x.RecipeMashStepId == Converter.Convert<int>(recipeMashStepViewModel.Id));
                                if (match == null)
                                {
                                    throw new InvalidOperationException("Unable to find matching MashStep");
                                }

                                Mapper.Map(recipeMashStepViewModel, match);
                            }
                        }
					}

					#endregion

					#region STEP DELETIONS / ADDITIONS / UPDATES

					// Deletions
					if (!isNewRecipe)
					{
						var stepsForDeletion = recipe.Steps.Except(recipe.Steps.Join(recipeViewModel.Steps ?? new List<RecipeStepViewModel>(),
							x => x.RecipeStepId, y => Converter.Convert<int>(y.Id), (x, y) => x)).ToList();
						this.RecipeService.MarkRecipeStepsForDeletion(stepsForDeletion);
					}

					// Additions
					if (recipeViewModel.Steps != null)
					{
						if(recipe.Steps == null)
						{
							recipe.Steps = new List<RecipeStep>();
						}

						recipeViewModel.GetSteps()
							.Where(x => Converter.Convert<int>(x.Id) == 0)
							.Where(x => !string.IsNullOrWhiteSpace(x.Text))
							.ForEach(x => recipe.Steps.Add(Mapper.Map(x, new RecipeStep { DateCreated = DateTime.Now })));
					}

					// Updates
					if (!isNewRecipe)
					{
						if (recipeViewModel.Steps != null)
						{
							foreach (var recipeStep in recipeViewModel.GetSteps()
								.Where(x => Converter.Convert<int>(x.Id) > 0)
								.Where(x => !string.IsNullOrWhiteSpace(x.Text)))
							{
								var match = recipe.Steps.FirstOrDefault(x => x.RecipeStepId == Converter.Convert<int>(recipeStep.Id));
								if (match == null)
								{
									throw new InvalidOperationException("Unable to find matching step");
								}

								match = Mapper.Map(recipeStep, match);
								match.DateModified = DateTime.Now;
							}
						}
					}

					#endregion

					// Save the Image
					if(isNewRecipe)
					{
						if(recipeViewModel.PhotoForUpload != null)
						{
							// Save the New Image
							recipe.ImageUrlRoot = this.StaticContentService.SaveRecipeImage(recipeViewModel.PhotoForUpload.InputStream,
								this.WebSettings.MediaPhysicalRoot);
						}
					}

					// New, complete recipes publish by default. An explicit private choice
					// remains owner-only; incomplete recipes are always saved as drafts.
					var publishRequested = !string.Equals(recipeViewModel.Visibility, "private", StringComparison.OrdinalIgnoreCase);
					this.RecipeService.FinalizeRecipe(recipe, publishRequested);

					unitOfWork.Commit();

					if(isNewRecipe)
					{
						var importQueue = this.Session[BeerXmlImportQueueKey] as IList<Recipe>;
						if (importQueue != null && importQueue.Any())
						{
							this.ForwardMessage(new SuccessMessage { Text = "Recipe saved. " + importQueue.Count + " selected BeerXML recipe" + (importQueue.Count == 1 ? " remains" : "s remain") + " to review." });
							return RedirectToAction("ContinueBeerXmlImport");
						}

						this.ForwardMessage(new SuccessMessage { Text = BrewgrMessages.RecipeSaved });
						return Redirect(Url.RecipeEditUrl(recipe.RecipeId));
					}
					else
					{
						// Signals Success
						return Content("1");
					}
				}
				catch (Exception ex)
				{
					this.LogHandledException(ex);
					unitOfWork.Rollback();

					if(isNewRecipe)
					{
						ViewBag.RecipeCreationOptions = this.RecipeService.GetRecipeCreationOptions();
						this.AppendMessage(new ErrorMessage { Text = GenericMessages.ErrorMessage });
						return View("NewRecipe", recipeViewModel);
					}
					else
					{
						// Signals Failure
						return Content("-1");
					}
				}
			}
		}

		#endregion

		#region NEW RECIPE

		/// <summary>
		/// Executes the View for RecipeClone
		/// </summary>
		[ForceHttps]
		public ActionResult RecipeClone(int recipeId)
		{
			var recipe = this.RecipeService.GetRecipeById(recipeId);

			if (recipe == null)
			{
				return this.Issue404();
			}

			ViewBag.RecipeCreationOptions = this.RecipeService.GetRecipeCreationOptions();

			var cloned = Mapper.Map(recipe, new RecipeViewModel());
			cloned.RecipeId = 0;
			cloned.OriginalRecipeId = recipe.RecipeId;
			cloned.Name = "Clone Of " + recipe.RecipeName;
			cloned.Description = null;

			// Reset Recipe Ingredent Ids to 0 
			cloned.Fermentables.ForEach(x => x.Id = "0");
			cloned.Hops.ForEach(x => x.Id = "0");
			cloned.Yeasts.ForEach(x => x.Id = "0");
			cloned.Others.ForEach(x => x.Id = "0");
			cloned.Steps.ForEach(x => x.Id = "0");

			// Add Messaging
			this.AppendMessage(new InfoMessage { Text = "You are cloning \"" + recipe.RecipeName + "\".  Once you have made your changes, click \"Save Recipe\"" });

			return View("NewRecipe", cloned);
		}

		/// <summary>
		/// Executes the View for RecipeBuilder301
		/// </summary>
		[ActionName("homebrew-recipe-builder")]
		public ActionResult RecipeBuilder301()
		{
			return RedirectPermanent(Url.Action("homebrew-recipe-calculator"));
		}

		/// <summary>
		/// Executes the View for NewRecipe
		/// </summary>
		[ActionName("homebrew-recipe-calculator")]
		[ForceHttps]
		public ViewResult NewRecipe()
		{
			ViewBag.RecipeCreationOptions = this.RecipeService.GetRecipeCreationOptions();

			// Source Recipe (or Default) // TODO: Derive Defaults from user preferences
			var recipe = new RecipeViewModel();
			recipe.UnitType = "s";
			recipe.BatchSize = 5;
			recipe.BoilSize = 6.5;
			recipe.BoilTime = 60;
			recipe.Efficiency = .75;
			recipe.IbuFormula = "t";
			recipe.Visibility = "public";

			return View("NewRecipe", recipe);
		}

		#endregion

		#region NEW ING ROWS

		[ActionName("buildertemplates-v2")]
		[ForceHttps]
		public ViewResult BuilderTemplates()
		{
			return View("_BuilderTemplates");			
		}

		[HttpGet]
		[ActionName("ba-2026-style-catalog")]
		public JsonResult Ba2026StyleCatalog()
		{
			lock (BaCatalogLock)
			{
				if (Ba2026CatalogCache == null)
				{
					var html = new WebClient().DownloadString("https://www.brewersassociation.org/wp-json/wp/v2/pages/15930");
					html = html.Replace("\\/", "/");
					Ba2026CatalogCache = Regex.Matches(html, "href=['\\\"]#(?<id>\\d+)['\\\"][^>]*>(?<name>.*?)</a>", RegexOptions.IgnoreCase | RegexOptions.Singleline)
						.Cast<Match>().Select(x => new { Id = x.Groups["id"].Value, Name = Regex.Replace(WebUtility.HtmlDecode(x.Groups["name"].Value), "<.*?>", "").Trim() })
						.Where(x => !string.IsNullOrWhiteSpace(x.Name)).Distinct().Cast<object>().ToList();
				}
			}
			return Json(Ba2026CatalogCache, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[ActionName("ba-2026-style-counts")]
		public JsonResult Ba2026StyleCounts()
		{
			var counts = this.RecipeService.GetAllRecipes()
				.Where(x => x.StyleCatalog == "ba-2026" && !string.IsNullOrWhiteSpace(x.CatalogStyleCode))
				.GroupBy(x => x.CatalogStyleCode)
				.ToDictionary(x => x.Key, x => x.Count());
			return Json(counts, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[ActionName("ba-2026-style-range")]
		public JsonResult Ba2026StyleRange(string styleId)
		{
			if (string.IsNullOrWhiteSpace(styleId) || !Regex.IsMatch(styleId, "^\\d+$")) return Json(null, JsonRequestBehavior.AllowGet);
			var guideline = GetBa2026Guideline(styleId);
			return Json(ToBaStyleChartRange(guideline), JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[ActionName("ba-2026-style-comparisons")]
		public JsonResult Ba2026StyleComparisons()
		{
			lock (BaCatalogLock)
			{
				if (Ba2026StyleComparisonCache == null)
				{
					if (Ba2026GuidelineHtml == null) GetBa2026Guideline("1");
					var ids = Regex.Matches(Ba2026GuidelineHtml ?? "", "<ul[^>]*id=[\\\"'](?<id>\\d+)[\\\"']", RegexOptions.IgnoreCase)
						.Cast<Match>().Select(x => x.Groups["id"].Value).Distinct().ToList();
					Ba2026StyleComparisonCache = ids.Select(GetBa2026Guideline).Where(x => x != null).Select(ToBaStyleChartRange).ToList();
				}
				return Json(Ba2026StyleComparisonCache, JsonRequestBehavior.AllowGet);
			}
		}

		[ActionName("Ba2026StyleDetail")]
		public ActionResult Ba2026StyleDetail(string styleId, string slug)
		{
			if (string.IsNullOrWhiteSpace(styleId) || !Regex.IsMatch(styleId, "^\\d+$")) return this.Issue404();
			var detail = GetBa2026Guideline(styleId);
			if (detail == null) return this.Issue404();

			detail.Recipes = this.RecipeService.GetAllRecipes()
				.Where(x => x.StyleCatalog == "ba-2026" && x.CatalogStyleCode == styleId)
				.Take(24).ToList();
			return View("Ba2026StyleDetail", detail);
		}

		Ba2026StyleDetailViewModel GetBa2026Guideline(string styleId)
		{
			lock (BaCatalogLock)
			{
				if (Ba2026GuidelineHtml == null)
				{
					var raw = new WebClient().DownloadString("https://www.brewersassociation.org/wp-json/wp/v2/pages/15930");
					var page = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(raw);
					var content = (Dictionary<string, object>)page["content"];
					Ba2026GuidelineHtml = (string)content["rendered"];
				}
			}
			var match = Regex.Match(Ba2026GuidelineHtml, "<ul[^>]*id=[\\\"']" + Regex.Escape(styleId) + "[\\\"'][^>]*>(?<body>.*?)(?:<!--End Style-->|<ul[^>]*id=)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
			if (!match.Success) return null;
			var items = Regex.Matches(match.Groups["body"].Value, "<li[^>]*>(?<item>.*?)</li>", RegexOptions.Singleline)
				.Cast<Match>().Select(x => Regex.Replace(WebUtility.HtmlDecode(x.Groups["item"].Value), "<.*?>", " ").Replace("&nbsp;", " ").Trim()).Where(x => x.Length > 0).ToList();
			Func<string, string> field = label => items.FirstOrDefault(x => x.StartsWith(label, StringComparison.OrdinalIgnoreCase)) ?? "Varies";
			Func<string, string> value = label => Regex.Replace(field(label), "^" + Regex.Escape(label) + "\\s*:?[\\s]*", "", RegexOptions.IgnoreCase).Trim();
			var name = items.FirstOrDefault() ?? "Beer style";
			Func<string, string> cue = label => {
				var text = value(label);
				if (text == "Varies") return null;
				text = Regex.Replace(text, "\\s+", " ").Trim();
				return text.Length > 180 ? text.Substring(0, 177).TrimEnd() + "…" : text;
			};
			Func<string, string, string> intensityCue = (label, subject) => {
				var rawCue = cue(label);
				if (string.IsNullOrWhiteSpace(rawCue)) return null;
				var intensity = Regex.Match(rawCue.ToLowerInvariant(), "(none|very low|low|medium-low|medium-high|medium|high|very high|intense)(?:\\s+to\\s+(none|very low|low|medium-low|medium-high|medium|high|very high|intense))?");
				return intensity.Success ? "Keep the " + subject + " expression " + intensity.Value + "." : "Let the " + subject + " expression support, rather than dominate, the balance.";
			};
			Func<string, string> fermentationCue = rawCue => {
				if (string.IsNullOrWhiteSpace(rawCue)) return null;
				var lower = rawCue.ToLowerInvariant();
				var cues = new List<string>();
				if (lower.Contains("ester")) cues.Add(lower.Contains("low") ? "Keep fruitiness restrained." : "A light fruity fermentation note can fit.");
				if (lower.Contains("phenol")) cues.Add("Let spice-like yeast character suit the style.");
				if (lower.Contains("diacetyl")) cues.Add(lower.Contains("absent") ? "Finish clean, without buttery notes." : "A subtle buttery note may be appropriate.");
				if (lower.Contains("sour") || lower.Contains("acid")) cues.Add("Acidity should feel deliberate and integrated.");
				return cues.Any() ? string.Join(" ", cues) : null;
			};
			Func<string, string> description = styleName => {
				var n = styleName.ToLowerInvariant();
				if (n.Contains("ipa") || n.Contains("india pale")) return styleName + " is a hop-led ale, built around a bright bitterness and expressive hop character.";
				if (n.Contains("stout")) return styleName + " is a dark, roast-accented ale with a fuller flavor profile than its color alone suggests.";
				if (n.Contains("porter")) return styleName + " is a dark ale that leans into smooth toasted and cocoa-like malt character.";
				if (n.Contains("sour") || n.Contains("gose") || n.Contains("lambic") || n.Contains("gueuze") || n.Contains("weisse")) return styleName + " is a tart, refreshing beer where acidity is part of the intended balance.";
				if (n.Contains("wheat") || n.Contains("weizen") || n.Contains("wit")) return styleName + " is a wheat-forward beer with a soft texture and an easy-drinking profile.";
				if (n.Contains("lager") || n.Contains("pils") || n.Contains("helles") || n.Contains("bock") || n.Contains("märzen") || n.Contains("maerzen")) return styleName + " is a clean-fermented lager that puts balance, drinkability, and malt-and-hop precision first.";
				if (n.Contains("belgian") || n.Contains("saison") || n.Contains("tripel") || n.Contains("dubbel") || n.Contains("quadrupel")) return styleName + " is a characterful Belgian-inspired ale, shaped as much by fermentation as by malt and hops.";
				if (n.Contains("fruit")) return styleName + " is a fruit-accented beer where the added fruit should feel integrated with the base style.";
				return styleName + " is a distinct beer style with its own balance of malt, hops, fermentation character, and drinkability.";
			};
			var sourceNotes = string.Join(" ", new[] { cue("Perceived Malt Aroma & Flavor"), cue("Perceived Hop Aroma & Flavor"), cue("Fermentation Characteristics") }.Where(x => !string.IsNullOrWhiteSpace(x))).ToLowerInvariant();
			var tastingNotes = new List<string>();
			var noteMap = new Dictionary<string, string> { { "citrus", "citrus" }, { "tropical", "tropical fruit" }, { "pine", "piney hops" }, { "floral", "floral hops" }, { "caramel", "caramel malt" }, { "toast", "toasted malt" }, { "chocolate", "cocoa" }, { "coffee", "coffee" }, { "roast", "roast" }, { "spice", "spice" }, { "banana", "banana" }, { "clove", "clove" }, { "stone fruit", "stone fruit" }, { "berry", "berries" }, { "honey", "honey" }, { "smoke", "smoke" } };
			foreach (var note in noteMap.Where(x => sourceNotes.Contains(x.Key)).Select(x => x.Value)) if (!tastingNotes.Contains(note)) tastingNotes.Add(note);
			var bitterness = value("Bitterness (IBU)");
			var color = value("Color SRM (EBC)");
			var originalGravity = value("Original Gravity");
			var finalGravity = value("Apparent Extract/Final Gravity");
			var alcohol = value("Alcohol by Weight (Volume)");
			var ibuRange = ExtractBaNumberRange(bitterness, @"\d+(?:\.\d+)?");
			var srmRange = ExtractBaNumberRange(color, @"\d+(?:\.\d+)?");
			var ogRange = ExtractBaNumberRange(originalGravity, @"1\.\d+");
			var fgRange = ExtractBaNumberRange(finalGravity, @"1\.\d+");
			var alcoholValues = ExtractBaNumberRange(alcohol, @"\d+(?:\.\d+)?(?=%)", true);
			var gaugeMetrics = new List<BaStyleGaugeMetric>();
			AddBaGaugeMetric(gaugeMetrics, "OG", ogRange, 1.020, 1.130, "", "0.000");
			AddBaGaugeMetric(gaugeMetrics, "FG", fgRange, 1.000, 1.040, "", "0.000");
			AddBaGaugeMetric(gaugeMetrics, "IBU", ibuRange, 0, 120, "IBU", "0.#");
			AddBaGaugeMetric(gaugeMetrics, "SRM", srmRange, 0, 40, "SRM", "0.#");
			AddBaGaugeMetric(gaugeMetrics, "ABV", alcoholValues, 0, 16, "%", "0.#");
			if (ibuRange != null && ogRange != null && ogRange[0] > 1 && ogRange[1] > 1)
			{
				var buGuLow = ibuRange[0] / ((ogRange[1] - 1) * 1000);
				var buGuHigh = ibuRange[1] / ((ogRange[0] - 1) * 1000);
				AddBaGaugeMetric(gaugeMetrics, "BU:GU", new[] { buGuLow, buGuHigh }, 0, 1.5, "", "0.00");
			}
			return new Ba2026StyleDetailViewModel {
				Id = styleId, Name = name, Description = description(name), TastingNotes = tastingNotes, Bitterness = value("Bitterness (IBU)"), Color = value("Color SRM (EBC)"),
				OriginalGravity = originalGravity, FinalGravity = finalGravity, Alcohol = alcohol,
				VisualCue = cue("Color"), BalanceCue = cue("Perceived Bitterness"), BodyCue = cue("Body"),
				MaltCue = intensityCue("Perceived Malt Aroma & Flavor", "malt"), HopCue = intensityCue("Perceived Hop Aroma & Flavor", "hop"), FermentationCue = fermentationCue(cue("Fermentation Characteristics")),
				GaugeMetrics = gaugeMetrics, Recipes = new List<Recipe>() };
		}

		static double[] ExtractBaNumberRange(string value, string pattern, bool useLastPair = false)
		{
			if (string.IsNullOrWhiteSpace(value)) return null;
			var matches = Regex.Matches(value, pattern).Cast<Match>().Select(x => {
				double number;
				return double.TryParse(x.Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out number) ? (double?)number : null;
			}).Where(x => x.HasValue).Select(x => x.Value).ToList();
			if (matches.Count < 2) return null;
			var start = useLastPair && matches.Count >= 4 ? matches.Count - 2 : 0;
			return new[] { Math.Min(matches[start], matches[start + 1]), Math.Max(matches[start], matches[start + 1]) };
		}

		static void AddBaGaugeMetric(ICollection<BaStyleGaugeMetric> metrics, string label, double[] range, double minimum, double baselineMaximum, string unit, string format)
		{
			if (range == null)
			{
				metrics.Add(new BaStyleGaugeMetric { Label = label, Unit = unit, LowLabel = "Varies", HighLabel = "Varies", RangeLabel = "Varies", StartPercent = 0, WidthPercent = 100 });
				return;
			}
			var maximum = Math.Max(baselineMaximum, range[1] * 1.12);
			var start = Math.Max(0, Math.Min(100, (range[0] - minimum) / (maximum - minimum) * 100));
			var end = Math.Max(start + 2, Math.Min(100, (range[1] - minimum) / (maximum - minimum) * 100));
			var lowLabel = range[0].ToString(format, System.Globalization.CultureInfo.InvariantCulture);
			var highLabel = range[1].ToString(format, System.Globalization.CultureInfo.InvariantCulture);
			metrics.Add(new BaStyleGaugeMetric {
				Label = label, Unit = unit, LowLabel = lowLabel, HighLabel = highLabel,
				RangeLabel = unit == "%" ? lowLabel + "% – " + highLabel + "%" : lowLabel + " – " + highLabel + (string.IsNullOrEmpty(unit) ? "" : " " + unit),
				Low = range[0], High = range[1],
				StartPercent = start, WidthPercent = end - start
			});
		}

		static BaStyleChartRange ToBaStyleChartRange(Ba2026StyleDetailViewModel guideline)
		{
			if (guideline == null || guideline.GaugeMetrics == null) return null;
			Func<string, BaStyleGaugeMetric> metric = label => guideline.GaugeMetrics.FirstOrDefault(x => x.Label == label);
			Func<string, double> low = label => metric(label) != null ? metric(label).Low : 0;
			Func<string, double> high = label => metric(label) != null ? metric(label).High : 0;
			return new BaStyleChartRange {
				SubCategoryID = guideline.Id, SubCategoryName = guideline.Name,
				og_low = low("OG"), og_high = high("OG"), fg_low = low("FG"), fg_high = high("FG"),
				ibu_low = low("IBU"), ibu_high = high("IBU"), srm_low = low("SRM"), srm_high = high("SRM"),
				abv_low = low("ABV"), abv_high = high("ABV")
			};
		}

		#endregion

		#region EDIT RECIPE

		/// <summary>
		/// Executes the View for RecipeEdit
		/// </summary>
		[Authorize]
		[ForceHttps]
		public ActionResult RecipeEdit(int recipeId)
		{
			var recipe = this.RecipeService.GetRecipeById(recipeId);
			
			// Issue 404 if recipe does not exists or not owned by user
			if (recipe == null || !recipe.WasCreatedBy(this.ActiveUser.UserId))
			{
				return this.Issue404();
			}

			// Explain why an owner-only recipe is not visible to the public.
			if (!recipe.IsPublic)
			{
				var visibilityMessage = recipe.Fermentables.Any(x => x.Amount > 0) && recipe.Yeasts.Any()
					? "This recipe is private and only you can see it. Change Sharing to publish it."
					: "This recipe is a draft and only you can see it. Add fermentables and yeast before publishing.";
				this.AppendMessage(new WarnMessage { Text = visibilityMessage });
			}

			ViewBag.RecipeCreationOptions = this.RecipeService.GetRecipeCreationOptions();

			var recipeModel = Mapper.Map(recipe, new RecipeViewModel());


			// Fetch Tasting Notes
			var tastingNotes = this.RecipeService.GetRecipeTastingNotes(recipe.RecipeId);
			recipeModel.TastingNotes = tastingNotes;

			var commentWrapperViewModel = new CommentWrapperViewModel();
			commentWrapperViewModel.CommentViewModels = Mapper.Map(this.RecipeService.GetRecipeComments(recipeId), new List<CommentViewModel>());
			commentWrapperViewModel.GenericId = recipeId;
			commentWrapperViewModel.CommentType = CommentType.Recipe;
			recipeModel.CommentWrapperViewModel = commentWrapperViewModel;

			// Get the most recent brew session -- this should be added to the recipe in the service, really but hey
			recipeModel.MostRecentBrewSession = this.RecipeService.GetMostRecentBrewSession(recipeId);

			return View(recipeModel);
		}

		/// <summary>
		/// /Executes the view for ChangeRecipePhoto
		/// </summary>
		[Authorize]
		[ForceHttps]
		public ActionResult BuilderChangeRecipePhoto(int recipeId)
		{
			var recipe = this.RecipeService.GetRecipeById(recipeId);

			if(recipe.CreatedBy != this.ActiveUser.UserId)
			{
				return this.Issue404();
			}

			this.AppendMessage(new InfoMessage { Text = "<span>Your photo is being uploaded</span><img src=\"/img/h-loader.gif\" />" });

			ViewBag.UploadComplete = false;
			return View("~/Views/Recipe/BuilderChangeRecipePhoto.cshtml", Mapper.Map(recipe, new RecipeViewModel()));
		}

		/// <summary>
		/// /Executes the view for ChangeRecipePhoto
		/// </summary>
		[HttpPost]
		[ForceHttps]
		public ActionResult BuilderChangeRecipePhoto(ChangeRecipePhotoViewModel changeRecipePhotoViewModel)
		{
			var recipe = this.RecipeService.GetRecipeById(changeRecipePhotoViewModel.RecipeId);

			if (recipe.CreatedBy != this.ActiveUser.UserId)
			{
				return this.Issue404();
			}

			var recipeViewModel = Mapper.Map(recipe, new RecipeViewModel());
			var uploadSucceeded = false;

			using(var unitOfWork = this.UnitOfWorkFactory.NewUnitOfWork())
			{
				try
				{
					var oldImageUrlRoot = recipe.ImageUrlRoot;

					// Save the New Image
					recipe.ImageUrlRoot = this.StaticContentService.SaveRecipeImage(changeRecipePhotoViewModel.PhotoForUpload.InputStream,
						this.WebSettings.MediaPhysicalRoot);

					// Delete the Old Image (if it exists)
					if (!string.IsNullOrWhiteSpace(oldImageUrlRoot))
					{
						this.StaticContentService.DeleteRecipeImage(this.WebSettings.MediaPhysicalRoot, oldImageUrlRoot);
					}

					unitOfWork.Commit();
					recipeViewModel = Mapper.Map(recipe, new RecipeViewModel());
					uploadSucceeded = true;

					this.AppendMessage(new SuccessMessage { Text = "Your photo has been uploaded." });
				}
				catch(Exception ex)
				{
					this.LogHandledException(ex);					
					unitOfWork.Rollback();

					this.AppendMessage(new ErrorMessage { Text = "There was a problem saving your photo.  Please try again."});
				}
			}

			ViewBag.UploadComplete = uploadSucceeded;
			return View("~/Views/Recipe/BuilderChangeRecipePhoto.cshtml", recipeViewModel);
		}

		#endregion

		#region RECIPE DETAIL

		/// <summary>
		/// Executes the View for RecipeDetail
		/// </summary>
		public ActionResult RecipeDetail(int recipeId)
		{
			var recipe = this.RecipeService.GetRecipeById(recipeId);

			if(recipe == null)
			{
				return this.Issue404();
			}

			// A public-preview link must never render an owner-only recipe, even when
			// the owner happens to be signed in. This keeps shared/private URLs honest
			// and lets the friendly 404 explain the next steps without exposing it.
			if (!recipe.IsPublic && !string.IsNullOrWhiteSpace(Request["public"]))
			{
				return this.Issue404();
			}

			// Auto Redirect to EDIT page for owner
			if (this.ActiveUser != null && recipe.CreatedBy == this.ActiveUser.UserId && string.IsNullOrWhiteSpace(Request["public"]))
			{
				return this.Redirect(Url.RecipeEditUrl(recipeId));
			}

			// Notify the owner why their recipe is not public.
			if(!recipe.IsPublic)
			{
				var visibilityMessage = recipe.Fermentables.Any(x => x.Amount > 0) && recipe.Yeasts.Any()
					? "This recipe is private and only you can see it. Change Sharing to publish it."
					: "This recipe is a draft and only you can see it. Add fermentables and yeast before publishing.";
				this.AppendMessage(new WarnMessage { Text = visibilityMessage });
			}

			// Get Similar Recipes
			ViewBag.SimilarRecipes = this.RecipeService.GetSimilarRecipes(recipe, 4);

			var recipeViewModel = Mapper.Map(recipe, new RecipeViewModel());

			// Fetch Brew Session Count (this should really go in a service as part of recipe get)
			recipeViewModel.BrewSessionCount = this.RecipeService.GetRecipeBrewSessionsCount(recipeId);

			// Fetch most recent brew session
			if(recipeViewModel.BrewSessionCount > 0)
			{
				recipeViewModel.MostRecentBrewSession = this.RecipeService.GetMostRecentBrewSession(recipeId);
			}

			// Fetch Tasting Notes
			var tastingNotes = this.RecipeService.GetRecipeTastingNotes(recipe.RecipeId);

			// Get Additional Data
            var user = this.UserService.GetUserById(recipe.CreatedBy);
			
            if ((recipeViewModel.OriginalRecipeId ?? 0) != 0)
            { 
                var originalRecipe = this.RecipeService.GetRecipeById((recipeViewModel.OriginalRecipeId ?? 0));
	            if(originalRecipe != null)
	            {
		            recipeViewModel.OriginalRecipe = Mapper.Map(originalRecipe, new RecipeViewModel());
	            }
            }
            
            var recipeDetailViewModel = new RecipeDetailViewModel();
            recipeDetailViewModel.RecipeViewModel = recipeViewModel;
            recipeDetailViewModel.UserSummary = Mapper.Map(user, new UserSummary());
			recipeDetailViewModel.TastingNotes = tastingNotes;
			
            var commentWrapperViewModel = new CommentWrapperViewModel();
            commentWrapperViewModel.CommentViewModels = Mapper.Map(this.RecipeService.GetRecipeComments(recipeId), new List<CommentViewModel>());
            commentWrapperViewModel.GenericId = recipeId;
            commentWrapperViewModel.CommentType = CommentType.Recipe;
            recipeDetailViewModel.RecipeViewModel.CommentWrapperViewModel = commentWrapperViewModel;

			// TODO: Check if the name passed in the URL is different than what
			// TODO: is in the DB.  If it is....do a 301 Redirect.  This is for SEO.
            ViewData["DisableEditing"] = true;


			// Get Send To Shop Settings (if any)
			ViewBag.SendToShopSettings = this.SendToShopService.GetRecipeCreationSendToShopSettings(false);

            return View(recipeDetailViewModel);
		}

 
        [HttpPost]
        public ActionResult AddComment(CommentAddViewModel commentAddViewModel)
        {
            if (!commentAddViewModel.Validate().IsValid)
            {
                return this.Issue404();
            }

            // Normalize the Line Breaks
            commentAddViewModel.CommentText = commentAddViewModel.CommentText.Replace("\n", Environment.NewLine);

            switch (commentAddViewModel.CommentType)
            {
                case CommentType.Recipe:
                    RecipeComment recipeComment;

                    using (var unitOfWork = this.UnitOfWorkFactory.NewUnitOfWork())
                    {
                        recipeComment = new RecipeComment();
                        recipeComment.CommentText = commentAddViewModel.CommentText;
                        recipeComment.RecipeId = commentAddViewModel.GenericId;
                        this.RecipeService.AddRecipeComment(recipeComment);
                        unitOfWork.Commit();
                    }

                    // Queue Comment Notification
                    this.NotificationService.QueueNotification(NotificationType.RecipeComment, recipeComment);
                    break;
                case CommentType.Session:
                    BrewSessionComment brewSessionComment;

                    using (var unitOfWork = this.UnitOfWorkFactory.NewUnitOfWork())
                    {
                        brewSessionComment = new BrewSessionComment();
                        brewSessionComment.CommentText = commentAddViewModel.CommentText;
                        brewSessionComment.BrewSessionId = commentAddViewModel.GenericId;
                        this.RecipeService.AddBrewSessionComment(brewSessionComment);
                        unitOfWork.Commit();
                    }

                    // Queue Comment Notification
                    this.NotificationService.QueueNotification(NotificationType.BrewSessionComment, brewSessionComment);
                    break;
                default:
                    return this.Issue404();
            }
            
            var commentViewModel = new CommentViewModel();
            commentViewModel.CommentText = commentAddViewModel.CommentText;
            commentViewModel.UserId = this.ActiveUser.UserId;
            commentViewModel.UserName = this.ActiveUser.Username;
            commentViewModel.UserAvatarUrl = UserAvatar.GetAvatar(59, this.ActiveUser.EmailAddress);
            commentViewModel.DateCreated = DateTime.Now;

            return View("_Comment", commentViewModel);
        }



		#endregion

		#region DELETE RECIPE 

		/// <summary>
        /// Executes the View for RecipeDelete
        /// </summary>
        [ForceHttps]
        public ActionResult RecipeDelete(int recipeId)
        {
            using (var unitOfWork = this.UnitOfWorkFactory.NewUnitOfWork())
            {
                try
                {
                    var recipe = this.RecipeService.GetRecipeById(recipeId);

                    // Issue 404 if recipe does not exists or not owned by user
                    if (recipe == null || !recipe.WasCreatedBy(this.ActiveUser.UserId))
                    {
                        return this.Issue404();
                    }

					// Delete the Recipe
					this.RecipeService.DeleteRecipe(recipe);

                    unitOfWork.Commit();

                    if (!string.IsNullOrWhiteSpace(recipe.ImageUrlRoot))
                    {
                        this.StaticContentService.DeleteRecipeImage(this.WebSettings.MediaPhysicalRoot, recipe.ImageUrlRoot);
                    }

                    this.ForwardMessage(new SuccessMessage { Text = BrewgrMessages.RecipeDeleted });

                    if (Request.UrlReferrer == null)
                    {
                        return Redirect(Url.Action("my-recipes", "recipe", null, "http"));
                    }

                    if (Request.UrlReferrer.ToString() == Url.RecipeEditUrl(recipe.RecipeId))
                    {
                        // if they are deleting from the recipe edit page then redirect to the my recipes page
                        return Redirect("/dashboard#recipes");
                    }

                    if (Request.UrlReferrer.ToString().ToLower().Contains("/dashboard"))
                    {
                        return Redirect(Request.UrlReferrer + "#recipes");
                    }
                    else
                    {
                        return Redirect(Request.UrlReferrer.ToString());
                    }
                }
                catch (Exception ex)
                {
                    this.LogHandledException(ex);

                    this.AppendMessage(new ErrorMessage { Text = GenericMessages.ErrorMessage });

                    unitOfWork.Rollback();

                    return this.Issue404();
                }
            }
		}

		#endregion

		#region PRINT RECIPE 

		[Route("recipe/{recipeid:int}/print")]
		public ActionResult RecipePrint(int recipeId)
		{
			var recipe = this.RecipeService.GetRecipeById(recipeId);

            if (recipe == null)
                return this.Issue404();

            var recipeViewModel = Mapper.Map(recipe, new RecipeViewModel());
			recipeViewModel.CommentWrapperViewModel = new CommentWrapperViewModel { CommentViewModels = new List<CommentViewModel>() };

			// Get Additional Data
			var user = this.UserService.GetUserById(recipe.CreatedBy);

			var recipeDetailViewModel = new RecipeDetailViewModel
			{
				RecipeViewModel = recipeViewModel,
				UserSummary = Mapper.Map(user, new UserSummary()),
				TastingNotes = new List<TastingNote>()
			};

			return this.View(recipeDetailViewModel);
		}

		#endregion

		#region RECIPE BREW SESSIONS

		/// <summary>
		/// Executes the View for RecipeBrewSessions
		/// </summary>
		public ActionResult RecipeBrewSessions(int recipeId)
		{
			var recipeSummary = this.RecipeService.GetRecipeSummaryById(recipeId);

			if (recipeSummary == null)
			{
				return this.Issue404();
			}

			var brewSessions = this.RecipeService.GetRecipeBrewSessions(recipeId)
				.OrderByDescending(x => x.BrewDate)
				.ToList();

			return View(new RecipeBrewSessionsViewModel
			{
				RecipeSummary = Mapper.Map(recipeSummary, new RecipeSummaryViewModel()),
				BrewSessions = brewSessions
			});
		}

		#endregion

		#region EXPORT

		/// <summary>
		/// Executes the View for Export
		/// </summary>
		public ActionResult BeerXml(int recipeId)
		{
			var recipe = this.RecipeService.GetRecipeById(recipeId);

			if (recipe == null)
			{
				return this.Issue404();
			}

			var xmlString = this.BeerXmlExporter.Export(recipe);
			var xmlBytes = Encoding.Default.GetBytes(xmlString);

			var disposition = new ContentDisposition
			{
				// for example foo.bak
				FileName = string.Format("{0}-brewgr.xml", StringCleaner.CleanForUrl(recipe.RecipeName)),

				// always prompt the user for downloading, set to true if you want 
				// the browser to try to show the file inline
				Inline = false,
			};
			Response.AppendHeader("Content-Disposition", disposition.ToString());
			return new FileStreamResult(new MemoryStream(xmlBytes), "text/xml");
		}

		#endregion

		#region IMPORT 

		/// <summary>
		/// Executes the View for ImportBeerXml
		/// </summary>
		[ForceHttps]
		public ViewResult ImportBeerXmlDialog()
		{
			return View("~/Views/Recipe/ImportBeerXmlDialog.cshtml");
		}

		/// <summary>
		/// Executes the Http Post View for ImportBeerXml
		/// </summary>
		[HttpPost]
		[ForceHttps]
		public ActionResult ImportBeerXmlDialog(HttpPostedFileBase beerXmlFile)
		{
			if (beerXmlFile == null || beerXmlFile.ContentLength <= 0)
			{
				this.ForwardMessage(new ErrorMessage { Text = "Please choose a BeerXML file to import." });
				return RedirectToAction("homebrew-recipe-calculator");
			}

			var reader = new StreamReader(beerXmlFile.InputStream);
			var xmlContent = reader.ReadToEnd();
			IList<Recipe> recipes;
			try
			{
				recipes = this.BeerXmlImporter.ImportMany(xmlContent);
			}
			catch (Exception ex)
			{
				this.LogHandledException(ex);
				this.ForwardMessage(new ErrorMessage { Text = "Import failed. Please choose a valid BeerXML file containing at least one recipe." });
				return RedirectToAction("homebrew-recipe-calculator");
			}

			if (!recipes.Any())
			{
				this.ForwardMessage(new ErrorMessage { Text = "Import failed. The BeerXML file did not contain any recipes." });
				return RedirectToAction("homebrew-recipe-calculator");
			}

			this.Session[BeerXmlImportQueueKey] = recipes;
			if (recipes.Count == 1) return RedirectToAction("ContinueBeerXmlImport");

			return View("~/Views/Recipe/ImportBeerXmlReview.cshtml", recipes);
		}

		[HttpPost]
		[ForceHttps]
		public ActionResult ImportBeerXmlSelection(int[] recipeIndexes)
		{
			var imported = this.Session[BeerXmlImportQueueKey] as IList<Recipe>;
			var selected = (recipeIndexes ?? new int[0]).Distinct().Where(x => imported != null && x >= 0 && x < imported.Count)
				.Select(x => imported[x]).ToList();
			if (!selected.Any())
			{
				this.ForwardMessage(new ErrorMessage { Text = "Choose at least one recipe to import." });
				return RedirectToAction("homebrew-recipe-calculator");
			}

			this.Session[BeerXmlImportQueueKey] = selected;
			return RedirectToAction("ContinueBeerXmlImport");
		}

		[ForceHttps]
		public ActionResult ContinueBeerXmlImport()
		{
			var queue = this.Session[BeerXmlImportQueueKey] as IList<Recipe>;
			if (queue == null || !queue.Any())
			{
				this.Session.Remove(BeerXmlImportQueueKey);
				return RedirectToAction("homebrew-recipe-calculator");
			}

			var recipe = queue.First();
			queue.RemoveAt(0);
			if (!queue.Any()) this.Session.Remove(BeerXmlImportQueueKey);
			else this.Session[BeerXmlImportQueueKey] = queue;

			this.AppendMessage(new InfoMessage { Text = "BeerXML recipe imported. Review it, then save to continue with the next selected recipe." });
			ViewBag.RecipeCreationOptions = this.RecipeService.GetRecipeCreationOptions();
			return View("~/Views/Recipe/NewRecipe.cshtml", Mapper.Map(recipe, new RecipeViewModel()));
		}

		#endregion

		#region SPECIAL PAGES

		[Route("pliny-the-elder-clone-recipes")]
		public ActionResult PlinyTheElderClones()
		{
			var recipes = this.RecipeService.GetPopularRecipeClones("pliny");
			return this.View(recipes);
		}

		#endregion
	}
}
