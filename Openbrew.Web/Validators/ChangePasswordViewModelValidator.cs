using System;
using Openbrew.Web.Models;
using FluentValidation;

namespace Openbrew.Web.Validators
{
	public class ChangePasswordViewModelValidator : AbstractValidator<ChangePasswordViewModel>
	{
		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public ChangePasswordViewModelValidator()
		{
			this.RuleFor(m => m.CurrentPassword)
				.NotEmpty().WithMessage("Please enter your current password");

			this.RuleFor(m => m.NewPassword)
				.NotEmpty().WithMessage("Please enter your new password");

			this.RuleFor(m => m.ConfirmNewPassword)
				.NotEmpty().WithMessage("Please re-type your new password")
				.Equal(x => x.NewPassword).WithMessage("The passwords do not match");
		}
	}
}