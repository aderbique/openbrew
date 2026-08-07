using System;
using ctorx.Core.Email;

namespace Openbrew.Web.Email
{
	public interface IEmailMessageFactory
	{
		/// <summary>
		/// Makes an Email Message
		/// </summary>
		IEmailMessage Make(EmailMessageType emailMessageType);
	}
}