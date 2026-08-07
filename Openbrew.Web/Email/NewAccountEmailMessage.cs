using System;
using System.Text;
using ctorx.Core.Email;
using Openbrew.Web.Core.Configuration;

namespace Openbrew.Web.Email
{
    public class NewAccountEmailMessage : AbstractEmailMessage
    {
        readonly IWebSettings WebSettings;
        string AuthToken;

        /// <summary>
		/// Franck The Tank
		/// </summary>
        public NewAccountEmailMessage(IWebSettings webSettings)
        {
            this.WebSettings = webSettings;

            this.SenderAddress = webSettings.SenderAddress;
            this.SenderDisplayName = webSettings.SenderDisplayName;
            this.Subject = "Welcome to openbrew";
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
            
            message.AppendLine("Welcome to openbrew!");
            message.AppendLine();
            message.AppendLine("We're glad that you've signed up.  You've just joined a growing community of homebrewers with tons of recipes.  If you haven't already picked a username, you can do so by navigating to:");
            message.AppendLine();
            message.AppendFormat("<{0}/settings>", this.WebSettings.RootPathSecure);
            message.AppendLine();
            message.AppendLine();
            message.AppendLine("If clicking the link above does not work, copy and paste the URL in a new browser window instead.");
            message.AppendLine();
            message.AppendLine("This is an automatically generated message. Replies are not monitored or answered.");
            message.AppendLine();
            message.AppendLine("Happy Brewing,");
            message.AppendLine("The openbrew Team");


            return message.ToString();
        }
    }
}
