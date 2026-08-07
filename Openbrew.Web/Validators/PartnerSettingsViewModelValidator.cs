using Openbrew.Web.Models;
using FluentValidation;

namespace Openbrew.Web.Validators
{
	public class PartnerSettingsViewModelValidator : AbstractValidator<PartnerSettingsViewModel>
	{
		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public PartnerSettingsViewModelValidator()
		{
			this.RuleFor(m => m.Name)
				.NotEmpty()
				.WithMessage("Please enter the partner name");

			this.RuleFor(m => m.ContactName)
				.NotEmpty()
				.WithMessage("Please enter the contact name");

			this.RuleFor(m => m.ContactAddress1)
				.NotEmpty()
				.WithMessage("Please enter the contact address");

			this.RuleFor(m => m.ContactCity)
				.NotEmpty()
				.WithMessage("Please enter the contact city");

			this.RuleFor(m => m.ContactStateProvince)
				.NotEmpty()
				.WithMessage("Please enter the contact state/province");

			this.RuleFor(m => m.ContactPostalCode)
				.NotEmpty()
				.WithMessage("Please enter the contact postal code");

			this.RuleFor(m => m.ContactCountry)
				.NotEmpty()
				.WithMessage("Please enter the contact country");

			this.RuleFor(m => m.ContactPhone)
				.NotEmpty()
				.WithMessage("Please enter the contact phone");

			this.RuleFor(m => m.ContactEmail)
				.NotEmpty().WithMessage("Please enter the contact email address")
				.EmailAddress().WithMessage("Please enter a valid email address");
		}
	}
}