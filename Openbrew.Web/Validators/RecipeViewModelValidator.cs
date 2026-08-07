using System;
using System.Linq;
using ctorx.Core.Conversion;
using FluentValidation;

namespace Openbrew.Web.Models
{
	public class RecipeViewModelValidator : AbstractValidator<RecipeViewModel>
	{
		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public RecipeViewModelValidator()
		{
			this.RuleFor(x => x.Name)
				.NotEmpty()
				.Matches("[\\w\\d\\s-_]{1,}$")
				.WithMessage("Your Recipe name contains invalid characters");

			this.RuleFor(m => m.PhotoForUpload)
				.Must(x => x.ContentLength <= 10000000).When(x => x.PhotoForUpload != null)
				.WithMessage("Please use photos that are 10MB or less only");

			// Ingredient Child Validators
			this.RuleFor(x => x.Fermentables).SetCollectionValidator(new RecipeFermentableViewModelValidator());
			this.RuleFor(x => x.Hops).SetCollectionValidator(new RecipeHopViewModelValidator());
			this.RuleFor(x => x.Yeasts).SetCollectionValidator(new RecipeYeastViewModelValidator());
			this.RuleFor(x => x.Others).SetCollectionValidator(new RecipeOtherViewModelValidator());
            this.RuleFor(x => x.MashSteps).SetCollectionValidator(new RecipeMashStepViewModelValidator());
		}
	}

	public class RecipeFermentableViewModelValidator : AbstractValidator<RecipeFermentableViewModel>
	{
		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public RecipeFermentableViewModelValidator()
		{
			// Standard
			this.RuleFor(x => x.Rank).NotEmpty();
			this.RuleFor(x => x.Amt).NotEmpty();
			this.RuleFor(x => x.L).NotEmpty();
			this.RuleFor(x => x.Ppg).NotEmpty();
			this.RuleFor(x => x.Use).NotEmpty();

			// Conditional
			this.RuleFor(x => x.CustomName).NotEmpty().When(x => Converter.Convert<int>(x.IngId) == 0);
			this.RuleFor(x => x.IngId).NotEmpty().When(x => Converter.Convert<int>(x.Id) > 0);
		}
	}

	public class RecipeHopViewModelValidator : AbstractValidator<RecipeHopViewModel>
	{
		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public RecipeHopViewModelValidator()
		{
			// Standard
			this.RuleFor(x => x.Rank).NotEmpty();
			this.RuleFor(x => x.Amt).NotEmpty();
			this.RuleFor(x => x.Type).NotEmpty();
			this.RuleFor(x => x.AA).NotEmpty();
			this.RuleFor(x => x.Use).NotEmpty();
			this.RuleFor(x => x.Ibu).NotEmpty();

			// Conditional
			this.RuleFor(x => x.CustomName).NotEmpty().When(x => Converter.Convert<int>(x.IngId) == 0);
		}
	}

	public class RecipeYeastViewModelValidator : AbstractValidator<RecipeYeastViewModel>
	{
		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public RecipeYeastViewModelValidator()
		{
			// Standard
			this.RuleFor(x => x.Rank).NotEmpty();
			this.RuleFor(x => x.Atten).NotEmpty();

			// Conditional
			this.RuleFor(x => x.CustomName).NotEmpty().When(x => Converter.Convert<int>(x.IngId) == 0);
		}
	}

	public class RecipeOtherViewModelValidator : AbstractValidator<RecipeOtherViewModel>
	{
		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public RecipeOtherViewModelValidator()
		{
			// Standard
			this.RuleFor(x => x.Rank).NotEmpty();
			this.RuleFor(x => x.Amt).NotEmpty();
			this.RuleFor(x => x.Use).NotEmpty();
			this.RuleFor(x => x.Unit).NotEmpty();

			// Conditional
			this.RuleFor(x => x.CustomName).NotEmpty().When(x => Converter.Convert<int>(x.IngId) == 0);
		}
	}

    public class RecipeMashStepViewModelValidator : AbstractValidator<RecipeMashStepViewModel>
    {
        /// <summary>
        /// ctor the Mighty
        /// </summary>
        public RecipeMashStepViewModelValidator()
        {
            // Standard
            this.RuleFor(x => x.Rank).NotEmpty();
            this.RuleFor(x => x.Heat).NotEmpty();
            this.RuleFor(x => x.Temp).NotEmpty();
            this.RuleFor(x => x.Time).NotEmpty();

            // Conditional
            this.RuleFor(x => x.CustomName).NotEmpty().When(x => Converter.Convert<int>(x.IngId) == 0);
        }
    }
}