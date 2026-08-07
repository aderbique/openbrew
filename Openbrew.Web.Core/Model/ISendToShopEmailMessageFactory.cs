using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Openbrew.Web.Core.Configuration;
using Openbrew.Web.Core.Service;
using ctorx.Core.Crypto;
using ctorx.Core.Email;

namespace Openbrew.Web.Core.Model
{
	public interface ISendToShopEmailMessageFactory
	{
		/// <summary>
		/// Makes one or more email messages for a send to shop order
		/// </summary>
		IList<IEmailMessage> Make(SendToShopOrder sendToShopOrder);
	}

	public class DefaultSendToShopEmailMessageFactory : ISendToShopEmailMessageFactory
	{
		readonly IUserService UserService;
		readonly IPartnerService PartnerService;
		readonly IStringCryptoService StringCryptoService;
		readonly IWebSettings WebSettings;
		readonly IRecipeDataService RecipeDataService;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public DefaultSendToShopEmailMessageFactory(IUserService userService, IPartnerService partnerService, IStringCryptoService stringCryptoService,
			IWebSettings webSettings, IRecipeDataService recipeDataService)
		{
			this.UserService = userService;
			this.PartnerService = partnerService;
			this.StringCryptoService = stringCryptoService;
			this.WebSettings = webSettings;
			this.RecipeDataService = recipeDataService;
		}

		/// <summary>
		/// Makes one or more email messages for a send to shop order
		/// </summary>
		public IList<IEmailMessage> Make(SendToShopOrder sendToShopOrder)
		{
			if (sendToShopOrder == null)
			{
				throw new ArgumentNullException("sendToShopOrder");
			}

			var user = this.UserService.GetUserById(sendToShopOrder.UserId);
			if (user == null)
			{
				throw new NotSupportedException("Could not locate user for ID=" + sendToShopOrder.UserId);
			}

			var partner = this.PartnerService.GetPartnerById(sendToShopOrder.PartnerId);
			if (partner == null)
			{
				throw new NotSupportedException("Could not locate partner for ID=" + sendToShopOrder.PartnerId);
			}

			var partnerSendToShopSettings = this.PartnerService.GetPartnerSendToShopSettings(sendToShopOrder.PartnerId);
			if (partnerSendToShopSettings == null)
			{
				throw new NotSupportedException("There are no Send-To-Shop settings for the provided Partner");
			}

			var messages = new List<IEmailMessage>();

			switch ((SendToShopOrderStatus)sendToShopOrder.SendToShopOrderStatusId)
			{
				case SendToShopOrderStatus.Cancelled:
					messages.Add(new SendToShopCancellationMessage(sendToShopOrder, partner, partnerSendToShopSettings, this.WebSettings));
					break;

				case SendToShopOrderStatus.Created:
					
					// Shop Notification
					messages.Add(new SendToShopOrderPartnerNotificationMessage(partner, partnerSendToShopSettings, sendToShopOrder,
						user, this.StringCryptoService, this.WebSettings, this.RecipeDataService));

					// User Confirmation
					messages.Add(new SendToShopConfirmationMessage(sendToShopOrder, partner, partnerSendToShopSettings,
						this.WebSettings));
			
					break;

				case SendToShopOrderStatus.OnHold:
					messages.Add(new SendToShopOnHoldMessage(sendToShopOrder, partner, partnerSendToShopSettings, this.WebSettings));
					break;
	
				case SendToShopOrderStatus.InProgress:
					messages.Add(new SendToShopInProgressMessage(sendToShopOrder, partner, partnerSendToShopSettings, this.WebSettings));
					break;
				
				case SendToShopOrderStatus.ReadyForPickup:
					messages.Add(new SendToShopReadyMessage(sendToShopOrder, partner, partnerSendToShopSettings, this.WebSettings));
					break;
				
				default:
					throw new NotSupportedException("No Message available for this status level");
			}

			return messages;
		}
	}
}