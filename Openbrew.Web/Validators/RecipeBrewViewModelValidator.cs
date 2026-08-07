using System;
using System.Linq;
using Openbrew.Web.Models;
using FluentValidation;

namespace Openbrew.Web.Validators
{
	[Obsolete]
	public class BrewSessionViewModelValidator : AbstractValidator<BrewSessionViewModel>
	{
		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public BrewSessionViewModelValidator()
		{
			this.RuleFor(x => x.BrewDate)
				.NotEmpty();
		}
	}
}