using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Openbrew.Web.Core.Model
{
    public class RecipeAdjunct : IRecipeIngredient
    {
		/// <summary>
		/// Gets or sets the RecipeAdjunctId
		/// </summary>
		public int RecipeAdjunctId { get; set; }

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
    	/// Gets or sets the Adjunct
    	/// </summary>
    	public virtual Adjunct Adjunct { get; set; }

        /// <summary>
		/// Gets or sets the AdjunctUsageId
        /// </summary>
        public int AdjunctUsageTypeId { get; set; }

        /// <summary>
        /// Gets or sets the Amount
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        /// Gets or sets the UnitOfMeasure
        /// </summary>
        public string Unit { get; set; }

		/// <summary>
		/// Gets the Usage Type
		/// </summary>
	    public AdjunctUsageType GetUsageType()
	    {
		    return (AdjunctUsageType)this.AdjunctUsageTypeId;
	    }

		/// <summary>
		/// Gets the Ingredient
		/// </summary>
		public IIngredient GetIngredient()
		{
			return this.Adjunct;
		}

		/// <summary>
		/// Sets the Ingredient
		/// </summary>
		public void SetIngredient(IIngredient ingredient)
		{
			this.Adjunct = (Adjunct)ingredient;
		}
    }
}
