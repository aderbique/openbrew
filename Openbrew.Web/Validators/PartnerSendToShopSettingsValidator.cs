using Openbrew.Web.Core.Model;
using FluentValidation;

namespace Openbrew.Web.Validators
{
	public class PartnerSendToShopSettingsValidator : AbstractValidator<PartnerSendToShopSettings>
	{
		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public PartnerSendToShopSettingsValidator()
		{
			this.RuleFor(x => x.DayStart).InclusiveBetween(0, 6);
			this.RuleFor(x => x.DayEnd).InclusiveBetween(0, 6);
			this.RuleFor(x => x.HourStart).InclusiveBetween(1, 23);
			this.RuleFor(x => x.HourEnd).InclusiveBetween(1, 23);
			this.RuleFor(x => x.DeliveryEmailAddress).NotEmpty().EmailAddress();
			//this.RuleFor(x => x.ConfirmationMessageText).NotEmpty();
			//this.RuleFor(x => x.ContactPartnerMessageText).NotEmpty();
			//this.RuleFor(x => x.ReadyForPickupMessageText).NotEmpty();
		}
	}
}