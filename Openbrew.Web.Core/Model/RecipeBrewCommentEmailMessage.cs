using System;
using System.Linq;
using System.Text;
using System.Web;
using Openbrew.Web.Core.Configuration;
using ctorx.Core.Email;
using ctorx.Core.Formatting;

namespace Openbrew.Web.Core.Model
{
    public class BrewSessionCommentEmailMessage : BrewgrEmailMessage
    {
        readonly IWebSettings WebSettings;
        readonly BrewSession BrewSession;
        readonly string CommenterUsername;
        readonly UserSummary UserToNotify;
        readonly BrewSessionComment BrewSessionComment;
        readonly BrewgrUrlBuilder BrewgrUrlBuilder;

        /// <summary>
        /// ctor the Mighty
        /// </summary>
        public BrewSessionCommentEmailMessage(IWebSettings webSettings, BrewSession brewSession, string commenterUsername,
            UserSummary userToNotify, BrewSessionComment brewSessionComment, BrewgrUrlBuilder brewgrUrlBuilder)
            : base(webSettings)
        {
            this.WebSettings = webSettings;
            this.BrewSession = brewSession;
            this.CommenterUsername = commenterUsername;
            this.UserToNotify = userToNotify;
            this.BrewSessionComment = brewSessionComment;
            this.BrewgrUrlBuilder = brewgrUrlBuilder;

            // Set Sender
            this.SenderAddress = webSettings.SenderAddress;
            this.SenderDisplayName = webSettings.SenderDisplayName;

            // Set Recipient
            this.ToRecipients.Add(userToNotify.EmailAddress);

            // Build Subject
            if (brewSession.UserId == userToNotify.UserId)
            {
                this.Subject = string.Format("{0} commented on your brew session for {1}", commenterUsername, brewSession.RecipeSummary.RecipeName);
            }
            else
            {
                this.Subject = string.Format("{0} also left a comment on the brew session for {1}", commenterUsername, brewSession.RecipeSummary.RecipeName);
            }
        }

        /// <summary>
        /// Builds the Message Body
        /// </summary>
        public override string BuildMessageBody()
        {
            var message = new StringBuilder();

            message.AppendLine("Hello " + this.UserToNotify.FirstName + ",");
            message.AppendLine();

            if (this.BrewSession.UserId == this.UserToNotify.UserId)
            {
                message.AppendLine(this.CommenterUsername + " has commented on your brew session for " + this.BrewSession.RecipeSummary.RecipeName + ".");
            }
            else
            {
                message.AppendLine(this.CommenterUsername + " also commented on the brew session for " + this.BrewSession.RecipeSummary.RecipeName + ".");
            }
            message.AppendLine();

            message.AppendLine("\"" + HttpUtility.HtmlDecode(this.BrewSessionComment.CommentText.Replace(Environment.NewLine, "")) + "\"");
            message.AppendLine();

            message.AppendLine("You can respond to " + this.CommenterUsername + " by visiting the brew session page at the following link:");
            message.AppendLine();
            message.AppendLine("<" + this.BrewgrUrlBuilder.BuildBrewSessionDetailUrl(this.BrewSession) + "#comments>");
            message.AppendLine();

            message.AppendLine("If clicking the link above does not work, copy and paste the URL in");
            message.AppendLine("a new browser window instead.");
            message.AppendLine();
            message.AppendLine("Happy Brewing,");
            message.AppendLine("The Brewgr Team");

            message.Append(this.GetSubscriptionNotice());

            return message.ToString();
        }
    }
}