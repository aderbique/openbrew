using System;
using System.Web;

namespace Openbrew.Web
{
	public class BlogRedirectHttpModule : IHttpModule
	{
		/// <summary>
		/// Initializes a module and prepares it to handle requests.
		/// </summary>
		public void Init(HttpApplication context)
		{
			context.BeginRequest += delegate { this.DoTheRedirectAlready(context); };
		}

		/// <summary>
		/// That's right, do it.
		/// </summary>
		void DoTheRedirectAlready(HttpApplication context)
		{
			// We are checking the request for BlogEngine.NET type URLs so that we can issue 301 redirects
			// to the new URLs

			var rawUrl = context.Request.RawUrl ?? "/";
			var host = context.Request.Headers["Host"] ?? "";

			// The BlogEngine.Net app uses ASPX files, which Brewgr does not, so we can isolate most
			// files using the presence of that extension.  The exception is the tags URLs which
			// we simply look at the query for the presence of tag=
			if (rawUrl.Split('?')[0].ToLower().EndsWith(".aspx")
				|| rawUrl.ToLower().IndexOf("?tag=") > -1
				|| (rawUrl.ToLower().IndexOf("?page=") > -1 && rawUrl.ToLower().IndexOf("elmah.axd") == -1)
				|| rawUrl.ToLower().IndexOf("syndication.axd") > -1)
			{
				var newUrl = ("http://" + host + rawUrl).Replace("/" + host + "/", "/brewgr.com/blog/");

				context.Response.Clear();
				context.Response.Status = "301 Moved Permanently";
				context.Response.AddHeader("Location", newUrl);
				context.Response.End();
			}
		}

		/// <summary>
		/// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
		/// </summary>
		public void Dispose()
		{
			// I will dispose of nothing.  You can't make me do it!
		}
	}
}
