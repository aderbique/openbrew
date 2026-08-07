using System;

namespace Openbrew.Web.Core.Model
{
	public class IngredientAffiliateProduct<TIngredientType> where TIngredientType : class, IIngredient
	{
		/// <summary>
		/// Gets or sets the IngredientAffiliateProductId
		/// </summary>
		public int IngredientAffiliateProductId { get; set; }

		/// <summary>
		/// Gets or sets the IngredientId
		/// </summary>
		public int IngredientId { get; set; }

		/// <summary>
		/// Gets or sets the Ingredient
		/// </summary>
		public TIngredientType Ingredient { get; set; }

		/// <summary>
		/// Gets or sets the AffiliateProductId
		/// </summary>
		public int AffiliateProductId { get; set; }

		/// <summary>
		/// Gets or sets the AffiliateProduct
		/// </summary>
		public AffiliateProduct AffiliateProduct { get; set; }

		/// <summary>
		/// Gets or sets the IsActive
		/// </summary>
		public bool IsActive { get; set; }

		/// <summary>
		/// Gets or sets the Rank
		/// </summary>
		public int Rank { get; set; }

		/// <summary>
		/// Gets or sets the DateCreated
		/// </summary>
		public DateTime DateCreated { get; set; }
	}
}