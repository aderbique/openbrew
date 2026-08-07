using System;

namespace Openbrew.Web.Core.Model
{
	public class PartnerService
	{
		/// <summary>
		/// Gets or sets the PartnerId
		/// </summary>
		public int PartnerId { get; set; }

		/// <summary>
		/// Gets or sets the PartnerServiceTypeId
		/// </summary>
		public int PartnerServiceTypeId { get; set; }

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
	}
}