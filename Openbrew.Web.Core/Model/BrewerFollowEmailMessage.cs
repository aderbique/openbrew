using System;
using System.Text;
using Openbrew.Web.Core.Configuration;

namespace Openbrew.Web.Core.Model
{
	public class BrewerFollowEmailMessage : BrewgrEmailMessage
	{
		readonly IWebSettings WebSettings;
		readonly string FollowingUsername;
		readonly UserSummary UserToNotify;
		readonly BrewgrUrlBuilder BrewgrUrlBuilder;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public BrewerFollowEmailMessage(IWebSettings webSettings, string followingUsername, UserSummary userToNotify, BrewgrUrlBuilder brewgrUrlBuilder) : base(webSettings)
		{
			this.WebSettings = webSettings;
			this.FollowingUsername = followingUsername;
			this.UserToNotify = userToNotify;
			this.BrewgrUrlBuilder = brewgrUrlBuilder;

			// Set Sender
			this.SenderAddress = webSettings.SenderAddress;
			this.SenderDisplayName = webSettings.SenderDisplayName;

			// Set Recipient
			this.ToRecipients.Add(userToNotify.EmailAddress);

			// Build Subject
			this.Subject = followingUsername + " is now following you!";
		}

		/// <summary>
		/// Builds the message body
		/// </summary>
		public override string BuildMessageBody()
		{
			var message = new StringBuilder();

			message.AppendLine("Hello " + this.UserToNotify.FirstName + ",");
			message.AppendLine();

			message.AppendLine(this.FollowingUsername + " is now following you on Brewgr!");
			message.AppendLine();

			message.AppendLine("You can view " + this.FollowingUsername + (FollowingUsername.EndsWith("s") ? "'" : "'s") + " Brewgr profile at the following link:");
			message.AppendLine();
			message.AppendLine("<" + this.BrewgrUrlBuilder.BuildUserProfileUrl(this.FollowingUsername) + ">");
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