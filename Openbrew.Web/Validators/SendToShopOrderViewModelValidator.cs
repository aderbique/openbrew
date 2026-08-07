using Openbrew.Web.Core.Model;
using Openbrew.Web.Models;
using FluentValidation;

namespace Openbrew.Web.Validators
{
	public class SendToShopOrderViewModelValidator : AbstractValidator<SendToShopOrderViewModel>
	{
		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public SendToShopOrderViewModelValidator()
		{
			this.RuleFor(x => x.Name).NotEmpty();
			this.RuleFor(x => x.PhoneNumber).NotEmpty();
			this.RuleFor(x => x.EmailAddress).NotEmpty().EmailAddress();
			this.RuleFor(x => x.RecipeId).GreaterThan(0);

			// Ingredient Child Validators
			this.RuleFor(m => m.Items).SetCollectionValidator(new SendToShopOrderItemValidator());
		}
	}

	public class SendToShopOrderItemValidator : AbstractValidator<SendToShopOrderItem>
	{
		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public SendToShopOrderItemValidator()
		{
			this.RuleFor(x => x.IngredientId).GreaterThan(0);
			this.RuleFor(x => x.IngredientTypeId).GreaterThan(0);
			this.RuleFor(x => x.Quantity).GreaterThan(0);
			this.RuleFor(x => x.Unit).NotEmpty();
		}
	}
}