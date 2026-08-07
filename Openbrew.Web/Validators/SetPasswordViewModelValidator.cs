using System;
using Openbrew.Web.Models;
using FluentValidation;

namespace Openbrew.Web.Validators
{
	public class SetPasswordViewModelValidator : AbstractValidator<SetPasswordViewModel>
	{
		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public SetPasswordViewModelValidator()
		{
			this.RuleFor(x => x.AuthToken)
				.NotEmpty()
				.WithMessage("Oops, something went wrong");

			this.RuleFor(x => x.Password)
				.NotEmpty()
					.WithMessage("Please enter your new password");

			this.RuleFor(x => x.ConfirmPassword)
				.NotEmpty()
					.WithMessage("Please re-type your new password")
				.Equal(x => x.Password)
					.WithMessage("The passwords do not match");
		}
	}
}