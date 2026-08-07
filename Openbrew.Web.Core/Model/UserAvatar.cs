using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Web;
using Openbrew.Web.Core.Configuration;
using ctorx.Core.Ninject;

namespace Openbrew.Web.Core.Model
{
    public static class UserAvatar
    {
        public static string GetAvatar (int size, string email) 
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email");
            }

            if(size == 0)
            {
                size = 80;
            }

            string hash = HashEmailForGravatar(email.ToLower().Trim());

			// NOTE: I don't like doing the HTTP/HTTPS check here...
			// TODO: Move this to the UrlHelper Extensions so it can decided HTTP or HTTPS

			bool isSecure = HttpContext.Current.Request.Url.Scheme == "https";

            return (isSecure ? "https://secure." : "http://www.") + "gravatar.com/avatar/" + hash + "?size=" + size + "&d=mm";
        }

        /// <summary>
        /// Returns md5 hash for Gravatar image
        /// </summary>
        private static string HashEmailForGravatar(string email)
        {
            var md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(email));
            var sBuilder = new StringBuilder();

            for (var i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

    }
}
