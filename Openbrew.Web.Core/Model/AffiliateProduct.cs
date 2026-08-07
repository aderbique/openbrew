using System;

namespace Openbrew.Web.Core.Model
{
	public class AffiliateProduct
	{
		/// <summary>
		/// Gets or sets the AffiliateProductId
		/// </summary>
		public int AffiliateProductId { get; set; }

		/// <summary>
		/// Gets or sets the AffiliateId
		/// </summary>
		public int AffiliateId { get; set; }

		/// <summary>
		/// Gets or sets the Affiliate
		/// </summary>
		public Affiliate Affiliate { get; set; }

		/// <summary>
		/// Gets or sets the Sku
		/// </summary>
		public string Sku { get; set; }

		/// <summary>
		/// Gets or sets the Price
		/// </summary>
		public decimal Price { get; set; }

		/// <summary>
		/// Gets or sets the Name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the Description
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the Url
		/// </summary>
		public string Url { get; set; }

		/// <summary>
		/// Gets or sets the ImageUrl
		/// </summary>
		public string ImageUrl { get; set; }

		/// <summary>
		/// Gets or sets the Category
		/// </summary>
		public string Category { get; set; }

		/// <summary>
		/// Gets or sets the InStock
		/// </summary>
		public bool InStock { get; set; }

		/// <summary>
		/// Gets or sets the IsActive
		/// </summary>
		public bool IsActive { get; set; }

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