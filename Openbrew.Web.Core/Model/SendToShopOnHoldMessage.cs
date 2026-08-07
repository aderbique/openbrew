using System.Text;
using Openbrew.Web.Core.Configuration;
using ctorx.Core.Email;

namespace Openbrew.Web.Core.Model
{
	public class SendToShopOnHoldMessage : AbstractEmailMessage
	{
		readonly SendToShopOrder SendToShopOrder;
		readonly Partner Partner;
		readonly PartnerSendToShopSettings PartnerSendToShopSettings;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public SendToShopOnHoldMessage(SendToShopOrder sendToShopOrder, Partner partner, PartnerSendToShopSettings partnerSendToShopSettings,
			IWebSettings webSettings)
		{
			this.SendToShopOrder = sendToShopOrder;
			this.Partner = partner;
			this.PartnerSendToShopSettings = partnerSendToShopSettings;

			this.FormatAsHtml = false;
			this.ToRecipients.Add(sendToShopOrder.EmailAddress);
			this.Subject = "ON-HOLD: Send-To-Shop Order #: " + sendToShopOrder.SendToShopOrderId;
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

			message.AppendLine("This message is to inform you that your Send-To-Shop order #: " + this.SendToShopOrder.SendToShopOrderId + " has been placed on hold.");
			
			message.AppendLine("Please contact " + this.Partner.ContactName + " at " + 
				this.Partner.ContactPhone + " at your earliest convenience.  We will not be able to continue processing your order until you contact us.");

			message.AppendLine();

			message.AppendLine("Cheers!");
			message.AppendLine("Brewgr.com on behalf of " + this.Partner.Name);

			return message.ToString();
		}
	}
}