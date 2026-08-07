using System;
using System.Collections.Generic;
using Openbrew.Web.Core.Configuration;
using Openbrew.Web.Core.Data;
using Openbrew.Web.Core.Service;
using Ninject;
using Ninject.Parameters;
using ctorx.Core.Collections;
using ctorx.Core.Email;
using ctorx.Core.Ninject;

namespace Openbrew.Web.Core.Model
{
	public class BrewSessionCommentNotification : INotification
	{
		readonly INotificationService NotificationService;
		readonly IRecipeService RecipeService;
		readonly IUserService UserService;
		readonly IEmailSender EmailSender;
		readonly IWebSettings WebSettings;
		readonly BrewgrUrlBuilder RecipeUrlBuilder;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
        public BrewSessionCommentNotification(INotificationService notificationService, IRecipeService recipeService, IUserService userService, IEmailSender emailSender, 
			IWebSettings webSettings, BrewgrUrlBuilder recipeUrlBuilder)
		{
			this.NotificationService = notificationService;
			this.RecipeService = recipeService;
			this.UserService = userService;
			this.EmailSender = emailSender;
			this.WebSettings = webSettings;
			this.RecipeUrlBuilder = recipeUrlBuilder;
		}

		/// <summary>
		/// Performs the Notification
		/// </summary>
		public void Notify(object data)
		{
			var brewSessionComment = data as BrewSessionComment;

			var brewSession = this.RecipeService.GetBrewSessionById(brewSessionComment.BrewSessionId);
			var commenterUsername = this.UserService.GetUserSummaryById(brewSessionComment.UserId).Username;

			// Get users to notify
            var usersToNotify = this.NotificationService.GetUsersForBrewSessionCommentNotification(brewSessionComment.BrewSessionId, brewSessionComment.UserId);

			// Notify the Lot
			usersToNotify.ForEach(x =>
			{
                var emailMessage = new BrewSessionCommentEmailMessage(this.WebSettings, brewSession, commenterUsername, x, brewSessionComment, this.RecipeUrlBuilder);
				this.EmailSender.Send(emailMessage);
			});
		}
	}
}
