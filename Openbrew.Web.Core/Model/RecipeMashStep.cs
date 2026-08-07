using System;
using System.Collections.Generic;
using System.Linq;

namespace Openbrew.Web.Core.Model
{
    public class RecipeMashStep : IRecipeIngredient
	{
		/// <summary>
		/// Gets or sets the RecipeStepId
		/// </summary>
		public int RecipeMashStepId { get; set; }

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
        /// Gets or sets the MashStep
        /// </summary>
        public virtual MashStep MashStep { get; set; }

        /// <summary>
        /// Gets or sets the Heat
        /// </summary>
        public string Heat { get; set; }

        /// <summary>
        /// Gets or sets the Temp
        /// </summary>
        public double Temp { get; set; }

        /// <summary>
        /// Gets or sets the Time
        /// </summary>
        public int Time { get; set; }

        /// <summary>
        /// Gets the Ingredient
        /// </summary>
        public IIngredient GetIngredient()
        {
            return this.MashStep;
        }

        /// <summary>
        /// Sets the Ingredient
        /// </summary>
        public void SetIngredient(IIngredient ingredient)
        {
            this.MashStep = (MashStep)ingredient;
        }
	}
}
