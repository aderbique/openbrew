using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Web;
using System.Web.Mvc;
using FluentValidation;
using StackExchange.Exceptional;
using ctorx.Core.Collections;
using ctorx.Core.Configuration;
using ctorx.Core.Messaging;
using ctorx.Core.Ninject;
using ctorx.Core.Security;
using ctorx.Core.Validation;
using ctorx.Core.Web;
using Openbrew.Web.Core.Configuration;
using Openbrew.Web.Core.Model;
using Openbrew.Web.Core.Service;

namespace Openbrew.Web.Controllers
{
	public abstract class BrewgrController : Controller
	{
		readonly IMessageStore ForwardedMessageStore;
		readonly IUserResolver UserResolver;
		readonly IUserService UserService;
		readonly IAuthenticationService AuthenticationService;
		readonly IGoogleConnectSettings GoogleConnectSettings;

		protected UserSummary ActiveUser;
		protected readonly IWebSettings WebSettings;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		protected BrewgrController()
		{
			// Setup Messaging
			this.ViewBag.Messages = new List<IMessage>();
			this.ForwardedMessageStore = new TempDataMessageStore(this.TempData);

			// Set Environment
			ViewBag.Environment = ConfigReader.AppSettings.Read("Environment");

			// Dependencies (manual injection to avoid ctor pollution)
			try
			{
				var kernel = KernelPersister.Get();
				this.UserResolver = kernel.GetService(typeof(IUserResolver)) as IUserResolver;
				this.UserService = kernel.GetService(typeof(IUserService)) as IUserService;
				this.GoogleConnectSettings = kernel.GetService(typeof(IGoogleConnectSettings)) as IGoogleConnectSettings;
				this.WebSettings = kernel.GetService(typeof(IWebSettings)) as IWebSettings;
				this.AuthenticationService = kernel.GetService(typeof(IAuthenticationService)) as IAuthenticationService;
			}
			catch
			{
				// Keep static pages alive even if the data layer is unavailable.
				this.UserResolver = null;
				this.UserService = null;
				this.GoogleConnectSettings = null;
				this.WebSettings = null;
				this.AuthenticationService = null;
			}
		}

		#region LIFECYCLE

		/// <summary>
		/// Begins to invoke the action in the current controller context.
		/// </summary>
		protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
		{
			// Resolve the Active User
            if (this.User.Identity.IsAuthenticated)
            {
                try
                {
					if (this.UserResolver != null)
					{
	this.ActiveUser = this.UserResolver.Resolve();
					}
                    ViewBag.ActiveUser = this.ActiveUser;
                }
				catch (SecurityException se)
				{
					this.AuthenticationService?.SignOut();
					Session.Abandon();
				}
				catch
				{
					this.AuthenticationService?.SignOut();
					Session.Abandon();
				}
            }

            // Site Announcement
            //ViewBag.SiteAnnouncement = "Brewgr is now open source software!  <a href=\"/open-source-homebrew-software\">Learn more</a>";

            return base.BeginExecuteCore(callback, state);
		}

		/// <summary>
		/// Fires on Action Executing
		/// </summary>
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			// Append any Forwarded Messages
			var pushedMessages = this.ForwardedMessageStore.GetMessages();
			if (pushedMessages != null)
			{
				pushedMessages.ForEach(this.AppendMessage);
			}

			// Set View Bag Values for Client
			var scheme = Request.IsSecureConnection ? "https" : "http";
			ViewBag.StaticRoot = this.WebSettings == null
				? string.Empty
				: (scheme == "https" ? this.WebSettings.StaticRootPathSecure : this.WebSettings.StaticRootPath);
			ViewBag.GoogleAuthClientId = this.GoogleConnectSettings == null
				? string.Empty
				: this.GoogleConnectSettings.ApplicationKey;

			base.OnActionExecuting(filterContext);
		}

		#endregion

		#region MESSAGING

		/// <summary>
		/// Determines if there are any messages in the message store
		/// </summary>
		public bool HasMessages()
		{
			if(ViewBag.Messages == null)
			{
				return false;
			}

			return (this.ViewBag.Messages as List<IMessage>).Any();
		}

		/// <summary>
		/// Appends a Message to the Rendered View
		/// </summary>
		public void AppendMessage(IMessage message)
		{
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}

			this.ViewBag.Messages.Add(message);
		}

		/// <summary>
		/// Forwards a message to the new request
		/// </summary>
		protected void ForwardMessage(IMessage message)
		{
			this.ForwardedMessageStore.AddMessage(message);
		}

		/// <summary>
		/// Validates a model without messaging
		/// </summary>
		protected bool ValidateWithoutMessaging(IValidatingViewModel validatingViewModel)
		{
			return this.ValidateAndMessage(validatingViewModel);
		}

		/// <summary>
		/// Validates and appends any error messages
		/// </summary>
		protected bool ValidateAndAppendMessages(IValidatingViewModel validatingViewModel)
		{
			return this.ValidateAndMessage(validatingViewModel, this.AppendMessage);
		}

		/// <summary>
		/// Validates and forwards any error messages
		/// </summary>
		protected bool ValidateAndForwardMessages(IValidatingViewModel validatingViewModel)
		{
			return ValidateAndMessage(validatingViewModel, this.ForwardMessage);
		}

		/// <summary>
		/// Validates and Messages
		/// </summary>
		protected bool ValidateAndMessage(IValidatingViewModel validatingViewModel, Action<IMessage> messagingAction = null)
		{
			var result = validatingViewModel.Validate();
			if (!result.IsValid)
			{
				if(messagingAction != null)
				{
					result.Errors.ForEach(x => messagingAction(new ErrorMessage { Text = x.ErrorMessage }));
				}

				return false;
			}

			return true;
		}

		/// <summary>
		/// Validates and appends any error messages
		/// </summary>
		protected bool ValidateAndAppendMessages<TModel>(TModel model, IValidator<TModel> validator) where TModel : class
		{
			return this.ValidateAndMessage(model, validator, this.AppendMessage);
		}

		/// <summary>
		/// Validates and forwards any error messages
		/// </summary>
		protected bool ValidateAndForwardMessages<TModel>(TModel model, IValidator<TModel> validator) where TModel : class
		{
			return ValidateAndMessage(model, validator, this.ForwardMessage);
		}

		/// <summary>
		/// Validates and Messages
		/// </summary>
		protected bool ValidateAndMessage<TModel>(TModel model, IValidator<TModel> validator, Action<IMessage> messagingAction)
		{
			var result = validator.Validate(model);
			if (!result.IsValid)
			{
				result.Errors.ForEach(x => messagingAction(new ErrorMessage { Text = x.ErrorMessage }));
				return false;
			}

			return true;
		}

		#endregion

		#region UTILITY

		/// <summary>
		/// Logs a handled exception
		/// </summary>
		public void LogHandledException(Exception exception)
		{
			ErrorStore.LogException(exception, System.Web.HttpContext.Current);
		}

		/// <summary>
		/// Disables the browser cache for the response
		/// </summary>
		public void DisableBrowserCache()
		{
			Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
			Response.Cache.SetValidUntilExpires(false);
			Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
			Response.Cache.SetCacheability(HttpCacheability.NoCache);
			Response.Cache.SetNoStore();
		}

		#endregion

		#region FORCE STATUS CODE

		/// <summary>
		/// Issues a 404 Response
		/// </summary>
		public ActionResult Issue404()
		{
			// Returning the view directly avoids exposing an ASP.NET debug exception
			// page for normal cases such as private or removed recipes.
			Response.StatusCode = 404;
			Response.StatusDescription = "Not Found";
			Response.TrySkipIisCustomErrors = true;
			return View("~/Views/Error/404.cshtml");
		}

		/// <summary>
		/// Issues a 500 Response
		/// </summary>
		public EmptyResult Issue500()
		{
			throw new HttpException(500, "Server Error");
		}

		#endregion

		#region REUSABLE DATA

		/// <summary>
		/// Adds country data to the ViewBag
		/// </summary>
		public void AddCountryDataToViewBag(bool addChooseOption = true)
		{
			ViewBag.Countries = this.UserService.GetCountryList();
			if(addChooseOption)
			{
				ViewBag.Countries.Insert(0, new Country { CountryCode = "", CountryName = "Choose..." });
			}
		}

		#endregion
	}
}
