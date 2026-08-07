using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Openbrew.Web.Core.Configuration;
using Openbrew.Web.Core.Service;
using ctorx.Core.Crypto;
using ctorx.Core.Email;

namespace Openbrew.Web.Core.Model
{
	public class SendToShopOrderPartnerNotificationMessage : AbstractEmailMessage
	{
		readonly Partner Partner;
		readonly SendToShopOrder SendToShopOrder;
		readonly User User;
		readonly IStringCryptoService StringCryptoService;
		readonly IWebSettings WebSettings;
		readonly IList<IIngredient> Ingredients;


		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public SendToShopOrderPartnerNotificationMessage(Partner partner, PartnerSendToShopSettings partnerSendToShopSettings, 
			SendToShopOrder sendToShopOrder, User user, IStringCryptoService stringCryptoService, IWebSettings webSettings,
			IRecipeDataService recipeDataService)
		{
			this.Partner = partner;
			this.SendToShopOrder = sendToShopOrder;
			this.User = user;
			this.StringCryptoService = stringCryptoService;
			this.WebSettings = webSettings;

			this.Ingredients = recipeDataService.GetAllPublicIngredients();

			// Message Setup
			this.FormatAsHtml = true;
			this.Subject = "Brewgr.com - Send-To-Shop Order #:" + sendToShopOrder.SendToShopOrderId;
			this.SenderAddress = webSettings.SenderAddress;
			this.SenderDisplayName = webSettings.SenderDisplayName;
			this.ToRecipients.Add(partnerSendToShopSettings.DeliveryEmailAddress);
		}

		/// <summary>
		/// Gets an Ingredient Name
		/// </summary>
		string GetIngredientName(IngredientType ingredientType, int ingredientId)
		{
			var ingredient = this.Ingredients.FirstOrDefault(x => x.IngredientTypeId == (int)ingredientType && x.IngredientId == ingredientId);
			if(ingredient == null)
			{
				// NOTE: This shouldn't happen ... only public ingredients allowed for Send-To-Shop
				throw new NotSupportedException("Send-To-Shop Ingredient is not a public ingredient");
			}

			return ingredient.Name;
		}

		/// <summary>
		/// Builds the message body
		/// </summary>
		public override string BuildMessageBody()
		{
			var message = new StringBuilder();

			message.AppendLine("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\">");
			message.AppendLine("<html lang=\"en\">");
			message.AppendLine("    <head>");
			message.AppendLine("        <title>Brewgr Send-To-Shop Order</title>");
			message.AppendLine("    </head>");
			message.AppendLine("    <body>");
			message.AppendLine("        <div style=\"font-family: Arial;\">");
			
			message.AppendLine("Hello " + this.Partner.ContactName + ",<Br />");
			message.AppendLine("<br />");
			message.AppendLine("You've received a new Brewgr Send-To-Shop order.<br />");
			message.AppendLine("<br />");
			message.AppendLine("            <hr size=\"1\"style=\"height: 1px;\" />");
			message.AppendLine("            <b style=\"font-size: 18px;\">Customer Details:</b><br />");
			message.AppendLine("            <hr size=\"1\"style=\"height: 1px;\" />");
			message.AppendLine("            <table cellspacing=\"0\" cellpadding=\"0\" width=\"400\">");
			message.AppendLine("                <tr>");
			message.AppendLine("                    <td width=\"100\" style=\"width: 100px;\"><b>Name:</b></td>");
			message.AppendLine("                    <td>" + this.SendToShopOrder.Name + "</td>");
			message.AppendLine("                </tr>");
			message.AppendLine("                <tr>");
			message.AppendLine("                    <td><b>Username:</b></td>");
			message.AppendLine("                    <td>" + this.User.CalculatedUsername + "</td>");
			message.AppendLine("                </tr>");
			message.AppendLine("                <tr>");
			message.AppendLine("                    <td><b>Email:</b></td>");
			message.AppendLine("                    <td>" + this.SendToShopOrder.EmailAddress + "</td>");
			message.AppendLine("                </tr>");
			message.AppendLine("                <tr>");
			message.AppendLine("                    <td><b>Phone:</b></td>");
			message.AppendLine("                    <td>" + this.SendToShopOrder.PhoneNumber + (this.SendToShopOrder.AllowTextMessages ? " (Text Messages OK)" : "") + "</td>");
			message.AppendLine("                </tr>");
			message.AppendLine("            </table>");
			message.AppendLine("<br />");
			message.AppendLine("            <hr size=\"1\"style=\"height: 1px;\" />");
			message.AppendLine("            <b style=\"font-size: 18px;\">Order Details:</b><br />");
			message.AppendLine("            <hr size=\"1\"style=\"height: 1px;\" />");
			message.AppendLine("            <table cellspacing=\"0\" cellpadding=\"0\" width=\"400\">");
			message.AppendLine("                <tr>");
			message.AppendLine("                    <td width=\"100\" style=\"width: 100px;\"><b>Order Id:</b></td>");
			message.AppendLine("                    <td>" + this.SendToShopOrder.SendToShopOrderId + "</td>");
			message.AppendLine("                </tr>");
			message.AppendLine("                <tr>");
			message.AppendLine("                    <td><b>Order Date:</b></td>");
			message.AppendLine("                    <td>" + this.SendToShopOrder.DateCreated + "</td>");
			message.AppendLine("                </tr>");
			message.AppendLine("            </table>");
			message.AppendLine("<br />");

			message.AppendLine("            <hr size=\"1\"style=\"height: 1px;\" />");
			message.AppendLine("            <b style=\"font-size: 18px;\">Products:</b><br />");
			message.AppendLine("            <hr size=\"1\"style=\"height: 1px;\" />");

			// Fermentables
			if (this.SendToShopOrder.Items.Any(x => x.IngredientTypeId == (int)IngredientType.Fermentable))
			{
				message.AppendLine("<br />");
				message.AppendLine("<b>Fermentables:</b><br />");
				foreach (var ferm in this.SendToShopOrder.Items.Where(x => x.IngredientTypeId == (int)IngredientType.Fermentable).OrderByDescending(x => x.Quantity))
				{
					message.AppendLine(ferm.Quantity.ToString("F4") + " " + ferm.Unit + " - " + this.GetIngredientName(IngredientType.Fermentable, ferm.IngredientId) + (!string.IsNullOrWhiteSpace(ferm.Instructions) ? " (" + ferm.Instructions + ")" : ""));
					message.AppendLine("<br />");
				}
			}

			// Hops
			if (this.SendToShopOrder.Items.Any(x => x.IngredientTypeId == (int)IngredientType.Hop))
			{
				message.AppendLine("<br />");
				message.AppendLine("<b>Hops:</b><br />");
				foreach (var hop in this.SendToShopOrder.Items.Where(x => x.IngredientTypeId == (int)IngredientType.Hop).OrderByDescending(x => x.Quantity))
				{
					message.AppendLine(hop.Quantity.ToString("F4") + " " + hop.Unit + " - " + this.GetIngredientName(IngredientType.Hop, hop.IngredientId) + (!string.IsNullOrWhiteSpace(hop.Instructions) ? " (" + hop.Instructions + ")" : ""));
					message.AppendLine("<br />");
				}
			}

			// Yeast
			if (this.SendToShopOrder.Items.Any(x => x.IngredientTypeId == (int)IngredientType.Yeast))
			{
				message.AppendLine("<br />");
				message.AppendLine("<b>Yeast:</b><br />");
				foreach (var yeast in this.SendToShopOrder.Items.Where(x => x.IngredientTypeId == (int)IngredientType.Yeast))
				{
					message.AppendLine("1 - " + this.GetIngredientName(IngredientType.Yeast, yeast.IngredientId));
					message.AppendLine("<br />");
				}
			}

			// Adjuncts
			if (this.SendToShopOrder.Items.Any(x => x.IngredientTypeId == (int)IngredientType.Adjunct))
			{
				message.AppendLine("<br />");
				message.AppendLine("<b>Other Stuff:</b><br />");
				foreach (var adjunct in this.SendToShopOrder.Items.Where(x => x.IngredientTypeId == (int)IngredientType.Adjunct).OrderByDescending(x => x.Quantity))
				{
					message.AppendLine(adjunct.Quantity.ToString("F4") + " " + adjunct.Unit + " - " + this.GetIngredientName(IngredientType.Adjunct, adjunct.IngredientId));
					message.AppendLine("<br />");
				}
			}

			// Comments / Instructions
			message.AppendLine("<br />");
			message.AppendLine("<b style=\"font-size: 18px;\">Comments / Instructions:</b><br />");
			if(!string.IsNullOrWhiteSpace(this.SendToShopOrder.Comments))
			{
				message.AppendLine(this.SendToShopOrder.Comments);
			}
			else
			{
				message.AppendLine("No comments or instructions were provided.");
			}
			message.AppendLine("<br />");
			message.AppendLine("<br />");

			var orderToken = this.StringCryptoService.Encrypt(this.SendToShopOrder.SendToShopOrderId.ToString());

			message.AppendLine("");
			message.AppendLine("");
			message.AppendLine("            <hr size=\"1\"style=\"height: 1px;\" />");
			message.AppendLine("<b style=\"font-size: 18px;\">ACTION LINKS:</b> - NOTE: Clicking these links will trigger email messages to your customers.");
			message.AppendLine("<br /><br />");

			message.AppendLine("<table cellspacing=\"0\" cellpadding=\"5\" width=\"100%\" border=\"0\">");
			message.AppendLine("                <tr>");
			message.AppendLine("                    <td style=\"width: 200px;\"><b><a href=\"" + this.WebSettings.RootPathSecure + "/sendtoshop/action/cancel/" + orderToken + "\">CANCEL</a></b></td>");
			message.AppendLine("                    <td>Cancels the order and sends the customer a cancellation email.</td>");
			message.AppendLine("                </tr>");
			message.AppendLine("                <tr>");
			message.AppendLine("                    <td><b><a href=\"" + this.WebSettings.RootPathSecure + "/sendtoshop/action/hold/" + orderToken + "\">ON-HOLD</a></b></td>");
			message.AppendLine("                    <td>Places the order on hold and sends the customer an email, requesting them to contact you.</td>");
			message.AppendLine("                </tr>");
			message.AppendLine("                <tr>");
			message.AppendLine("                    <td><b><a href=\"" + this.WebSettings.RootPathSecure + "/sendtoshop/action/started/" + orderToken + "\">IN-PROGRESS</a></b></td>");
			message.AppendLine("                    <td>Sets the order as In-Progress and sends the customer an email letting them know the order is being built.</td>");
			message.AppendLine("                </tr>");
			message.AppendLine("                <tr>");
			message.AppendLine("                    <td><b><a href=\"" + this.WebSettings.RootPathSecure + "/sendtoshop/action/ready/" + orderToken + "\">READY FOR PICKUP</a></b></td>");
			message.AppendLine("                    <td>Sets the order as Ready for Pickup and sends the customer an email letting them know their order is ready.</td>");
			message.AppendLine("                </tr>");
			message.AppendLine("                <tr>");
			message.AppendLine("                    <td><b><a href=\"" + this.WebSettings.RootPathSecure + "/sendtoshop/action/complete/" + orderToken + "\">COMPLETED</a></b></td>");
			message.AppendLine("                    <td>Sets the order as completed.  No emails are sent out.</td>");
			message.AppendLine("                </tr>");
			message.AppendLine("            </table>");


			message.AppendLine("<br /><br />");
			message.AppendLine("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
			message.AppendLine("        </div>");
			message.AppendLine("    </body>");
			message.AppendLine("</html>");

			return message.ToString();

		}
	}
}