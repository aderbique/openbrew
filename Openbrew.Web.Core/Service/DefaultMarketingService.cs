using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Openbrew.Web.Core.Data;
using Openbrew.Web.Core.Model;
using ctorx.Core.Identity;

namespace Openbrew.Web.Core.Service
{
	public class DefaultMarketingService : IMarketingService
	{
		readonly IBrewgrRepository Repository;
		readonly IUserResolver UserResolver;
		readonly IUserHostAddressResolver UserHostAddressResolver;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public DefaultMarketingService(IBrewgrRepository repository, IUserResolver userResolver, IUserHostAddressResolver userHostAddressResolver)
		{
			this.Repository = repository;
			this.UserResolver = userResolver;
			this.UserHostAddressResolver = userHostAddressResolver;
		}

		/// <summary>
		/// Adds a Newsletter Signup
		/// </summary>
		public NewsletterSignup SaveNewsletterSignup(NewsletterSignup newsletterSignup)
		{
			if(newsletterSignup == null)
			{
				throw new ArgumentNullException("newsletterSignup");
			}

			// Ignore the Request if Email Already Exists
			var existing = this.Repository.GetSet<NewsletterSignup>()
				.FirstOrDefault(x => x.EmailAddress.ToLower() == newsletterSignup.EmailAddress.ToLower());
			if(existing != null)
			{
				// This method is used for both first-time subscriptions and later
				// confirmation/preference changes.  The old implementation returned
				// the existing row without copying those changes, so a confirmation
				// page could say "subscribed" while the database remained pending.
				existing.ConfirmationToken = newsletterSignup.ConfirmationToken;
				existing.IsConfirmed = newsletterSignup.IsConfirmed;
				existing.IsUnsubscribed = newsletterSignup.IsUnsubscribed;
				existing.DateConfirmed = newsletterSignup.DateConfirmed;
				existing.DateUnsubscribed = newsletterSignup.DateUnsubscribed;
				return existing;
			}

			this.Repository.Add(newsletterSignup);
			return newsletterSignup;
		}

		public NewsletterSignup GetNewsletterSignupByToken(string confirmationToken)
		{
			if (string.IsNullOrWhiteSpace(confirmationToken)) return null;
			return this.Repository.GetSet<NewsletterSignup>().FirstOrDefault(x => x.ConfirmationToken == confirmationToken);
		}

		public NewsletterSignup GetNewsletterSignupByEmailAddress(string emailAddress)
		{
			if (string.IsNullOrWhiteSpace(emailAddress)) return null;
			return this.Repository.GetSet<NewsletterSignup>().FirstOrDefault(x => x.EmailAddress.ToLower() == emailAddress.ToLower());
		}

		public NewsletterSignup GetNewsletterSignupById(int newsletterSignupId)
		{
			return newsletterSignupId > 0 ? this.Repository.GetSet<NewsletterSignup>().FirstOrDefault(x => x.NewsletterSignupId == newsletterSignupId) : null;
		}

		public IList<NewsletterSignup> GetNewsletterSignups()
		{
			return this.Repository.GetSet<NewsletterSignup>()
				.OrderBy(x => x.IsUnsubscribed)
				.ThenBy(x => x.IsConfirmed ? 0 : 1)
				.ThenBy(x => x.EmailAddress)
				.ToList();
		}

		public void DeleteNewsletterSignup(NewsletterSignup newsletterSignup)
		{
			if (newsletterSignup == null) throw new ArgumentNullException("newsletterSignup");
			this.Repository.Delete(newsletterSignup);
		}

		/// <summary>
		/// Logs user feedback
		/// </summary>
		public void LogUserFeedback(string feedback)
		{
			var currentUser = this.UserResolver.Resolve();

			var userFeedback = new UserFeedback
			{
				UserId = currentUser.UserId > 0 ? (int?)currentUser.UserId : null,
				Feedback = feedback,
				UserHostAddress = this.UserHostAddressResolver.Resolve(),
				DateCreated = DateTime.Now
			};

			this.Repository.Add(userFeedback);
		}

		/// <summary>
		/// Gets a list of recent user feedback
		/// </summary>
		public IList<UserFeedback> GetFeedback()
		{
			return this.Repository.GetSet<UserFeedback>()
				.Include(x => x.User)
				.Where(x => x.DateResponded == null && x.RespondedBy == null)
				.OrderByDescending(x => x.DateCreated)
				.ToList();
		}

		/// <summary>
		/// Determines if a user can send feedback
		/// </summary>
		public bool UserCanSendFeedback()
		{
			var startDate = DateTime.Now.AddMinutes(-5);
			var ipaddress = this.UserHostAddressResolver.Resolve();

			var count = this.Repository.GetSet<UserFeedback>()
				.Where(x => x.UserHostAddress == ipaddress)
				.Where(x => x.DateCreated >= startDate)
				.Count();

			return count < 3;
		}
	}
}
