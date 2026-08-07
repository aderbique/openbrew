using System;
using System.Linq;
using System.Web;

namespace Openbrew.Web
{
	public class CustomHttpHeadersModule : IHttpModule
	{		/// <summary>
		/// Initializes a module and prepares it to handle requests.
		/// </summary>
		public void Init(HttpApplication context)
		{
			context.PreSendRequestHeaders += (sender, args) =>
			{
				var response = context.Response;
				response.Headers.Remove("Server");
				response.Headers.Remove("X-Powered-By");
				response.Headers.Set("X-Content-Type-Options", "nosniff");
				response.Headers.Set("X-Frame-Options", "SAMEORIGIN");
				response.Headers.Set("Referrer-Policy", "strict-origin-when-cross-origin");
				response.Headers.Set("Permissions-Policy", "camera=(), microphone=(), geolocation=()");

				var forwardedProto = context.Request.Headers["X-Forwarded-Proto"];
				if (context.Request.IsSecureConnection || string.Equals(forwardedProto, "https", StringComparison.OrdinalIgnoreCase))
				{
					response.Headers.Set("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
				}
			};

			// Handle Caching Headers
			context.BeginRequest += (sender, args) =>
			{
				var absolutePath = (context.Request.RawUrl ?? "/").Split('?')[0].ToLower();

				// URLs with Extensions
				if (absolutePath.IndexOf(".") > -1)
				{
					// Get Extension
					var extension = absolutePath.Split('.').Reverse().First();

					// Set Caching for Images (far future)
					if(extension == "jpg" || extension == "png" || extension == "gif" || extension == "ico")
					{
						context.Response.AddHeader("Cache-Control", "max-age=631139000,public");
						context.Response.AddHeader("Expires", DateTime.Now.AddYears(20).ToString("r"));
					}

					// Versioned CSS and JS assets can be cached between deployments.
					if(extension == "js" || extension == "css")
					{
						context.Response.AddHeader("Cache-Control", "max-age=604800,public");
						context.Response.AddHeader("Expires", DateTime.UtcNow.AddDays(7).ToString("r"));
					}
				}
				// URLs with no extensions
				else
				{
					// New Ingredient Rows
					if (absolutePath.EndsWith("buildertemplates-v2"))
					{
						context.Response.AddHeader("Cache-Control", "max-age=86400,public");
						context.Response.AddHeader("Expires", DateTime.Now.AddYears(20).ToString("r"));
					}

					// Recent Photos
					if(absolutePath == "/recentphotos")
					{
						context.Response.AddHeader("Cache-Control", "max-age=900,public");
						context.Response.AddHeader("Expires", DateTime.Now.AddMinutes(15).ToString("r"));
					}
				}
			};
		}

		/// <summary>
		/// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
		/// </summary>
		public void Dispose()
		{
			// whatever
		}
	}
}
