using System;
using Openbrew.Web.Core.Configuration;
using ctorx.Core.Email;
using ctorx.Core.Identity;

namespace Openbrew.Web.Email
{
	public class DefaultEmailMessageFactory : IEmailMessageFactory
	{
		readonly IWebSettings WebSettings;
		readonly IUserHostAddressResolver UserHostAddressResolver;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public DefaultEmailMessageFactory(IWebSettings webSettings, IUserHostAddressResolver userHostAddressResolver)
		{
			this.WebSettings = webSettings;
			this.UserHostAddressResolver = userHostAddressResolver;
		}

		/// <summary>
		/// Makes an Email Message
		/// </summary>
		public IEmailMessage Make(EmailMessageType emailMessageType)
		{
			switch (emailMessageType)
			{
				case EmailMessageType.PasswordReset:
					return new PasswordResetEmailMessage(this.WebSettings);

				case EmailMessageType.ContactForm:
					return new ContactFormEmailMessage(this.WebSettings, this.UserHostAddressResolver);

                case EmailMessageType.NewAccount:
                    return new NewAccountEmailMessage(this.WebSettings);

				default:
					throw new InvalidOperationException("Dear compiler, please check yo self so I don't have to default");
			}
		}
	}
}