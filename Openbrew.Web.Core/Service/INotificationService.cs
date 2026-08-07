using System;
using System.Collections.Generic;
using Openbrew.Web.Core.Model;

namespace Openbrew.Web.Core.Service
{
	public interface INotificationService
	{
		/// <summary>
		/// Queues a Notification
		/// </summary>
		void QueueNotification(NotificationType notificationType, object data);

		/// <summary>
		/// Gets a list of users who are to be notified for a recipe comment notification
		/// </summary>
		IList<UserSummary> GetUsersForRecipeCommentNotification(int recipeId, int commentingUserId);

        /// <summary>
		/// Gets a list of users who are to be notified for a recipe comment notification
		/// </summary>
        IList<UserSummary> GetUsersForBrewSessionCommentNotification(int brewSessionId, int commentingUserId);

		/// <summary>
		/// Determines if a user is subscribed to a notification type
		/// </summary>
		bool UserSubscribedTo(int userId, NotificationType notificationType);
	}
}