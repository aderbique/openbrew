using System;
using System.Linq;
using Openbrew.Web.Models;
using FluentValidation;

namespace Openbrew.Web.Validators
{
	public class ContactViewModelValidator : AbstractValidator<ContactViewModel>
	{
		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public ContactViewModelValidator()
		{
			this.RuleFor(x => x.Name)
				.NotEmpty().WithMessage("Please enter your name");

			this.RuleFor(x => x.EmailAddress)
				.NotEmpty()
				.EmailAddress().WithMessage("Please enter a valid email address");

			this.RuleFor(x => x.MessageContent)
				.NotEmpty().WithMessage("Please enter your message");
		}
	}
}