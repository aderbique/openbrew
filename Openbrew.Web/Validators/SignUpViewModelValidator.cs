using System;
using Openbrew.Web.Models;
using FluentValidation;

namespace Openbrew.Web.Validators
{
	public class SignUpViewModelValidator : AbstractValidator<SignUpViewModel>
	{
		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public SignUpViewModelValidator()
		{
			this.RuleFor(x => x.NewUserEmailAddress)
				.NotEmpty().WithMessage("Please enter your email address")
				.EmailAddress().WithMessage("Please enter a valid email address");

			this.RuleFor(x => x.NewUserFullName)
				.NotEmpty().WithMessage("Please enter your name");

			this.RuleFor(x => x.NewUserPassword)
				.NotEmpty().WithMessage("Please enter a password");
		}
	}
}