using System;
using FluentValidation;

namespace Openbrew.Web.Models
{
	public class BrewSessionWaterInfusionViewModelValidator : AbstractValidator<BrewSessionWaterInfusionViewModel>
	{
		/// <summary>
		/// ctor the Mighty
		/// </summary>
        public BrewSessionWaterInfusionViewModelValidator()
		{
            //this.RuleFor(m => m.PostBoilGravity)
            //    .NotEmpty();

            //this.RuleFor(m => m.PostBoilGravityTemp)
            //    .NotEmpty();

            //this.RuleFor(m => m.WortCoolingMethod)
            //    .NotEmpty();
		}
	}
}