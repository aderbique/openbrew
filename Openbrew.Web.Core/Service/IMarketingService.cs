using System;
using System.Collections.Generic;
using Openbrew.Web.Core.Model;

namespace Openbrew.Web.Core.Service
{
	public interface IMarketingService
	{
		/// <summary>
		/// Adds a Newsletter Signup
		/// </summary>
		NewsletterSignup SaveNewsletterSignup(NewsletterSignup newsletterSignup);

		NewsletterSignup GetNewsletterSignupByToken(string confirmationToken);

		NewsletterSignup GetNewsletterSignupByEmailAddress(string emailAddress);

		NewsletterSignup GetNewsletterSignupById(int newsletterSignupId);

		IList<NewsletterSignup> GetNewsletterSignups();

		void DeleteNewsletterSignup(NewsletterSignup newsletterSignup);

		/// <summary>
		/// Logs user feedback
		/// </summary>
		void LogUserFeedback(string suggestion);

		/// <summary>
		/// Gets a list of feedback that hasn't been responded to
		/// </summary>
		IList<UserFeedback> GetFeedback();

		/// <summary>
		/// Determines if a user can send feedback
		/// </summary>
		bool UserCanSendFeedback();
	}
}
