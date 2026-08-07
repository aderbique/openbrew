using System;
using System.Linq;
using System.Text;
using System.Web;
using Openbrew.Web.Core.Configuration;
using ctorx.Core.Email;
using ctorx.Core.Formatting;

namespace Openbrew.Web.Core.Model
{
	public class RecipeCommentEmailMessage : BrewgrEmailMessage
	{
		readonly IWebSettings WebSettings;
		readonly RecipeSummary RecipeSummary;
		readonly string CommenterUsername;
		readonly UserSummary UserToNotify;
		readonly RecipeComment RecipeComment;
		readonly BrewgrUrlBuilder BrewgrUrlBuilder;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public RecipeCommentEmailMessage(IWebSettings webSettings, RecipeSummary recipeSummary, string commenterUsername, 
			UserSummary userToNotify, RecipeComment recipeComment, BrewgrUrlBuilder brewgrUrlBuilder) : base(webSettings)
		{
			this.WebSettings = webSettings;
			this.RecipeSummary = recipeSummary;
			this.CommenterUsername = commenterUsername;
			this.UserToNotify = userToNotify;
			this.RecipeComment = recipeComment;
			this.BrewgrUrlBuilder = brewgrUrlBuilder;

			// Set Sender
			this.SenderAddress = webSettings.SenderAddress;
			this.SenderDisplayName = webSettings.SenderDisplayName;

			// Set Recipient
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

			message.AppendLine("Hello " + this.UserToNotify.FirstName+ ",");
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

			message.AppendLine("\"" + HttpUtility.HtmlDecode(this.RecipeComment.CommentText.Replace(Environment.NewLine, "")) + "\"");
			message.AppendLine();

			message.AppendLine("You can respond to " + this.CommenterUsername + " by visiting the recipe page at the following link:");
			message.AppendLine();
			message.AppendLine("<" + this.BrewgrUrlBuilder.BuildDetailUrl(this.RecipeSummary) + "#comments>");
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