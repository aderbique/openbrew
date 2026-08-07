using System;
using System.Web;
using System.Web.Security;

namespace ctorx.Core.Security
{
    public class ASPNETFormsAuthenticationService : IAuthenticationService
    {
        /// <summary>
        /// Performs the sign in process
        /// </summary>
        public void SignIn(string identifier, bool persist)
        {
            if (string.IsNullOrEmpty(identifier))
            {
				throw new ArgumentNullException("identifier");
            }


			var issuedAt = DateTime.Now;
			var authTicket = new FormsAuthenticationTicket(1, //version
				identifier, // user name
				issuedAt,             //creation
				persist ? issuedAt.AddDays(30) : issuedAt.AddHours(8),
				persist,
				identifier); // additional informations

			var encryptedTicket = FormsAuthentication.Encrypt(authTicket);

			var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
			{
				HttpOnly = true,
				Secure = HttpContext.Current.Request.IsSecureConnection
			};

			if (persist)
			{
				authCookie.Expires = authTicket.Expiration;
			}

			HttpContext.Current.Response.Cookies.Add(authCookie);
        }

        /// <summary>
        /// Performs the sign out process
        /// </summary>
        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }

        /// <summary>
        /// Determines if the user is signed in
        /// </summary>
        public bool UserIsSignedIn()
        {
            return HttpContext.Current.User != null && !string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name);
        }


        /// <summary>
        /// Gets the identifier for the user
        /// </summary>
        public string GetUserIdentifier()
        {
            return HttpContext.Current.User.Identity.Name;
        }
    }
}
