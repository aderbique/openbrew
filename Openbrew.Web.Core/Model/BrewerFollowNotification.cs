using System;
using Openbrew.Web.Core.Configuration;
using Openbrew.Web.Core.Service;
using ctorx.Core.Email;

namespace Openbrew.Web.Core.Model
{
	public class BrewerFollowNotification : INotification
	{
		readonly IWebSettings WebSettings;
		readonly IUserService UserService;
		readonly IEmailSender EmailSender;
		readonly INotificationService NotificationService;
		readonly BrewgrUrlBuilder BrewgrUrlBuilder;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public BrewerFollowNotification(IWebSettings webSettings, IUserService userService, 
			IEmailSender emailSender, INotificationService notificationService, BrewgrUrlBuilder brewgrUrlBuilder)
		{
			this.WebSettings = webSettings;
			this.UserService = userService;
			this.EmailSender = emailSender;
			this.NotificationService = notificationService;
			this.BrewgrUrlBuilder = brewgrUrlBuilder;
		}

		/// <summary>
		/// Performs the notification
		/// </summary>
		public void Notify(object data)
		{
			// 2 count array (position 0 = user being followed, 1 = follower)
			var userIds = data as int[];

			// Check if User is Subscribed to Follow Notifications
			if (!this.NotificationService.UserSubscribedTo(userIds[0], NotificationType.BrewerFollowed))
			{
				return;
			}

			var userSummary = this.UserService.GetUserSummaryById(userIds[0]);
			var followerSummary = this.UserService.GetUserSummaryById(userIds[1]);

			var emailMessage = new BrewerFollowEmailMessage(this.WebSettings, followerSummary.Username, userSummary, this.BrewgrUrlBuilder);
			this.EmailSender.Send(emailMessage);
		}
	}
}