using System;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using Openbrew.Web.Models;
using Openbrew.Web.Email;
using ctorx.Core.Data;
using ctorx.Core.Email;
using Openbrew.Web.Core.Data;
using Openbrew.Web.Core.Model;
using Openbrew.Web.Core.Service;

namespace Openbrew.Web.Controllers
{
	public class MarketingController : BrewgrController
	{
		readonly IUnitOfWorkFactory<BrewgrContext> UnitOfWorkFactory;
		readonly IMarketingService MarketingService;
		readonly IEmailSender EmailSender;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public MarketingController(IUnitOfWorkFactory<BrewgrContext> unitOfWorkFactory, IMarketingService marketingService, IEmailSender emailSender)
		{
			this.UnitOfWorkFactory = unitOfWorkFactory;
			this.MarketingService = marketingService;
			this.EmailSender = emailSender;
		}

		/// <summary>
		/// Executes the EmailSignUp view
		/// </summary>
		[HttpPost]
		[ForceHttps]
		public JsonResult EmailSignUp()
		{
			var emailAddress = Request["emailAddress"];

			if(string.IsNullOrWhiteSpace(emailAddress))
			{
				Response.StatusCode = 400;
				return Json(new { success = false, message = "Enter an email address to subscribe." });
			}
			emailAddress = emailAddress.Trim();
			try { new MailAddress(emailAddress); }
			catch (FormatException) { Response.StatusCode = 400; return Json(new { success = false, message = "Enter a valid email address." }); }

			var newsletterSignup = this.MarketingService.GetNewsletterSignupByEmailAddress(emailAddress);
			if (newsletterSignup != null && newsletterSignup.IsConfirmed && !newsletterSignup.IsUnsubscribed)
			{
				return Json(new { success = true, message = "That email is already subscribed." });
			}

			var keepPendingToken = newsletterSignup != null && !newsletterSignup.IsConfirmed && !newsletterSignup.IsUnsubscribed && !string.IsNullOrWhiteSpace(newsletterSignup.ConfirmationToken);
			var confirmationToken = keepPendingToken ? newsletterSignup.ConfirmationToken : Guid.NewGuid().ToString("N");
			if (newsletterSignup == null)
			{
				newsletterSignup = new NewsletterSignup
				{
					EmailAddress = emailAddress,
					Source = "Footer",
					IPAddress = Request.UserHostAddress,
					DateCreated = DateTime.Now,
					ConfirmationToken = confirmationToken
				};
			}
			else if (!keepPendingToken)
			{
				newsletterSignup.ConfirmationToken = confirmationToken;
				newsletterSignup.IsConfirmed = false;
				newsletterSignup.IsUnsubscribed = false;
				newsletterSignup.DateConfirmed = null;
				newsletterSignup.DateUnsubscribed = null;
			}

			using(var unitOfWork = this.UnitOfWorkFactory.NewUnitOfWork())
			{
				this.MarketingService.SaveNewsletterSignup(newsletterSignup);
				unitOfWork.Commit();
			}

			try
			{
				this.EmailSender.Send(new NewsletterConfirmationEmailMessage(this.WebSettings, newsletterSignup.EmailAddress, newsletterSignup.ConfirmationToken));
			}
			catch (Exception ex)
			{
				this.LogHandledException(ex);
				Response.StatusCode = 500;
				return Json(new { success = false, message = "We couldn't send the confirmation email. Please try again shortly." });
			}

			return Json(new { success = true, message = "Check your inbox to confirm your OpenBrew newsletter subscription." });
		}

		[HttpGet]
		public ActionResult NewsletterConfirm(string token)
		{
			NewsletterSignup signup;
			using (var unitOfWork = this.UnitOfWorkFactory.NewUnitOfWork())
			{
				signup = this.MarketingService.GetNewsletterSignupByToken(token);
				if (signup == null) return this.Issue404();
				if (!signup.IsConfirmed || signup.IsUnsubscribed)
				{
					signup.IsConfirmed = true;
					signup.IsUnsubscribed = false;
					signup.DateConfirmed = DateTime.Now;
					signup.DateUnsubscribed = null;
				}
				this.MarketingService.SaveNewsletterSignup(signup);
				unitOfWork.Commit();
			}

			ViewBag.NewsletterMessage = "You’re subscribed. Welcome to the OpenBrew newsletter.";
			return View("~/Views/Marketing/NewsletterStatus.cshtml");
		}

		[HttpGet]
		public ActionResult NewsletterUnsubscribe()
		{
			return View("~/Views/Marketing/NewsletterUnsubscribe.cshtml");
		}

		[HttpPost]
		public ActionResult NewsletterUnsubscribe(string emailAddress)
		{
			if (string.IsNullOrWhiteSpace(emailAddress))
			{
				ViewBag.NewsletterMessage = "Enter the email address you used to subscribe.";
				return View("~/Views/Marketing/NewsletterUnsubscribe.cshtml");
			}

			using (var unitOfWork = this.UnitOfWorkFactory.NewUnitOfWork())
			{
				var signup = this.MarketingService.GetNewsletterSignupByEmailAddress(emailAddress.Trim());
				if (signup != null)
				{
					signup.IsUnsubscribed = true;
					signup.DateUnsubscribed = DateTime.Now;
					this.MarketingService.SaveNewsletterSignup(signup);
				}
				unitOfWork.Commit();
			}
			ViewBag.NewsletterMessage = "If that address was subscribed, it has now been removed from future OpenBrew newsletter mail.";
			return View("~/Views/Marketing/NewsletterStatus.cshtml");
		}

		/// <summary>
		/// Executes the View for Feedback
		/// </summary>
		public ViewResult Feedback()
		{
			return View("~/Views/Marketing/Feedback.cshtml");
		}

		/// <summary>
		/// Executes the Http Post View for UserSuggestion
		/// </summary>
		[HttpPost]
		public ActionResult Feedback(FeedbackViewModel feedbackViewModel)
		{
			// Simple Validation (one property model)
			if (feedbackViewModel == null || string.IsNullOrWhiteSpace(feedbackViewModel.Feedback) || feedbackViewModel.Feedback.Length > 1000)
			{
				return this.Issue404();
			}

			// Only 3 submissions allowed in 5 minutes
			if (!this.MarketingService.UserCanSendFeedback())
			{
				return View("~/Views/Marketing/FeedbackReceived.cshtml", false);
			}

			using (var unitOfWork = this.UnitOfWorkFactory.NewUnitOfWork())
			{
				try
				{
					this.MarketingService.LogUserFeedback(feedbackViewModel.Feedback);
					unitOfWork.Commit();
				}
				catch (Exception ex)
				{
					this.LogHandledException(ex);
					unitOfWork.Rollback();
				}
			}

			return View("~/Views/Marketing/FeedbackReceived.cshtml", true);
		}

		/// <summary>
		/// Executes the View for FeedbackReceived
		/// </summary>
		public ViewResult FeedbackReceived()
		{
			return View("~/Views/Marketing/FeedbackReceived.cshtml");
		}
	}
}
