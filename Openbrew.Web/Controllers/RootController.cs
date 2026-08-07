using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security;
using System.Web.Mvc;
using Openbrew.Web.Email;
using ctorx.Core.Crypto;
using ctorx.Core.Data;
using ctorx.Core.Email;
using ctorx.Core.Messaging;
using ctorx.Core.Security;
using Openbrew.Web.Core.Configuration;
using Openbrew.Web.Core.Data;
using Openbrew.Web.Core.Model;
using Openbrew.Web.Core.Service;
using Openbrew.Web.Models;
using ctorx.Core.Web;
using ctorx.Core.Ninject;
using AutoMapper;

namespace Openbrew.Web.Controllers
{
	public class RootController : BrewgrController
	{
		static T Resolve<T>() where T : class
		{
			try
			{
				return KernelPersister.Get().GetService(typeof(T)) as T;
			}
			catch
			{
				return null;
			}
		}

		#region LOGIN CHECK

		/// <summary>
		/// Executes the User Is Logged In View
		/// </summary>
		public ContentResult UserIsLoggedIn()
		{
			return this.Content(this.ActiveUser != null ? "1" : "0");
		}

		#endregion

		/// <summary>
		/// Executes the View for Index
		/// </summary>
		public ActionResult Index()
		{
            if (this.ActiveUser != null)
            {
                return RedirectToAction("Dashboard", "Dashboard");
            }

			var newRecipes = new List<RecipeSummary>();
			var popularRecipes = new List<RecipeSummary>();
			var topContrinutors = new List<UserSummary>();

			try
			{
				var recipeService = Resolve<IRecipeService>();
				if (recipeService != null)
				{
					newRecipes = recipeService.GetNewestRecipes(4).ToList();
					popularRecipes = recipeService.GetPopularRecipes(4).ToList();
				}

				var userService = Resolve<IUserService>();
				if (userService != null)
				{
					topContrinutors = userService.GetWeeklyTopContributors(4).ToList();
				}
			}
			catch
			{
				// Fall back to the static hero/content shell when the DB is unavailable.
			}

			return View("OpenBrewIndex", new HomePageViewModel { NewRecipes = newRecipes, TopContributors = topContrinutors, PopularRecipes = popularRecipes});
		}

		/// <summary>
		/// Executes the View for Featires
		/// </summary>
		public ActionResult Features()
		{
			return View("OpenBrewFeatures");
		}

		/// <summary>
		/// Executes the View for About
		/// </summary>
		public ActionResult About()
		{
			return View("OpenBrewAbout");
		}

		/// <summary>
		/// Explains how a style guideline can guide recipe design without
		/// turning a brewer's own preferences into a pass/fail rule.
		/// </summary>
		[Route("brewing-to-style")]
		public ActionResult BrewingToStyle()
		{
			return View("OpenBrewBrewingToStyle");
		}

		/// <summary>
		/// Executes the View for Terms
		/// </summary>
		public ActionResult Terms()
		{
			return View("OpenBrewTerms");
		}

		/// <summary>
		/// Executes the View for Privacy
		/// </summary>
		public ActionResult Privacy()
		{
			return View("OpenBrewPrivacy");
		}

		/// <summary>
		/// Executes the View for Faq
		/// </summary>
		public ActionResult Faq()
		{
			return View("OpenBrewFaq");
		}

		/// <summary>
		/// Explains how the free service is supported and offers an optional donation path.
		/// </summary>
		public ActionResult Support()
		{
			return View("OpenBrewSupport");
		}

		/// <summary>
		/// Executes the View for RecentPhotos
		/// </summary>
		public JsonResult RecentPhotos()
		{
			var recipeService = Resolve<IRecipeService>();
			var recentRecipes = recipeService == null
				? new List<RecipeSummary>()
				: recipeService.GetRecentRecipesCached(3);

			return Json(recentRecipes.Select(x => new
			{
				ImageUrl = Url.RecipeThumbnailUrl(x.ImageUrlRoot, x.Srm),
				Url = Url.RecipeDetailUrl(x.RecipeId, x.RecipeName, x.BJCPStyleName),
				Name = x.RecipeName
			}), JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Executes the View for Contact
		/// </summary>
		public ActionResult Contact()
		{
			return View("OpenBrewContact", this.ActiveUser == null
				? new ContactViewModel()
				: new ContactViewModel { Name = this.ActiveUser.FullName, EmailAddress = this.ActiveUser.EmailAddress });
		}

		/// <summary>
		/// Executes the Http Post View for Contact
		/// </summary>
		[HttpPost]
		public ActionResult Contact(ContactViewModel contactViewModel)
		{
			if(!this.ValidateAndAppendMessages(contactViewModel))
			{
				return RedirectToAction("Contact");
			}

			var emailMessageFactory = Resolve<IEmailMessageFactory>();
			var emailSender = Resolve<IEmailSender>();
			if (emailMessageFactory != null && emailSender != null)
			{
				var contactMessage = (ContactFormEmailMessage)emailMessageFactory.Make(EmailMessageType.ContactForm);
				contactMessage.SetContactViewModel(contactViewModel);
				emailSender.Send(contactMessage);
			}

			this.ForwardMessage(new SuccessMessage { Text = "Thank You.  Your message has been sent" });

			return RedirectToAction("contact");
		}

        /// <summary>
        /// Executes the View for HowItWorks
        /// </summary>
        public ActionResult HowItWorks()
        {
            return View("OpenBrewHowItWorks");
        }

        /// <summary>
        /// Legacy howitworks route.  Keep old bookmarks alive but send them to the canonical dashed URL.
        /// </summary>
        public ActionResult HowItWorksLegacy()
        {
            return RedirectPermanent(Url.Action("HowItWorks", "Root"));
        }

        /// <summary>
        /// Legacy blog route.  OpenBrew does not ship a blog archive, so keep old links alive by sending
        /// visitors back to the homepage instead of serving a dead page.
        /// </summary>
        public ActionResult Blog()
        {
            return RedirectPermanent(Url.Action("Index", "Root"));
        }

        [Route("open-source-homebrew-software")]
	    public ActionResult OpenSourceSoftware()
        {
            return View("OpenBrewOpenSourceSoftware");
        }

		/// <summary>
		/// Executes the View for Sitemap
		/// </summary>
		public ContentResult Sitemap()
		{
			this.Response.ContentType = "text/xml";
			var seoSitemap = Resolve<ISeoSitemap>();
			return Content(seoSitemap == null ? string.Empty : seoSitemap.GenerateXml(this.Url));
		}

	}
}
