using System.Text;
using Openbrew.Web.Core.Configuration;
using ctorx.Core.Email;

namespace Openbrew.Web.Core.Model
{
	public class SendToShopInProgressMessage : AbstractEmailMessage
	{
		readonly SendToShopOrder SendToShopOrder;
		readonly Partner Partner;
		readonly PartnerSendToShopSettings PartnerSendToShopSettings;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public SendToShopInProgressMessage(SendToShopOrder sendToShopOrder, Partner partner, PartnerSendToShopSettings partnerSendToShopSettings,
			IWebSettings webSettings)
		{
			this.SendToShopOrder = sendToShopOrder;
			this.Partner = partner;
			this.PartnerSendToShopSettings = partnerSendToShopSettings;

			this.FormatAsHtml = false;
			this.ToRecipients.Add(sendToShopOrder.EmailAddress);
			this.Subject = "IN-PROGRESS: Send-To-Shop Order #: " + sendToShopOrder.SendToShopOrderId;
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

			message.AppendLine("This message is to inform you that your Send-To-Shop order #: " + this.SendToShopOrder.SendToShopOrderId + " is currently being prepared.  You will receive an additional email message when it is ready for pickup.");
			message.AppendLine();

			message.AppendLine("If you have any questions regarding this order, please contact " + this.Partner.Name + ".");
			message.AppendLine();

			message.AppendLine("Happy Brewing!");
			message.AppendLine("Brewgr.com on behalf of " + this.Partner.Name);

			return message.ToString();
		}
	}
}