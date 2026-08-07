using System;

namespace Openbrew.Web.Core.Model
{
	public class RecipeFermentable : IRecipeIngredient
	{
		/// <summary>
		/// Gets or sets the RecipeFermentableId
		/// </summary>
		public int RecipeFermentableId { get; set; }

		/// <summary>
		/// Gets or sets the RecipeId
		/// </summary>
		public int RecipeId { get; set; }

		/// <summary>
		/// Gets or sets the Recipe
		/// </summary>
		public Recipe Recipe { get; set; }

        /// <summary>
        /// Gets or sets the FermentableId
        /// </summary>
        public int IngredientId { get; set; }

		/// <summary>
		/// Gets or sets the Rank
		/// </summary>
		public int Rank { get; set; }

		/// <summary>
		/// Gets or sets the Fermentable
		/// </summary>
		public virtual Fermentable Fermentable { get; set; }

        /// <summary>
        /// Gets or sets the Amount
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        /// Gets or sets the Ppg
        /// </summary>
        public int Ppg { get; set; }

        /// <summary>
        /// Gets or sets the Lovibond
        /// </summary>
        public int Lovibond { get; set; }

		/// <summary>
		/// Gets or sets the FermentableUsageTypeId
		/// </summary>
		public int FermentableUsageTypeId { get; set; }

		/// <summary>
		/// Gets the Usage Type
		/// </summary>
		public FermentableUsageType GetUsageType()
		{
			return (FermentableUsageType)this.FermentableUsageTypeId;
		}

		/// <summary>
		/// Gets the Ingredient
		/// </summary>
		public IIngredient GetIngredient()
		{
			return this.Fermentable;
		}

		/// <summary>
		/// Sets the Ingredient
		/// </summary>
		public void SetIngredient(IIngredient ingredient)
		{
			this.Fermentable = (Fermentable)ingredient;
		}
	}
}