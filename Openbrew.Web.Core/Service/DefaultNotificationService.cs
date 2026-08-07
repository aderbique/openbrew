using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Openbrew.Web.Core.Data;
using Openbrew.Web.Core.Model;
using ctorx.Core.Linq;

namespace Openbrew.Web.Core.Service
{
	public delegate void NotifyDelegate(object data);

	public class DefaultNotificationService : INotificationService
	{
		readonly INotificationFactory NotificationFactory;
		readonly IBrewgrRepository Repository;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public DefaultNotificationService(INotificationFactory notificationFactory, IBrewgrRepository repository)
		{
			this.NotificationFactory = notificationFactory;
			this.Repository = repository;
		}

		/// <summary>
		/// Queues a Notification
		/// </summary>
		public void QueueNotification(NotificationType notificationType, object data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}

			var notification = this.NotificationFactory.Make(notificationType);

			var notifyDelegate = new NotifyDelegate(notification.Notify);
			notifyDelegate.BeginInvoke(data, null, null);
		}

		/// <summary>
		/// Gets a list of users who are to be notified for a recipe comment notification
		/// </summary>
		public IList<UserSummary> GetUsersForRecipeCommentNotification(int recipeId, int commentingUserId)
		{
			if (recipeId <= 0)
			{
				throw new ArgumentOutOfRangeException("recipeId");
			}

			if (commentingUserId <= 0)
			{
				throw new ArgumentOutOfRangeException("commentingUserId");
			}

			// Users already in Comment Thread
			var threadUserIds = this.Repository.GetSet<RecipeComment>()
				.Where(x => x.RecipeId == recipeId)
				.Where(x => x.Recipe.IsActive)
				.Where(x => x.Recipe.IsPublic)
				.Where(x => x.UserId != commentingUserId)
				.Where(x => x.User.IsActive)
				.Where(
					x =>
					x.User.UserNotificationTypes.Any(y => y.NotificationTypeId == (int) NotificationType.RecipeComment))
				.Select(x => x.UserId)
				.ToList();

			// Recipe Creator
			var creatorUserId = this.Repository.GetSet<Recipe>()
			    .Where(x => x.RecipeId == recipeId)
			    .Where(x => x.IsActive)
			    .Where(x => x.IsPublic)
			    .Where(x => x.CreatedBy != commentingUserId)
			    .Where(x => x.User.IsActive)
			    .Where(
				    x =>
				    x.User.UserNotificationTypes.Any(
					    y => y.NotificationTypeId == (int) NotificationType.RecipeComment))
			    .Select(x => x.CreatedBy);

			return this.Repository.GetSet<UserSummary>().WhereIn(x => x.UserId, threadUserIds.Union(creatorUserId)).ToList();
		}

        /// <summary>
        /// Gets a list of users who are to be notified for a recipe brew comment notification
        /// </summary>
        public IList<UserSummary> GetUsersForBrewSessionCommentNotification(int brewSessionId, int commentingUserId)
        {
            if (brewSessionId <= 0)
            {
                throw new ArgumentOutOfRangeException("recipeId");
            }

            if (commentingUserId <= 0)
            {
                throw new ArgumentOutOfRangeException("commentingUserId");
            }

            // Users already in Comment Thread
            var threadUserIds = this.Repository.GetSet<BrewSessionComment>()
                .Where(x => x.BrewSessionId == brewSessionId)
                .Where(x => x.IsActive)
                .Where(x => x.UserId != commentingUserId)
                .Where(x => x.User.IsActive)
                .Where(
                    x =>
                    x.User.UserNotificationTypes.Any(y => y.NotificationTypeId == (int)NotificationType.BrewSessionComment))
                .Select(x => x.UserId)
                .ToList();

            // Recipe Creator
            var creatorUserId = this.Repository.GetSet<BrewSession>()
                .Where(x => x.BrewSessionId == brewSessionId)
                .Where(x => x.IsActive)
                .Where(x => x.IsPublic)
                .Where(x => x.UserId != commentingUserId)
                .Where(x => x.BrewedByUser.IsActive)
                .Where(
                    x =>
                    x.BrewedByUser.UserNotificationTypes.Any(
                        y => y.NotificationTypeId == (int)NotificationType.BrewSessionComment))
                .Select(x => x.UserId);

            return this.Repository.GetSet<UserSummary>().WhereIn(x => x.UserId, threadUserIds.Union(creatorUserId)).ToList();
        }

		/// <summary>
		/// Determines if a user is subscribed to a notification type
		/// </summary>
		public bool UserSubscribedTo(int userId, NotificationType notificationType)
		{
			return this.Repository.GetSet<UserNotificationType>()
			    .Any(x => x.UserId == userId && x.NotificationTypeId == (int)notificationType);

		}
	}
}