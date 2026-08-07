using System;
using System.Linq;
using System.Web.Mvc;
using Openbrew.Web.Core.Model;
using Openbrew.Web.Core.Service;
using Openbrew.Web.Models;
using ctorx.Core.Collections;
using ctorx.Core.Linq;

namespace Openbrew.Web.Controllers
{
	public class IngredientController : BrewgrController
	{
		readonly IRecipeDataService RecipeDataService;
		readonly IIngredientCategorizer IngredientCategorizer;
		readonly IAffiliateService AffiliateService;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public IngredientController(IRecipeDataService recipeDataService, IIngredientCategorizer ingredientCategorizer,
			IAffiliateService affiliateService)
		{
			this.RecipeDataService = recipeDataService;
			this.IngredientCategorizer = ingredientCategorizer;
			this.AffiliateService = affiliateService;
		}

		/// <summary>
		/// Executes the View for Fermentables
		/// </summary>
		public ViewResult Fermentables()
		{
			var fermentables = this.RecipeDataService.GetUsableIngredients<Fermentable>(null).OrderBy(x => x.Lovibond).ThenBy(x => x.Name).ToList();
			var topTenIds = this.RecipeDataService.GetTopFermentableIds();

			var model = new IngredientListViewModel<Fermentable>(this.IngredientCategorizer.Categorize(fermentables), topTenIds);

			return View(model);
		}

		/// <summary>
		/// Executes the View for Hops
		/// </summary>
		public ViewResult Hops()
		{
			var hops = this.RecipeDataService.GetUsableIngredients<Hop>(null).OrderBy(x => x.Name).ThenBy(x => x.Name).ToList();
			var topTenIds = this.RecipeDataService.GetTopHopIds();

			var model = new IngredientListViewModel<Hop>(this.IngredientCategorizer.Categorize(hops), topTenIds);

			return View(model);
		}

		/// <summary>
		/// Executes the View for Yeasts
		/// </summary>
		public ViewResult Yeasts()
		{
			var yeasts = this.RecipeDataService.GetUsableIngredients<Yeast>(null).OrderBy(x => x.Name).ThenBy(x => x.Name).ToList();
			var topTenIds = this.RecipeDataService.GetTopYeastIds();

			var model = new IngredientListViewModel<Yeast>(this.IngredientCategorizer.Categorize(yeasts), topTenIds);

			return View(model);
		}

		/// <summary>
		/// Executes the View for Adjuncts
		/// </summary>
		public ViewResult Adjuncts()
		{
			var adjuncts = this.RecipeDataService.GetUsableIngredients<Adjunct>(null).OrderBy(x => x.Name).ThenBy(x => x.Name).ToList();
			var topTenIds = this.RecipeDataService.GetTopAdjunctIds();

			var model = new IngredientListViewModel<Adjunct>(this.IngredientCategorizer.Categorize(adjuncts), topTenIds);

			return View(model);
		}

		//public ActionResult IngredientDetail<TIngredientType>(string viewName, int ingredientId, int? page) where TIngredientType : class, IIngredient
		//{
		//	// Get ingredient
		//	var ingredient = this.RecipeDataService.GetIngredientById<TIngredientType>(ingredientId);

		//	if (ingredient == null)
		//	{
		//		return this.Issue404();
		//	}

		//	var pager = new Pager { CurrentPage = page ?? 1, ItemsPerPage = this.WebSettings.DefaultRecipesPerPage };

		//	// Get Recipes
		//	var ingRecipes = this.RecipeDataService.GetIngredientRecipes<TIngredientType>(ingredientId, pager);

		//	// Get Best Match Product
		//	var product = this.AffiliateService.GetBestMatchProduct<TIngredientType>(ingredientId);

		//	if (ingRecipes.Any() && !pager.IsInRange())
		//	{
		//		return this.Issue404();
		//	}

		//	var model = new IngredientDetailViewModel<TIngredientType>
		//	{
		//		Ingredient = ingredient,
		//		AffiliateProduct = product,
		//		Pager = pager,
		//		Recipes = ingRecipes,
		//		BaseUrl = Url.FermentableDetailUrl(ingredientId, ingredient.Name)
		//	};

		//	return View(viewName, model);
		//}

		///// <summary>
		///// Executes the View for IngredientDetail
		///// </summary>
		//public ActionResult FermentableDetail(int ingredientId, int? page)
		//{
		//	return IngredientDetail<Fermentable>("FermentableDetail", ingredientId, page);
		//}
	}
}