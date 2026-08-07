using System.Text;
using Openbrew.Web.Core.Configuration;
using ctorx.Core.Email;

namespace Openbrew.Web.Core.Model
{
	public class SendToShopCancellationMessage : AbstractEmailMessage
	{
		readonly SendToShopOrder SendToShopOrder;
		readonly Partner Partner;
		readonly PartnerSendToShopSettings PartnerSendToShopSettings;
		readonly IWebSettings WebSettings;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public SendToShopCancellationMessage(SendToShopOrder sendToShopOrder, Partner partner, PartnerSendToShopSettings partnerSendToShopSettings,
			IWebSettings webSettings)
		{
			this.SendToShopOrder = sendToShopOrder;
			this.Partner = partner;
			this.PartnerSendToShopSettings = partnerSendToShopSettings;
			this.WebSettings = webSettings;

			this.FormatAsHtml = false;
			this.ToRecipients.Add(sendToShopOrder.EmailAddress);
			this.Subject = "CANCELLED: Send-To-Shop Order #: " + sendToShopOrder.SendToShopOrderId;
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

			message.AppendLine("This message is confirmation that your Send-To-Shop order #" + this.SendToShopOrder.SendToShopOrderId + " has been cancelled.");
			message.AppendLine();

			message.AppendLine("If you have any question regarding this order or the cancellation, please contact " + this.Partner.Name + ".");
			message.AppendLine();

			message.AppendLine("Cheers!");
			message.AppendLine("Brewgr.com on behalf of " + this.Partner.Name);

			return message.ToString();
		}
	}
}