using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Openbrew.Web.Core.Data;
using Openbrew.Web.Core.Model;
using StackExchange.Exceptional;
using StackExchange.Exceptional.Stores;
using ctorx.Core.Ninject;
using Openbrew.Web.Controllers;
using Openbrew.Web.Core.Service;
using Ninject;
using FluentValidation.Mvc;

namespace Openbrew.Web
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801


	public class MvcApplication : System.Web.HttpApplication
	{
		static readonly Regex WwwRegex = new Regex("(http|https)://www\\.", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		/// <summary>
		/// Fires on Begin Request
		/// </summary>
		protected void Application_BeginRequest()
		{
			var context = HttpContext.Current;
			var request = context.Request;
			var host = request.Headers["Host"] ?? string.Empty;
			var scheme = request.IsSecureConnection ? "https" : "http";

			if (WwwRegex.IsMatch(host))
			{
				var newUrl = WwwRegex.Replace(host, string.Empty);
				context.Response.RedirectPermanent(string.Format("{0}://{1}{2}", scheme, newUrl, request.RawUrl));
				return;
			}

			// Handle Partner Detection
			if (!string.IsNullOrWhiteSpace(request.QueryString["pid"]))
			{
				var kernel = KernelPersister.Get();
				var partnerService = kernel.Get<IPartnerService>();
				var partnerIdResolver = kernel.Get<IPartnerIdResolver>();

				var token = request.QueryString["pid"];
				var partnerId = partnerService.GetPartnerIdFromToken(token);

				if (partnerId != null)
				{
					// Persist the Partner Id
					partnerIdResolver.Persist(partnerId.Value);

					// Redirect (to remove the url token)
					var redirectUrl = request.RawUrl.ToLower().Replace("pid=" + token.ToLower(), "").TrimEnd(new[] { '?', '#' });
					context.Response.RedirectPermanent(string.Format("{0}://{1}{2}", scheme, host, redirectUrl), true);
				}
			}
		}

		/// <summary>
		/// Fires on Application Error
		/// </summary>
		protected void Application_Error(object sender, EventArgs e)
		{
			var environment = ctorx.Core.Configuration.ConfigReader.AppSettings.Read("Environment");

			if (environment != "dev")
			{
				var ex = Server.GetLastError().GetBaseException();

				Context.Response.Clear();
				Server.ClearError();

				var routeData = new RouteData();
				routeData.Values.Add("controller", "Error");
				routeData.Values.Add("action", "500");

				if (ex.GetType() == typeof(HttpException))
				{
					var httpException = (HttpException)ex;
					var code = httpException.GetHttpCode();

					// Is it a 4xx Error
					if (code % 400 < 100)
					{
						routeData.Values["action"] = "404";
					}
				}

				ErrorStore.LogException(ex, this.Context);

				routeData.Values.Add("error", ex);

				IController errorController = new ErrorController();
				errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
			}
		}
	}
}
