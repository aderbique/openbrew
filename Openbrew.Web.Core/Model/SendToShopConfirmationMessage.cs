using System.Text;
using Openbrew.Web.Core.Configuration;
using ctorx.Core.Email;

namespace Openbrew.Web.Core.Model
{
	public class SendToShopConfirmationMessage : AbstractEmailMessage
	{
		readonly SendToShopOrder SendToShopOrder;
		readonly Partner Partner;
		readonly PartnerSendToShopSettings PartnerSendToShopSettings;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public SendToShopConfirmationMessage(SendToShopOrder sendToShopOrder, Partner partner, PartnerSendToShopSettings partnerSendToShopSettings, 
			IWebSettings webSettings)
		{
			this.SendToShopOrder = sendToShopOrder;
			this.Partner = partner;
			this.PartnerSendToShopSettings = partnerSendToShopSettings;

			this.FormatAsHtml = false;
			this.ToRecipients.Add(sendToShopOrder.EmailAddress);
			this.Subject = "Send-To-Shop Order #: " + sendToShopOrder.SendToShopOrderId;
			this.SenderAddress = webSettings.SenderAddress;
			this.SenderDisplayName = webSettings.SenderDisplayName;
		}

		/// <summary>
		/// Builds the message body
		/// </summary>
		public override string BuildMessageBody()
		{
			var message = new StringBuilder();

			message.AppendLine("Hello " + this.SendToShopOrder.Name + ", ");
			message.AppendLine();

			message.AppendLine("Thank you for your Brewgr Send-To-Shop order.  Your order number is: #" + this.SendToShopOrder.SendToShopOrderId);
			message.AppendLine();

			message.AppendLine("You will receive one or more email messages as your order is processed.  Once it is ready for pickup, you will receive a final email message. If you have any questions regarding this order, please contact " + this.Partner.Name + ".");
			message.AppendLine();

			message.AppendLine("If there are problems or questions about your order, you may be contacted at the phone number you provided.");
			message.AppendLine();

			message.AppendLine("Happy Brewing!");
			message.AppendLine("Brewgr.com on behalf of " + Partner.Name);

			return message.ToString();
		}
	}
}