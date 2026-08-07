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