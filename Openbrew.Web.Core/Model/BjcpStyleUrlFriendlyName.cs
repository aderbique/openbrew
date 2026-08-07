using System;

namespace Openbrew.Web.Core.Model
{
	public class BjcpStyleUrlFriendlyName
	{
		/// <summary>
		/// Gets or sets the SubCategoryId
		/// </summary>
		public string SubCategoryId { get; set; }

		/// <summary>
		/// Gets or sets the UrlFriendlyName
		/// </summary>
		public string UrlFriendlyName { get; set; }

		/// <summary>
		/// Gets or sets the BjcpStyle
		/// </summary>
		public BjcpStyle BjcpStyle { get; set; }
	}
}