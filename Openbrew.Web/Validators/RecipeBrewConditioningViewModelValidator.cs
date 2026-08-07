using System;
using FluentValidation;

namespace Openbrew.Web.Models
{
	public class BrewSessionConditioningViewModelValidator : AbstractValidator<BrewSessionConditioningViewModel>
	{
		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public BrewSessionConditioningViewModelValidator()
		{
			this.RuleFor(m => m.ConditionDate)
				.NotEmpty();

			this.RuleFor(m => m.ConditionType)
				.NotEmpty();

			this.RuleFor(m => m.ConditionLength)
				.NotEmpty();
		}
	}
}