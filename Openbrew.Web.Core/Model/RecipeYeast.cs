using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Openbrew.Web.Core.Model
{
    public class RecipeYeast : IRecipeIngredient
    {
		/// <summary>
		/// Gets or sets the RecipeYeastId
		/// </summary>
		public int RecipeYeastId { get; set; }

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
    	/// Gets or sets the Yeast
    	/// </summary>
    	public virtual Yeast Yeast { get; set; }

        /// <summary>
        /// Gets or sets the AttenuationPercent
        /// </summary>
        public double Attenuation { get; set; }

    	/// <summary>
    	/// Gets the Ingredient
    	/// </summary>
    	public IIngredient GetIngredient()
    	{
			return this.Yeast;
    	}

    	/// <summary>
    	/// Sets the Ingredient
    	/// </summary>
    	public void SetIngredient(IIngredient ingredient)
    	{
			this.Yeast = (Yeast)ingredient;
    	}
    }
}
