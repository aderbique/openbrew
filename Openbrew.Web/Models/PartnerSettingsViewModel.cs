using Openbrew.Web.Validators;
using ctorx.Core.Validation;

namespace Openbrew.Web.Models
{
	public class PartnerSettingsViewModel : ValidatesWith<PartnerSettingsViewModelValidator>
	{
		/// <summary>
		/// Gets or sets the PartnerId
		/// </summary>
		public int PartnerId { get; set; }

		/// <summary>
		/// Gets or sets the Name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the ContactName
		/// </summary>
		public string ContactName { get; set; }

		/// <summary>
		/// Gets or sets the Address1
		/// </summary>
		public string ContactAddress1 { get; set; }

		/// <summary>
		/// Gets or sets the Address2
		/// </summary>
		public string ContactAddress2 { get; set; }

		/// <summary>
		/// Gets or sets the ContactCity
		/// </summary>
		public string ContactCity { get; set; }

		/// <summary>
		/// Gets or sets the ContactStateProvince
		/// </summary>
		public string ContactStateProvince { get; set; }

		/// <summary>
		/// Gets or sets the ContactPostalCode
		/// </summary>
		public string ContactPostalCode { get; set; }

		/// <summary>
		/// Gets or sets the ContactCountry
		/// </summary>
		public string ContactCountry { get; set; }

		/// <summary>
		/// Gets or sets the ContactPhone
		/// </summary>
		public string ContactPhone { get; set; }

		/// <summary>
		/// Gets or sets the ContactFax
		/// </summary>
		public string ContactFax { get; set; }

		/// <summary>
		/// Gets or sets the ContactEmail
		/// </summary>
		public string ContactEmail { get; set; }
	}
}