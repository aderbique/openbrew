using System;

namespace Openbrew.Web.Core.Model
{
	public interface IAffiliateProduct<TIngredientType> where TIngredientType : IIngredient
	{
		/// <summary>
		/// Gets or sets the FermentableAffiliateProductId
		/// </summary>
		int IngredientAffiliateProductId { get; set; }

		/// <summary>
		/// Gets or sets the IngredientId
		/// </summary>
		int IngredientId { get; set; }

		/// <summary>
		/// Gets or sets the Fermentable
		/// </summary>
		TIngredientType Ingredient { get; set; }

		/// <summary>
		/// Gets or sets the AffiliateProductId
		/// </summary>
		int AffiliateProductId { get; set; }

		/// <summary>
		/// Gets or sets the AffiliateProduct
		/// </summary>
		AffiliateProduct AffiliateProduct { get; set; }

		/// <summary>
		/// Gets or sets the IsActive
		/// </summary>
		bool IsActive { get; set; }

		/// <summary>
		/// Gets or sets the Rank
		/// </summary>
		int Rank { get; set; }

		/// <summary>
		/// Gets or sets the DateCreated
		/// </summary>
		DateTime DateCreated { get; set; }
	}
}