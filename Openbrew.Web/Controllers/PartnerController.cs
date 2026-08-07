using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using AutoMapper;
using Openbrew.Web.Core.Data;
using Openbrew.Web.Core.Model;
using Openbrew.Web.Core.Service;
using Openbrew.Web.Models;
using ctorx.Core.Data;
using ctorx.Core.Messaging;

namespace Openbrew.Web.Controllers
{
	[Authorize]
	[ForceHttps]
	public class PartnerController : BrewgrController
	{
		readonly IUnitOfWorkFactory<BrewgrContext> UnitOfWorkFactory;
		readonly IUserService UserService;
		readonly IPartnerService PartnerService;
		readonly IRecipeDataService RecipeDataService;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public PartnerController(IUnitOfWorkFactory<BrewgrContext> unitOfWorkFactory, IUserService userService, IPartnerService partnerService,
			IRecipeDataService recipeDataService)
		{
			this.UnitOfWorkFactory = unitOfWorkFactory;
			this.UserService = userService;
			this.PartnerService = partnerService;
			this.RecipeDataService = recipeDataService;
		}

		/// <summary>
		/// Executes the PartnerTools View
		/// </summary>
		public ActionResult Tools()
		{
			// Fetch their linked Partners
			var partners = this.PartnerService.GetUserPartnerSummaries(this.ActiveUser.UserId);

			// No Partners?  404
			if(partners == null || !partners.Any())
			{
				return this.Issue404();
			}

			// If Only 1 Partner, Redirect to that Partner Dashboard
			// TODO: Need to finish the layout of the Partner List Page before allowing anyone to have multiple partners
			// TODO: Need to finish the layout of the Partner List Page before allowing anyone to have multiple partners
			// TODO: Need to finish the layout of the Partner List Page before allowing anyone to have multiple partners
			// TODO: Need to finish the layout of the Partner List Page before allowing anyone to have multiple partners
			// TODO: Need to finish the layout of the Partner List Page before allowing anyone to have multiple partners

			if(partners.Count == 1)
			{
				return this.RedirectToAction("Dashboard", new { partnerId = partners[0].PartnerId });
			}

			return this.View(partners);
		}

		/// <summary>
		/// Executes the Partners Dashboard View
		/// </summary>
		public ActionResult Dashboard(int partnerId)
		{
			var partner = this.GetPartnerIfAuthorized(partnerId);
			if(partner == null)
			{
				return this.Issue404();
			}

			var services = this.PartnerService.GetPartnerActiveServices(partnerId);

			return this.View(new PartnerDashboardViewModel { Partner = partner, Services = services });
		}

		#region PARTNER SETTINGS 

		/// <summary>
		/// Executes the Partner Settings View
		/// </summary>
		public ActionResult Settings(int partnerId)
		{
			var partner = this.GetPartnerIfAuthorized(partnerId);
			if (partner == null)
			{
				return this.Issue404();
			}

			this.AddCountryDataToViewBag();

			return this.View(Mapper.Map(partner, new PartnerSettingsViewModel()));
		}

		/// <summary>
		/// Executes the Partner Settings View
		/// </summary>
		[HttpPost]
		public ActionResult Settings(PartnerSettingsViewModel partnerSettingsViewModel)
		{
			if(!this.ValidateAndAppendMessages(partnerSettingsViewModel))
			{
				this.AddCountryDataToViewBag();
				return this.View(partnerSettingsViewModel);
			}

			using(var unitOfWork = this.UnitOfWorkFactory.NewUnitOfWork())
			{
				try
				{
					var partner = this.GetPartnerIfAuthorized(partnerSettingsViewModel.PartnerId);
					if (partner == null)
					{
						return this.Issue404();
					}

					Mapper.Map(partnerSettingsViewModel, partner);
					partner.DateModified = DateTime.Now;

					unitOfWork.Commit();

					this.ForwardMessage(new SuccessMessage { Text = GenericMessages.SuccessMessage });
					return RedirectToAction("Settings", new { partnerSettingsViewModel.PartnerId });
				}
				catch(Exception ex)
				{
					this.LogHandledException(ex);
					this.AddCountryDataToViewBag();
					return this.View(partnerSettingsViewModel);
				}
			}
		}

		#endregion

		#region SERVICE SETTINGS

		public ActionResult ServiceSettings(int partnerId, int partnerServiceTypeId)
		{
			// Choose Appropriate View For Service Type Id
			switch(partnerServiceTypeId)
			{
				case (int)PartnerServiceType.SendToShop:
					return RedirectToAction("SendToShopSettings", new { partnerId });
				case (int)PartnerServiceType.BrewShopDirectory:
					throw new NotImplementedException("Hold your horses, we're not there yet Batman");
				default:
					return this.Issue404();
			}
		}

		/// <summary>
		/// Executes the disable partner service view
		/// </summary>
		[HttpPost]
		public ActionResult SetPartnerServiceStatus(PartnerServiceStatusViewModel serviceStatusViewModel)
		{
			if (serviceStatusViewModel == null)
			{
				return this.Issue404();
			}

			if (serviceStatusViewModel.PartnerId <= 0)
			{
				return this.Issue404();
			}

			// Fetch the partner
			var partner = this.GetPartnerIfAuthorized(serviceStatusViewModel.PartnerId);
			if (partner == null)
			{
				return this.Issue404();
			}

			using (var unitOfWork = this.UnitOfWorkFactory.NewUnitOfWork())
			{
				try
				{
					// Fetch the Partner Service
					var partnerService = this.PartnerService.GetPartnerServiceByType(serviceStatusViewModel.PartnerId, serviceStatusViewModel.PartnerServiceType);
					if (partnerService == null)
					{
						return this.Issue404();
					}

					partnerService.IsPublic = serviceStatusViewModel.IsEnabled;

					unitOfWork.Commit();

					return this.View("_ServiceStatus", serviceStatusViewModel);
				}
				catch (Exception ex)
				{
					this.LogHandledException(ex);
					unitOfWork.Rollback();
					return this.Issue500();
				}
			}
		}

		#endregion

		#region SEND TO SHOP 

		/// <summary>
		/// Executes the Send To Shop Settings View
		/// </summary>
		public ActionResult SendToShopSettings(int partnerId)
		{
			var partner = this.GetPartnerIfAuthorized(partnerId);
			if (partner == null)
			{
				return this.Issue404();
			}

			// Used for the Title
			ViewBag.Title = partner.Name;

			// Service not Active or Not Setup, throw 404
			var partnerService = this.PartnerService.GetPartnerServiceByType(partnerId, PartnerServiceType.SendToShop);
			if(partnerService == null || !partnerService.IsActive)
			{
				return this.Issue404();
			}

			var partnerSendToShopSettings = this.PartnerService.GetPartnerSendToShopSettings(partnerId);

			// No settings -- these needs to be setup in advance 
			if(partnerSendToShopSettings == null)
			{
				return this.Issue404();
			}

			ViewBag.ServiceIsEnabled = partnerService.IsPublic;

			return this.View(partnerSendToShopSettings);
		}

		/// <summary>
		/// Executes the SendToShopSettings post view
		/// </summary>
		[HttpPost]
		public ActionResult SendToShopSettings(PartnerSendToShopSettings partnerSendToShopSettings)
		{
			var partner = this.GetPartnerIfAuthorized(partnerSendToShopSettings.PartnerId);
			if (partner == null)
			{
				return this.Issue404();
			}

			ViewBag.Title = partner.Name;

			using(var unitOfWork = this.UnitOfWorkFactory.NewUnitOfWork())
			{
				try
				{
					var currentSettings = this.PartnerService.GetPartnerSendToShopSettings(partnerSendToShopSettings.PartnerId);
					Mapper.Map(partnerSendToShopSettings, currentSettings);
					unitOfWork.Commit();
					return new EmptyResult();
				}
				catch(Exception ex)
				{
					unitOfWork.Rollback();
					this.LogHandledException(ex);
					return this.Issue500();
				}	
			}
		}

		/// <summary>
		/// Executes the SendToShopIngredients View
		/// </summary>
		public ActionResult SendToShopIngredients(int partnerId)
		{
			var partner = this.GetPartnerIfAuthorized(partnerId);
			if (partner == null)
			{
				return this.Issue404();
			}

			ViewBag.Title = partner.Name;

			// Ingredients Lists
			var fermentables = this.RecipeDataService.GetUsableIngredients<Fermentable>(null).OrderBy(x => x.Name).ToList();
			ViewBag.Fermentables = fermentables;

			var hops = this.RecipeDataService.GetUsableIngredients<Hop>(null).OrderBy(x => x.Name).ToList();
			ViewBag.Hops = hops;

			var yeasts = this.RecipeDataService.GetUsableIngredients<Yeast>(null);
			ViewBag.Yeasts = yeasts;

			var adjuncts = this.RecipeDataService.GetUsableIngredients<Adjunct>(null);
			ViewBag.Adjuncts = adjuncts;

			var ingredients = this.PartnerService.GetSendToShopIngredients(partnerId);

			return this.View(new SendToShopIngredientsViewModel { PartnerId = partnerId, Ingredients = ingredients });
		}

		/// <summary>
		/// Executes the SendToShopIngredients View
		/// </summary>
		[HttpPost]
		public ActionResult SendToShopIngredients(SendToShopIngredientsViewModel partnerSendToShopIngredientsViewModel)
		{
			var partner = this.GetPartnerIfAuthorized(partnerSendToShopIngredientsViewModel.PartnerId);
			if (partner == null)
			{
				return this.Issue404();
			}

			ViewBag.Title = partner.Name;

			using(var unitOfWork = this.UnitOfWorkFactory.NewUnitOfWork())
			{
				try
				{
					// Parse the Json
					var serializer = new JavaScriptSerializer();

					var postedIngs = serializer.Deserialize<List<PartnerSendToShopIngredient>>(
							partnerSendToShopIngredientsViewModel.IngredientJson);

					// Fetch Existing Ingredients
					var existingIngs = this.PartnerService.GetSendToShopIngredients(partner.PartnerId);

					// Determine New/Removed Ingredients
					var newIngs = postedIngs.Where(x => !existingIngs.Any(y => y.IngredientTypeId == x.IngredientTypeId && y.IngredientId == x.IngredientId)).ToList();
					var removedIngs = existingIngs.Where(x => !postedIngs.Any(y => y.IngredientTypeId == x.IngredientTypeId && y.IngredientId == x.IngredientId)).ToList();

					// Do the Adds
					if(newIngs.Any())
					{
						newIngs.ForEach(x => this.PartnerService.Add(x));
					}

					// Do the Deletes
					if(removedIngs.Any())
					{
						removedIngs.ForEach(x => this.PartnerService.Delete(x));
					}


					// This can be slow when a lot of ingredients are added..consider other options
					// for saving as needed to speed it up
					unitOfWork.Commit();

					return new EmptyResult();
				}
				catch(Exception ex)
				{
					this.LogHandledException(ex);
					unitOfWork.Rollback();
					return this.Issue500();
				}
			}
		}

		#endregion

		#region PARTNER ACCESS

		/// <summary>
		/// Gets a partner and verifies authorization.  This will return null if
		/// the partner does not exist, or the current user is not authorized to
		/// act on their behalf
		/// </summary>
		Partner GetPartnerIfAuthorized(int partnerId)
		{
			var partner = this.PartnerService.GetPartnerById(partnerId);

			// Partner does not exist, 404
			if (partner == null)
			{
				return null;
			}

			// Verify Partner Access
			if (!this.PartnerService.UserIsPartnerAdmin(this.ActiveUser.UserId, partnerId))
			{
				return null;
			}

			return partner;
		}

		#endregion
	}
}