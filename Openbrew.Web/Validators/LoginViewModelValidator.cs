using System;
using Openbrew.Web.Models;
using FluentValidation;

namespace Openbrew.Web.Validators
{
	public class LoginViewModelValidator : AbstractValidator<LoginViewModel>
	{
		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public LoginViewModelValidator()
		{
			this.RuleFor(x => x.EmailAddress)
				.NotEmpty()
				.WithMessage("Please enter an email address")
				.EmailAddress();

			this.RuleFor(x => x.Password)
				.NotEmpty()
				.WithMessage("Please enter your password");
		}
	}
}