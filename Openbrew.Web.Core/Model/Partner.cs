using System;
using System.Collections;
using System.Collections.Generic;

namespace Openbrew.Web.Core.Model
{
	public class Partner
	{
		/// <summary>
		/// Gets or sets the PartnerId
		/// </summary>
		public int PartnerId { get; set; }

		/// <summary>
		/// Gets or sets the CompanyName
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the Token
		/// </summary>
		public string Token { get; set; }

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

		/// <summary>
		/// Gets or sets the IsActive
		/// </summary>
		public bool IsActive { get; set; }

		/// <summary>
		/// Gets or sets the IsPublic
		/// </summary>
		public bool IsPublic { get; set; }

		/// <summary>
		/// Gets or sets the DateCreated
		/// </summary>
		public DateTime DateCreated { get; set; }

		/// <summary>
		/// Gets or sets the DateModified
		/// </summary>
		public DateTime? DateModified { get; set; }

		/// <summary>
		/// Gets or sets the PartnerSummary
		/// </summary>
		public PartnerSummary PartnerSummary { get; set; }

		/// <summary>
		/// Gets or sets the PartnerServices
		/// </summary>
		public IList<PartnerService> PartnerServices { get; set; }

		/// <summary>
		/// Gets or sets the UserPartnerAdmins
		/// </summary>
		public IList<UserPartnerAdmin> UserPartnerAdmins { get; set; }

		/// <summary>
		/// Gets or sets the PartnerSendToShopSettings
		/// </summary>
		public PartnerSendToShopSettings SendToShopSettings { get; set; }

		/// <summary>
		/// Gets or sets the SendToShopIngredients
		/// </summary>
		public IList<PartnerSendToShopIngredient> SendToShopIngredients { get; set; }
	}
}