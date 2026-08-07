using System;
using System.Text;
using Openbrew.Web.Core.Configuration;
using Openbrew.Web.Models;
using ctorx.Core.Collections;
using ctorx.Core.Email;
using ctorx.Core.Identity;

namespace Openbrew.Web.Email
{
	public class ContactFormEmailMessage : AbstractEmailMessage
	{
		readonly IWebSettings WebSettings;
		readonly IUserHostAddressResolver UserHostAddressResolver;
		ContactViewModel ContactViewModel;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public ContactFormEmailMessage(IWebSettings webSettings, IUserHostAddressResolver userHostAddressResolver)
		{
			WebSettings = webSettings;
			this.UserHostAddressResolver = userHostAddressResolver;

			this.SenderAddress = webSettings.SenderAddress;
			this.Subject = "openbrew Contact Form";
			this.FormatAsHtml = false;

			webSettings.ContactFormEmailAddress.ForEach(this.ToRecipients.Add);
		}

		/// <summary>
		/// Sets the Contact View Model
		/// </summary>
		public void SetContactViewModel(ContactViewModel contactViewModel)
		{
			this.ContactViewModel = contactViewModel;
			this.SenderDisplayName = contactViewModel.Name;
            this.ReplyToRecipients.Add(contactViewModel.EmailAddress);
		}

		/// <summary>
		/// Builds the message body
		/// </summary>
		public override string BuildMessageBody()
		{
			var message = new StringBuilder();

			message.AppendLine("openbrew Contact Form");
			message.AppendLine("----------------------------------------------------------------------------------");
			message.AppendLine("From: " + this.ContactViewModel.Name + " (" + this.ContactViewModel.EmailAddress + ") ");
			message.AppendLine("Date: " + DateTime.Now.ToString());
			message.AppendLine("IP Address: " + this.UserHostAddressResolver.Resolve());
			message.AppendLine();
			message.AppendLine("Message:");
			message.AppendLine(ContactViewModel.MessageContent);

			return message.ToString();
		}
	}
}
