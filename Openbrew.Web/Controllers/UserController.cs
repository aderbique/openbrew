using System;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using ctorx.Core.Data;
using ctorx.Core.Messaging;
using Openbrew.Web.Core.Data;
using Openbrew.Web.Core.Model;
using Openbrew.Web.Core.Service;
using Openbrew.Web.Mappers;
using Openbrew.Web.Models;
using Openbrew.Web.Email;
using ctorx.Core.Email;

namespace Openbrew.Web.Controllers
{
	public class UserController : BrewgrController
	{
		readonly IUnitOfWorkFactory<BrewgrContext> UnitOfWorkFactory;
		readonly IUserService UserService;
		readonly IBrewgrRepository Repository;
		readonly IUserResolver UserResolver;
		readonly IRecipeService RecipeService;
		readonly INotificationService NotificationService;
		readonly IMarketingService MarketingService;
		readonly IEmailSender EmailSender;
		readonly IUserLoginService UserLoginService;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public UserController(IUnitOfWorkFactory<BrewgrContext> unitOfWorkFactory, IUserService userService, IBrewgrRepository repository, IUserResolver userResolver,
			IRecipeService recipeService, INotificationService notificationService, IMarketingService marketingService, IEmailSender emailSender, IUserLoginService userLoginService)
		{
			this.UnitOfWorkFactory = unitOfWorkFactory;
			this.UserService = userService;
			this.Repository = repository;
			this.UserResolver = userResolver;
            this.RecipeService = recipeService;
			this.NotificationService = notificationService;
			this.MarketingService = marketingService;
			this.EmailSender = emailSender;
			this.UserLoginService = userLoginService;
		}

		/// <summary>
		/// Executes the Settings View
		/// </summary>
		[ForceHttps]
		[Authorize]
		public ViewResult Settings()
		{
			var user = this.UserService.GetUserById(this.ActiveUser.UserId);
			ViewBag.NewsletterSignup = this.MarketingService.GetNewsletterSignupByEmailAddress(user.EmailAddress);
			var model = Mapper.Map(user, new UserSettingsViewModel());
			model.HasGoogleConnection = this.UserHasGoogleConnection(user.UserId);
			return View("AccountSettings", model);
		}

		bool UserHasGoogleConnection(int userId)
		{
			return this.Repository.GetSet<UserOAuthUserId>()
				.Any(x => x.UserId == userId && x.OAuthProviderId == (int)OAuthProvider.Google);
		}

		[HttpPost]
		[ForceHttps]
		[Authorize]
		public JsonResult UnlinkGoogle(string newPassword, string confirmPassword)
		{
			if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 8)
			{
				Response.StatusCode = 400;
				return Json(new { Success = false, Message = "Choose a password with at least 8 characters before unlinking Google." });
			}
			if (newPassword != confirmPassword)
			{
				Response.StatusCode = 400;
				return Json(new { Success = false, Message = "The passwords do not match." });
			}

			using (var unitOfWork = this.UnitOfWorkFactory.NewUnitOfWork())
			{
				var googleLink = this.Repository.GetSet<UserOAuthUserId>()
					.FirstOrDefault(x => x.UserId == this.ActiveUser.UserId && x.OAuthProviderId == (int)OAuthProvider.Google);
				if (googleLink == null)
				{
					Response.StatusCode = 400;
					return Json(new { Success = false, Message = "This account is not connected to Google." });
				}

				// Set the replacement credential first. The OAuth link is deleted only
				// after a usable local sign-in method exists.
				this.UserLoginService.SetUserPassword(this.ActiveUser.UserId, newPassword);
				this.Repository.Delete(googleLink);
				unitOfWork.Commit();
			}

			return Json(new { Success = true, Message = "Google has been disconnected. You can now sign in with your email and new password." });
		}

		[HttpPost]
		[ForceHttps]
		[Authorize]
		public JsonResult SetNewsletterPreference(bool subscribe)
		{
			var user = this.UserService.GetUserById(this.ActiveUser.UserId);
			var signup = this.MarketingService.GetNewsletterSignupByEmailAddress(user.EmailAddress);
			if (!subscribe)
			{
				if (signup != null)
				{
					using (var unitOfWork = this.UnitOfWorkFactory.NewUnitOfWork())
					{
						signup.IsUnsubscribed = true;
						signup.DateUnsubscribed = DateTime.Now;
						this.MarketingService.SaveNewsletterSignup(signup);
						unitOfWork.Commit();
					}
				}
				return Json(new { Success = true, Message = "You will no longer receive the OpenBrew newsletter." });
			}

			if (signup != null && signup.IsConfirmed && !signup.IsUnsubscribed)
			{
				return Json(new { Success = true, Message = "You are already subscribed to the newsletter." });
			}

			var keepPendingToken = signup != null && !signup.IsConfirmed && !signup.IsUnsubscribed && !string.IsNullOrWhiteSpace(signup.ConfirmationToken);
			var token = keepPendingToken ? signup.ConfirmationToken : Guid.NewGuid().ToString("N");
			if (signup == null)
			{
				signup = new NewsletterSignup { EmailAddress = user.EmailAddress, Source = "AccountSettings", IPAddress = Request.UserHostAddress, DateCreated = DateTime.Now, ConfirmationToken = token };
			}
			else if (!keepPendingToken)
			{
				signup.ConfirmationToken = token;
				signup.IsConfirmed = false;
				signup.IsUnsubscribed = false;
				signup.DateConfirmed = null;
				signup.DateUnsubscribed = null;
			}

			using (var unitOfWork = this.UnitOfWorkFactory.NewUnitOfWork())
			{
				this.MarketingService.SaveNewsletterSignup(signup);
				unitOfWork.Commit();
			}

			try
			{
				this.EmailSender.Send(new NewsletterConfirmationEmailMessage(this.WebSettings, signup.EmailAddress, signup.ConfirmationToken));
				return Json(new { Success = true, Message = "Check your inbox to confirm your newsletter subscription." });
			}
			catch (Exception ex)
			{
				this.LogHandledException(ex);
				Response.StatusCode = 500;
				return Json(new { Success = false, Message = "We couldn't send the confirmation email. Please try again shortly." });
			}
		}

		/// <summary>
		/// Executes the Http Post View for Settings
		/// </summary>
		[HttpPost]
		[ForceHttps]
		[Authorize]
		public ActionResult Settings(UserSettingsViewModel userSettingsViewModel)
		{
			if(!this.ValidateAndAppendMessages(userSettingsViewModel))
			{
				return View("AccountSettings", userSettingsViewModel);
			}

			var currentUser = this.UserService.GetUserById(this.ActiveUser.UserId);
			var hasGoogleConnection = this.UserHasGoogleConnection(currentUser.UserId);
			if (hasGoogleConnection && !string.Equals(currentUser.EmailAddress, userSettingsViewModel.EmailAddress == null ? null : userSettingsViewModel.EmailAddress.Trim(), StringComparison.OrdinalIgnoreCase))
			{
				return Json(new { Success = false, Message = "Your email is managed by the connected Google account and cannot be changed here." });
			}

			// Check Email Address
			if (this.UserService.EmailAddressIsInUse(userSettingsViewModel.EmailAddress, this.ActiveUser.UserId))
			{
			    return Json(new { Success = false, Message = "The email address you entered is already in use." });
			}

			// Check Username Uniqueness
			if (this.UserService.UsernameIsInUse(this.ActiveUser.UserId, userSettingsViewModel.Username))
			{
                return Json(new { Success = false, Message = "The requested username is already in use" });
			}

			using(var unitOfWork = this.UnitOfWorkFactory.NewUnitOfWork())
			{
				try
				{
					var user = this.UserService.GetUserById(this.ActiveUser.UserId);

					var originalUsername = user.Username;

					user = Mapper.Map(userSettingsViewModel, user);

					// Cust Username Flag
					if (!user.HasCustomUsername && originalUsername != user.Username)
					{
						user.HasCustomUsername = true;
					}

					// Keep OAuth-linked accounts bound to the email asserted by Google.
					var userHasGoogleConnection = this.UserHasGoogleConnection(user.UserId);
					user.EmailAddress = userHasGoogleConnection ? user.EmailAddress : userSettingsViewModel.EmailAddress.Trim();
					user.DateModified = DateTime.Now;

					unitOfWork.Commit();

					this.UserResolver.Update(Mapper.Map(user, new UserSummary()));

                    return Json(new { Success = true, Message = "Your settings have been saved" });
                }
				catch (Exception ex)
				{
					unitOfWork.Rollback();
					this.LogHandledException(ex);
					return this.Issue500();
				}
			}
		}

		/// <summary>
		/// Executes the Http Post View for SetNotifications
		/// </summary>
		[HttpPost]
		public ActionResult SetNotifications(UserSettingsViewModel userSettingsViewModel)
		{
			if (userSettingsViewModel.UserId != this.ActiveUser.UserId)
			{
				return this.Issue404();
			}

			using (var unitOfWork = this.UnitOfWorkFactory.NewUnitOfWork())
			{
				try
				{
					var user = this.UserService.GetUserById(userSettingsViewModel.UserId);

					#region Sub/Un-SUB Types

					// RecipeComments: Handle Unsubscribe
					if (!userSettingsViewModel.RecipeCommentNotifications &&
					    user.UserNotificationTypes.Any(x => x.NotificationTypeId == (int) NotificationType.RecipeComment))
					{
						this.UserService.UnsubscribeUserFromNotificationType(user, NotificationType.RecipeComment);
					}

					// RecipeComments: Handle Subscribe
					if (userSettingsViewModel.RecipeCommentNotifications &&
					    !user.UserNotificationTypes.Any(x => x.NotificationTypeId == (int) NotificationType.RecipeComment))
					{
						this.UserService.SubscribeUserToNotificationType(user, NotificationType.RecipeComment);
					}

					// BrewSessionComments: Handle Unsubscribe
					if (!userSettingsViewModel.BrewSessionCommentNotifications &&
						user.UserNotificationTypes.Any(x => x.NotificationTypeId == (int)NotificationType.BrewSessionComment))
					{
						this.UserService.UnsubscribeUserFromNotificationType(user, NotificationType.BrewSessionComment);
					}

					// BrewSessionComments: Handle Subscribe
					if (userSettingsViewModel.BrewSessionCommentNotifications &&
						!user.UserNotificationTypes.Any(x => x.NotificationTypeId == (int)NotificationType.BrewSessionComment))
					{
						this.UserService.SubscribeUserToNotificationType(user, NotificationType.BrewSessionComment);
					}

					// BrewerFollow: Handle Unsubscribe
					if (!userSettingsViewModel.BrewerFollowNotifications &&
						user.UserNotificationTypes.Any(x => x.NotificationTypeId == (int)NotificationType.BrewerFollowed))
					{
						this.UserService.UnsubscribeUserFromNotificationType(user, NotificationType.BrewerFollowed);
					}

					// BrewerFollow: Handle Subscribe
					if (userSettingsViewModel.BrewerFollowNotifications &&
						!user.UserNotificationTypes.Any(x => x.NotificationTypeId == (int)NotificationType.BrewerFollowed))
					{
						this.UserService.SubscribeUserToNotificationType(user, NotificationType.BrewerFollowed);
					}

					// SiteFeatures: Handle Unsubscribe
					if (!userSettingsViewModel.SiteFeatureNotifications &&
						user.UserNotificationTypes.Any(x => x.NotificationTypeId == (int)NotificationType.SiteFeatures))
					{
						this.UserService.UnsubscribeUserFromNotificationType(user, NotificationType.SiteFeatures);
					}

					// SiteFeatures: Handle Subscribe
					if (userSettingsViewModel.SiteFeatureNotifications &&
						!user.UserNotificationTypes.Any(x => x.NotificationTypeId == (int)NotificationType.SiteFeatures))
					{
						this.UserService.SubscribeUserToNotificationType(user, NotificationType.SiteFeatures);
					}

					// SiteOutages: Handle Unsubscribe
					if (!userSettingsViewModel.SiteOutageNotifications &&
						user.UserNotificationTypes.Any(x => x.NotificationTypeId == (int)NotificationType.SiteOutages))
					{
						this.UserService.UnsubscribeUserFromNotificationType(user, NotificationType.SiteOutages);
					}

					// SiteOutages: Handle Subscribe
					if (userSettingsViewModel.SiteOutageNotifications &&
						!user.UserNotificationTypes.Any(x => x.NotificationTypeId == (int)NotificationType.SiteOutages))
					{
						this.UserService.SubscribeUserToNotificationType(user, NotificationType.SiteOutages);
					}

					#endregion

					unitOfWork.Commit();

				    return Json(new { Success = true, Message = "Your notification settings have been saved" });
				}
				catch (Exception ex)
				{
					unitOfWork.Rollback();
					this.LogHandledException(ex);
					return this.Issue500();
				}
			}
		}

		/// <summary>
		/// Executes the UsernameExists view
		/// </summary>
		[Authorize]
		public ContentResult UsernameExists(string username)
		{
			if(string.IsNullOrWhiteSpace(username))
			{
				throw new ArgumentNullException("username");
			}

			return Content(this.UserService.UsernameIsInUse(this.ActiveUser.UserId, username) ? "1" : "0");
		}

		/// <summary>
		/// Executes the View for EmailAddressInUse
		/// </summary>
		[Authorize]
		public ContentResult EmailAddressInUse()
		{
			var emailAddress = Request["email"];

			if (string.IsNullOrWhiteSpace(emailAddress))
			{
				throw new ArgumentNullException("emailAddress");
			}

			return Content(this.UserService.EmailAddressIsInUse(Server.UrlDecode(emailAddress), this.ActiveUser.UserId) ? "1" : "0");
		}

		/// <summary>
		/// Executes the View for UserProfile
		/// </summary>
		public ActionResult UserProfile(string userName)
		{
            var user = this.UserService.GetUserByUserName(userName);

			if(user == null)
			{
				return this.Issue404();
			}

			var userProfileViewModel = new UserProfileViewModel();
			userProfileViewModel.UserSummary = Mapper.Map(user, new UserSummary());

			// Check if active user follows this user
			if (this.ActiveUser != null)
			{
				ViewBag.UserIsFollowed = this.UserService.UserIsFollowedBy(user.UserId, this.ActiveUser.UserId);
			}

			// Recipes
            userProfileViewModel.Recipes = this.RecipeService.GetUserRecipes(user.UserId)
				.Where(x => x.IsPublic)
				.ToList();

			// Brew Summaries
			userProfileViewModel.BrewSessionSummaries = this.RecipeService.GetUserBrewSessions(user.UserId)
				.OrderByDescending(x => x.BrewDate)
				.ToList();

			// Followers
			userProfileViewModel.Followers = this.UserService.GetFollowersOf(user.UserId);

			// Followed
			userProfileViewModel.Follows = this.UserService.GetFollowedBy(user.UserId);

			return View(userProfileViewModel);
		}

		/// <summary>
		/// Executes the View for ToggleView
		/// </summary>
		[HttpPost]
		public ActionResult ToggleBrewerFollow(int userId)
		{
			using (var unitOfWork = this.UnitOfWorkFactory.NewUnitOfWork())
			{
				try
				{
					this.UserService.ToggleUserFollow(userId, this.ActiveUser.UserId);
					unitOfWork.Commit();
					return new EmptyResult();
				}
				catch (Exception ex)
				{
					this.LogHandledException(ex);
					unitOfWork.Rollback();
					return this.Issue500();
				}
			}
		}

		/// <summary>
		/// Gets the reputation score for a user
		/// </summary>
		public int UserRep(int userId)
		{
			return this.UserService.GetUserReputationScore(userId);
		}

	}
}
