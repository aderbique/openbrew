using System;
using System.Text;
using ctorx.Core.Email;
using Openbrew.Web.Core.Configuration;

namespace Openbrew.Web.Email
{
	public class PasswordResetEmailMessage : AbstractEmailMessage
	{
		readonly IWebSettings WebSettings;
		string AuthToken;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public PasswordResetEmailMessage(IWebSettings webSettings)
		{
			this.WebSettings = webSettings;

			this.SenderAddress = webSettings.SenderAddress;
			this.SenderDisplayName = webSettings.SenderDisplayName;
			this.Subject = "Password Reset for openbrew";
			this.FormatAsHtml = false;
		}

		/// <summary>
		/// Sets the Auth Token
		/// </summary>
		/// <param name="authToken"></param>
		public void SetAuthToken(string authToken)
		{
			this.AuthToken = authToken;
		}

		/// <summary>
		/// Builds the message body
		/// </summary>
		public override string BuildMessageBody()
		{
			var message = new StringBuilder();

			message.AppendLine("Hello,");
			message.AppendLine();
			message.AppendLine("We've received a request to reset your password for openbrew");
			message.AppendLine("To initiate the process, please click the following link:");
			message.AppendLine();
			message.AppendFormat("<{0}/set-password/{1}>", this.WebSettings.RootPathSecure, this.AuthToken);
			message.AppendLine();
			message.AppendLine();
			message.AppendLine("If clicking the link above does not work, copy and paste the URL in");
			message.AppendLine("a new browser window instead. The URL will expire in 8 hours for security");
			message.AppendLine("reasons.");
			message.AppendLine();
			message.AppendLine("Please disregard this message if you did not make a password reset request.");
			message.AppendLine();
			message.AppendLine("This is an automatically generated message. Replies are not monitored or");
			message.AppendLine("answered.");
			message.AppendLine();
			message.AppendLine("Happy Brewing,");
			message.AppendLine("The openbrew Team");


			return message.ToString();
		}
	}
}
