using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Openbrew.Web.Core.Model;

namespace Openbrew.Web.Models
{
	public class PartnerDashboardViewModel
	{
		/// <summary>
		/// Gets or sets the Partner
		/// </summary>
		public Partner Partner { get; set; }

		/// <summary>
		/// Gets or sets the Services
		/// </summary>
		public IList<PartnerService> Services { get; set; }

		/// <summary>
		/// Determines if the Partner is signed up for a specific service
		/// </summary>
		public bool HasService(PartnerServiceType partnerServiceType)
		{
			return this.Services.Any(x => x.PartnerServiceTypeId == (int)partnerServiceType);
		}
	}
}