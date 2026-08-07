using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Openbrew.Web.Core.Configuration;
using Openbrew.Web.Core.Data;
using Openbrew.Web.Core.Model;
using ctorx.Core.Crypto;
using ctorx.Core.Email;

namespace Openbrew.Web.Core.Service
{
	public class DefaultSendToShopService : ISendToShopService
	{
		readonly IBrewgrRepository Repository;
		readonly IPartnerIdResolver PartnerIdResolver;
		readonly IPartnerService PartnerService;
		readonly IUserService UserService;
		readonly IStringCryptoService StringCryptoService;
		readonly IWebSettings WebSettings;
		readonly IEmailSender EmailSender;
		readonly IRecipeDataService RecipeDataService;
		readonly ISendToShopEmailMessageFactory SendToShopEmailMessageFactory;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public DefaultSendToShopService(IBrewgrRepository repository, IPartnerIdResolver partnerIdResolver, IPartnerService partnerService, 
			IUserService userService, IStringCryptoService stringCryptoService, IWebSettings webSettings, IEmailSender emailSender,
			IRecipeDataService recipeDataService, ISendToShopEmailMessageFactory sendToShopEmailMessageFactory)
		{
			this.Repository = repository;
			this.PartnerIdResolver = partnerIdResolver;
			this.PartnerService = partnerService;
			this.UserService = userService;
			this.StringCryptoService = stringCryptoService;
			this.WebSettings = webSettings;
			this.EmailSender = emailSender;
			this.RecipeDataService = recipeDataService;
			this.SendToShopEmailMessageFactory = sendToShopEmailMessageFactory;
		}

		/// <summary>
		/// gets the Recipe Creation Send To Shop Settings for a Partner
		/// </summary>
		public RecipeCreationSendToShopSettings GetRecipeCreationSendToShopSettings(bool includeIngredients = true)
		{
			var sendToShopSettings = new RecipeCreationSendToShopSettings();

			var partnerId = this.PartnerIdResolver.Resolve();
			if (partnerId.HasValue)
			{
				var partner = this.PartnerService.GetPartnerById(partnerId.Value);

				if (partner != null)
				{
					if (this.PartnerService.PartnerOffersService(partnerId.Value, PartnerServiceType.SendToShop))
					{
						var partnerIngredients = this.PartnerService.GetSendToShopIngredients(partnerId.Value);
						if (partnerIngredients.Any())
						{
							sendToShopSettings.PartnerId = partnerId.Value;
							sendToShopSettings.PartnerName = partner.Name;
							sendToShopSettings.IsEnabled = true;

							if (includeIngredients)
							{
								sendToShopSettings.Ingredients = partnerIngredients;
							}

							return sendToShopSettings;
						}
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Gets a Send to Shop Order Shell
		/// </summary>
		public SendToShopOrder GetSendToShopOrderShell(int userId, int recipeId)
		{
			if (userId <= 0)
			{
				throw new ArgumentOutOfRangeException("userId");
			}

			if (recipeId <= 0)
			{
				throw new ArgumentOutOfRangeException("recipeId");
			}

			var user = this.UserService.GetUserById(userId);

			if (user == null)
			{
				throw new InvalidOperationException("The User could not be retrieved");
			}

			// Fetch the last order, if any
			var userLastOrder = this.PartnerService.GetUserLastSendToShopOrder(userId);

			return new SendToShopOrder
			{
				Name = string.Concat(user.FirstName, " ", user.LastName).Trim(),
				EmailAddress = user.EmailAddress,
				PhoneNumber = userLastOrder != null ? userLastOrder.PhoneNumber : "",
				AllowTextMessages = userLastOrder != null && userLastOrder.AllowTextMessages,
				SendToShopOrderStatusId = (int)SendToShopOrderStatus.SentToShop
			};
		}

		/// <summary>
		/// Places a send to shop order
		/// </summary>
		public void PlaceOrder(SendToShopOrder sendToShopOrder)
		{
			if(sendToShopOrder == null)
			{
				throw new ArgumentNullException("sendToShopOrder");
			}

			var partnerId = this.PartnerIdResolver.Resolve();
			if(partnerId == null)
			{
				throw new NotSupportedException("Partner Id is required to create a Send-To-Shop Order");
			}

			sendToShopOrder.SendToShopOrderStatusId = (int)SendToShopOrderStatus.Created;
			sendToShopOrder.PartnerId = partnerId.Value;
			sendToShopOrder.DateCreated = DateTime.Now;

			this.Repository.Add(sendToShopOrder);
		}

		/// <summary>
		/// Updates the status of a send to shop order
		/// </summary>
		public void UpdateStatus(int orderId, SendToShopOrderStatus status)
		{
			if(orderId <= 0)
			{
				throw new ArgumentOutOfRangeException("orderId");
			}

			var sendToShopOrder = this.Repository.GetSet<SendToShopOrder>()
				.Where(x => x.SendToShopOrderId == orderId)
				.Where(x => x.SendToShopOrderStatusId != (int)status)
				.FirstOrDefault();

			if(sendToShopOrder == null)
			{
				throw new InvalidOperationException("Could not find send-to-shop order with differing status");
			}

			sendToShopOrder.SendToShopOrderStatusId = (int)status;
			sendToShopOrder.DateModified = DateTime.Now;
		}

		/// <summary>
		/// Sends a notification for a send to shop order at a given status
		/// </summary>
		public void Notify(int sendToShopOrderId, string emailAddressOverride = null)
		{
			var sendToShopOrder = this.Repository.GetSet<SendToShopOrder>()
				.Include(x => x.Items)
				.FirstOrDefault(x => x.SendToShopOrderId == sendToShopOrderId);

			if(sendToShopOrder == null)
			{
				throw new InvalidOperationException("Could not find the Send To Shop order");
			}

			var messages = this.SendToShopEmailMessageFactory.Make(sendToShopOrder);

			foreach(var message in messages)
			{
				// Force an Email Address If Needed
				if (!string.IsNullOrWhiteSpace(emailAddressOverride))
				{
					message.ToRecipients.Clear();
					message.ToRecipients.Add(emailAddressOverride);
				}

				this.EmailSender.Send(message);
			}
		}
	}
}