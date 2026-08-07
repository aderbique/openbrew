using Openbrew.Web.Core.Model;

namespace Openbrew.Web.Models
{
	public class PartnerServiceStatusViewModel
	{
		/// <summary>
		/// Gets or sets the PartnerId
		/// </summary>
		public int PartnerId { get; set; }

		/// <summary>
		/// Gets or sets the PartnerServiceType
		/// </summary>
		public PartnerServiceType PartnerServiceType { get; set; }

		/// <summary>
		/// Gets or sets the IsEnabled
		/// </summary>
		public bool IsEnabled { get; set; }
	}
}