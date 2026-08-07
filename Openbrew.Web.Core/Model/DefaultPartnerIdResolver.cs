using System;
using System.Web;
using Openbrew.Web.Core.Service;
using ctorx.Core.Crypto;
using Ninject.Activation;

namespace Openbrew.Web.Core.Model
{
	public class DefaultPartnerIdResolver : IPartnerIdResolver
	{
		readonly IStringCryptoService StringCryptoService;
		readonly IPartnerService PartnerService;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public DefaultPartnerIdResolver(IStringCryptoService stringCryptoService, IPartnerService partnerService)
		{
			this.StringCryptoService = stringCryptoService;
			this.PartnerService = partnerService;
		}

		/// <summary>
		/// Resolves the partner Id
		/// </summary>
		public int? Resolve()
		{
			// Check Cookie First
			var partnerCookie = HttpContext.Current.Request.Cookies["pid"];
			if(partnerCookie != null)
			{
				var decrypted = this.StringCryptoService.Decrypt(partnerCookie.Value);

				int pid = 0;
				Int32.TryParse(decrypted, out pid);

				if(pid > 0)
				{
					return pid;
				}
			}

			return null;
		}

		/// <summary>
		/// Persists the partner Id
		/// </summary>
		public void Persist(int partnerId)
		{
			if(partnerId <= 0)
			{
				throw new ArgumentOutOfRangeException("partnerId");
			}

			// Only write to cookie as needed
			if(HttpContext.Current.Request.Cookies["pid"] == null)
			{
				var encrypted = this.StringCryptoService.Encrypt(partnerId.ToString());
				HttpContext.Current.Response.Cookies["pid"].Value = encrypted;
			}
		}
	}
}