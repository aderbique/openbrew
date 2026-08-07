using System;
using System.Web.Mvc;
using Openbrew.Web.Core.Configuration;
using ctorx.Core.Ninject;

namespace Openbrew.Web
{
	public class ForceHttps : RequireHttpsAttribute
	{
		/// <summary>
		/// Fires on Authorization
		/// </summary>
		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			if(filterContext == null)
			{
				throw new ArgumentNullException("filterContext");
			}

			if(filterContext.HttpContext != null)
			{
				// Traefik terminates TLS before proxying requests to the container.  In
				// that deployment the internal connection is HTTP, while the browser
				// connection is already HTTPS.  Respect the proxy's forwarded scheme
				// to avoid redirecting an HTTPS request back to itself indefinitely.
				var forwardedProto = filterContext.HttpContext.Request.Headers["X-Forwarded-Proto"];
				if (string.Equals(forwardedProto, "https", StringComparison.OrdinalIgnoreCase))
				{
					return;
				}

				var kernel = KernelPersister.Get();
				var settings = kernel.GetService(typeof(IWebSettings)) as IWebSettings;

				// Disable RequireHttps if not PROD
				if (!(settings is ProdWebSettings))
				{
					return;
				}
			}

			base.OnAuthorization(filterContext);
		}
	}
}
