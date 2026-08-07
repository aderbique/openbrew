using System;
using Openbrew.Web.Models;
using FluentValidation;

namespace Openbrew.Web.Validators
{
	public class UserSettingsViewModelValidator : AbstractValidator<UserSettingsViewModel>
	{
		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public UserSettingsViewModelValidator()
		{
			this.RuleFor(x => x.Username)
			    .NotEmpty()
				.Length(3,50)
			    .Matches("([A-Za-z0-9\\-]+)");

			this.RuleFor(x => x.EmailAddress)
				.NotEmpty().WithMessage("Please enter your email address")
				.EmailAddress().WithMessage("Please enter a valid email address");

			this.RuleFor(x => x.FirstName)
				.NotEmpty().WithMessage("Please enter your first name");

			this.RuleFor(x => x.LastName)
				.NotEmpty().WithMessage("Please enter your last name");
		}
	}
}