using System;
using System.Web.Mvc;
using AutoMapper;
using Openbrew.Web.Core.Data;
using Openbrew.Web.Core.Model;
using Openbrew.Web.Core.Service;
using Openbrew.Web.Models;
using ctorx.Core.Crypto;
using ctorx.Core.Data;
using ctorx.Core.Messaging;

namespace Openbrew.Web.Controllers
{
	public class SendToShopController : BrewgrController
	{
		readonly IUnitOfWorkFactory<BrewgrContext> UnitOfWorkFactory;
		readonly IPartnerIdResolver PartnerIdResolver;
		readonly IPartnerService PartnerService;
		readonly IRecipeService RecipeService;
		readonly ISendToShopService SendToShopService;
		readonly IStringCryptoService StringCryptoService;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public SendToShopController(IUnitOfWorkFactory<BrewgrContext> unitOfWorkFactory, IPartnerIdResolver partnerIdResolver, IPartnerService partnerService, 
			IRecipeService recipeService, ISendToShopService sendToShopService, IStringCryptoService stringCryptoService)
		{
			this.UnitOfWorkFactory = unitOfWorkFactory;
			this.PartnerIdResolver = partnerIdResolver;
			this.PartnerService = partnerService;
			this.RecipeService = recipeService;
			this.SendToShopService = sendToShopService;
			this.StringCryptoService = stringCryptoService;
		}

		/// <summary>
		/// Called when authorization occurs.
		/// </summary>
		/// <param name="filterContext">Information about the current request and action.</param>
		protected override void OnAuthorization(AuthorizationContext filterContext)
		{
			if(this.ActiveUser == null)
			{
				this.ForwardMessage(new InfoMessage { Text = "You must be logged in to use the Send-To-Shop Service" });
			}

			base.OnAuthorization(filterContext);
		}

		/// <summary>
		/// Executes the Send to Shop view
		/// </summary>
		[Authorize]
		public ActionResult Create(int recipeId)
		{
			var partnerId = this.PartnerIdResolver.Resolve();

			if (partnerId == null)
			{
				return this.Issue404();
			}

			var partner = this.PartnerService.GetPartnerById(partnerId.Value);

			if (partner == null)
			{
				return this.Issue404();
			}

			var model = new SendToShopOrderViewModel();

			// Make the Data Available
			model.Partner = partner;
			model.PartnerIngredients = this.PartnerService.GetSendToShopIngredients(partnerId.Value);
			model.PartnerSettings = this.PartnerService.GetPartnerSendToShopSettings(partnerId.Value);
			model.Recipe = this.RecipeService.GetRecipeById(recipeId);

			if (model.PartnerIngredients == null || model.PartnerSettings == null || model.Recipe == null)
			{
				return this.Issue404();
			}

			var orderShell = this.SendToShopService.GetSendToShopOrderShell(this.ActiveUser.UserId, recipeId);

			return this.View(Mapper.Map(orderShell, model));
		}

		/// <summary>
		/// Executes the CreateOrder post view
		/// </summary>
		[HttpPost]
		[Authorize]
		public ActionResult CreateOrder(SendToShopOrderViewModel sendToShopOrderViewModel)
		{
			if(!this.ValidateWithoutMessaging(sendToShopOrderViewModel))
			{
				return Content("0");
			}

			using(var unitOfWork = this.UnitOfWorkFactory.NewUnitOfWork())
			{
				try
				{
					var sendToShopOrder = Mapper.Map(sendToShopOrderViewModel, new SendToShopOrder());
					sendToShopOrder.UserId = this.ActiveUser.UserId;
					this.SendToShopService.PlaceOrder(sendToShopOrder);

					unitOfWork.Commit();

					// Notify Shop
					this.SendToShopService.Notify(sendToShopOrder.SendToShopOrderId);

					// Update Statuses with Commit
					sendToShopOrder.SendToShopOrderStatusId = (int)SendToShopOrderStatus.SentToShop;
					unitOfWork.Commit();

					return Content("1");
				}
				catch(Exception ex)
				{
					this.LogHandledException(ex);
					unitOfWork.Rollback();
					return this.Content("-1"); // General Failure
				}
			}
		}

		/// <summary>
		/// Executes the Confirmation view
		/// </summary>
		[Authorize]
		public ActionResult Confirmation()
		{
			var partnerId = this.PartnerIdResolver.Resolve();
			if(!partnerId.HasValue)
			{
				return this.Issue404();
			}

			ViewBag.Partner = this.PartnerService.GetPartnerById(partnerId.Value);

			return this.View();
		}

		/// <summary>
		/// Handles an action link for Send-To-Shop
		/// </summary>
		public ActionResult Action(string actionToPerform, string token)
		{
			// TODO: Add Check to ensure user is logged in and they are a Partner Admin

			SendToShopOrderStatus status = SendToShopOrderStatus.Created;

			switch(actionToPerform.ToLower())
			{
				case "cancel":
					status = SendToShopOrderStatus.Cancelled;
					break;
				case "hold":
					status = SendToShopOrderStatus.OnHold;
					break;
				case "started":
					status = SendToShopOrderStatus.InProgress;
					break;
				case "ready":
					status = SendToShopOrderStatus.ReadyForPickup;
					break;
				case "complete":
					status = SendToShopOrderStatus.Completed;
					break;
				default:
					throw new InvalidOperationException("Status not supported");
			}

			using (var unitOfWork = this.UnitOfWorkFactory.NewUnitOfWork())
			{
				try
				{
					var sendToShopOrderId = Int32.Parse(this.StringCryptoService.Decrypt(token));

					// Update Status
					this.SendToShopService.UpdateStatus(sendToShopOrderId, status);

					// Commit and Notify
					unitOfWork.Commit();

					// Only Notify if it is not the Complete Status
					if(status != SendToShopOrderStatus.Completed)
					{
						this.SendToShopService.Notify(sendToShopOrderId);
					}

					return Content("Success");
				}
				catch (Exception ex)
				{
					this.LogHandledException(ex);
					return Content("Failure\n\n" + ex.ToString()); // TODO: Remove the error message after testing is solid
				}
			}	
		}
	}
}