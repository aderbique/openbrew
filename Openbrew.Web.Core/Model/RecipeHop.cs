using System;

namespace Openbrew.Web.Core.Model
{
	public class RecipeHop : IRecipeIngredient
	{
		/// <summary>
		/// Gets or sets the RecipeHopId
		/// </summary>
		public int RecipeHopId { get; set; }

		/// <summary>
		/// Gets or sets the RecipeId
		/// </summary>
		public int RecipeId { get; set; }

		/// <summary>
		/// Gets or sets the Recipe
		/// </summary>
		public Recipe Recipe { get; set; }

		/// <summary>
		/// Gets or sets the IngredientId
		/// </summary>
		public int IngredientId { get; set; }

		/// <summary>
		/// Gets or sets the Rank
		/// </summary>
		public int Rank { get; set; }

		/// <summary>
		/// Gets or sets the Hop
		/// </summary>
		public virtual Hop Hop { get; set; }

		/// <summary>
		/// Gets or sets the HopUsageTypeId
		/// </summary>
		public int HopUsageTypeId { get; set; }

		/// <summary>
		/// Gets or sets the HopTypeId
		/// </summary>
		public int HopTypeId { get; set; }

		/// <summary>
		/// Gets or sets the AlphaAcidAmount
		/// </summary>
		public double AlphaAcidAmount { get; set; }

		/// <summary>
		/// Gets or sets the AmountInOunces
		/// </summary>
		public double Amount { get; set; }

		/// <summary>
		/// Gets or sets the TimeInMinutes
		/// </summary>
		public int TimeInMinutes { get; set; }

		/// <summary>
		/// Gets or sets the Ibu
		/// </summary>
		public double Ibu { get; set; }

		/// <summary>
		/// Gets the Ingredient
		/// </summary>
		public IIngredient GetIngredient()
		{
			return this.Hop;
		}

		/// <summary>
		/// Gets the Hop Type
		/// </summary>
		public HopType GetHopType()
		{
			return (HopType)this.HopTypeId;
		}

		/// <summary>
		/// Gets the Hop Usage Type
		/// </summary>
		public HopUsageType GetUsageType()
		{
			return (HopUsageType)this.HopUsageTypeId;
		}

		/// <summary>
		/// Sets the Ingredient
		/// </summary>
		public void SetIngredient(IIngredient ingredient)
		{
			this.Hop = (Hop)ingredient;
		}
	}
}