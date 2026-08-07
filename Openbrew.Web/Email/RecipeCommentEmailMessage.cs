using System;
using System.Linq;
using System.Text;
using Openbrew.Web.Core.Configuration;
using Openbrew.Web.Core.Model;
using ctorx.Core.Email;

namespace Openbrew.Web.Email
{
	public class RecipeCommentEmailMessage : AbstractEmailMessage
	{
		readonly RecipeSummary RecipeSummary;
		readonly string CommenterUsername;
		readonly UserSummary UserToNotify;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public RecipeCommentEmailMessage(IWebSettings webSettings, RecipeSummary recipeSummary, string commenterUsername, UserSummary userToNotify)
		{
			RecipeSummary = recipeSummary;
			CommenterUsername = commenterUsername;
			UserToNotify = userToNotify;

			this.SenderAddress = webSettings.SenderAddress;
			this.SenderDisplayName = webSettings.SenderDisplayName;

			this.ToRecipients.Add(userToNotify.EmailAddress);

			// Build Subject
			if (recipeSummary.CreatedBy == userToNotify.UserId)
			{
				this.Subject = string.Format("{0} commented on your recipe, {1}", commenterUsername, recipeSummary.RecipeName);
			}
			else
			{
				this.Subject = string.Format("{0} also left a comment on the recipe, {1}", commenterUsername, recipeSummary.RecipeName);
			}
		}

		/// <summary>
		/// Builds the Message Body
		/// </summary>
		public override string BuildMessageBody()
		{
			var message = new StringBuilder();

			message.AppendLine("Hi " + this.UserToNotify.FullName + ",");
			message.AppendLine();

			if (this.RecipeSummary.CreatedBy == this.UserToNotify.UserId)
			{
				message.AppendLine(this.CommenterUsername + " has commented on your recipe, " + this.RecipeSummary.RecipeName + ".");
			}
			else
			{
				message.AppendLine(this.CommenterUsername + " also commented on the recipe, " + this.RecipeSummary.RecipeName + ".");
			}
			message.AppendLine();
			message.AppendLine();
			message.AppendLine("You can read the comment and respond by visiting the recipe page on openbrew at the following link.");
			message.AppendLine();
			message.AppendLine("<http://127.0.0.1:8085>");

			message.AppendLine("If clicking the link above does not work, copy and paste the URL in");
			message.AppendLine("a new browser window instead.");
			message.AppendLine();
			message.AppendLine();
			message.AppendLine("Happy Brewing,");
			message.AppendLine("The openbrew Team");

			message.AppendLine();
			message.AppendLine();
			message.AppendLine();

			message.AppendLine("---------------------------------------------");
			message.AppendLine();
			message.AppendLine("You've received this message because you are currently signed up to");
			message.AppendLine("receive openbrew recipe comment notifications.  If you would like to ");
			message.AppendLine("stop reveiving these notifications, please click here");

			return message.ToString();
		}
	}
}
