using System;
using System.Text;
using ctorx.Core.Email;
using Openbrew.Web.Core.Configuration;

namespace Openbrew.Web.Email
{
	public class NewsletterConfirmationEmailMessage : AbstractEmailMessage
	{
		readonly IWebSettings WebSettings;
		readonly string ConfirmationToken;

		public NewsletterConfirmationEmailMessage(IWebSettings webSettings, string emailAddress, string confirmationToken)
		{
			this.WebSettings = webSettings;
			this.ConfirmationToken = confirmationToken;
			this.SenderAddress = webSettings.SenderAddress;
			this.SenderDisplayName = webSettings.SenderDisplayName;
			this.Subject = "Confirm your OpenBrew newsletter subscription";
			this.FormatAsHtml = true;
			this.ToRecipients.Add(emailAddress);
		}

		public override string BuildMessageBody()
		{
			var root = this.WebSettings.RootPathSecure.TrimEnd('/');
			var confirmUrl = root + "/newsletter/confirm/" + this.ConfirmationToken;
			var unsubscribeUrl = root + "/newsletter/unsubscribe/" + this.ConfirmationToken;
			var message = new StringBuilder();
			message.Append("<p>Thanks for joining OpenBrew.</p>");
			message.Append("<p>Please confirm that you want occasional updates, tips, and release notes by selecting the link below.</p>");
			message.AppendFormat("<p><a href=\"{0}\">Confirm my subscription</a></p>", confirmUrl);
			message.Append("<p>If you did not request this, you can ignore this email.</p>");
			message.AppendFormat("<p style=\"color:#6b625c;font-size:12px\">Changed your mind? <a href=\"{0}\">Unsubscribe</a>.</p>", unsubscribeUrl);
			return message.ToString();
		}
	}
}
