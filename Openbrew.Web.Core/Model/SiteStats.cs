using System;
using System.Collections.Generic;
using System.Linq;

namespace Openbrew.Web.Core.Model
{
	public class SiteStats
	{
		/// <summary>
		/// Gets or sets the RecipeCount
		/// </summary>
		public int RecipeCount { get; set; }

		/// <summary>
		/// Gets or sets the BrewSessionCount
		/// </summary>
		public int BrewSessionCount { get; set; }

		/// <summary>
		/// Gets or sets the UserCount
		/// </summary>
		public int UserCount { get; set; }

		/// <summary>
		/// Gets or sets the TastingNoteCount
		/// </summary>
		public int TastingNoteCount { get; set; }

		/// <summary>
		/// Gets or sets the LoginCount
		/// </summary>
		public int LoginCountLast24Hours { get; set; }

		/// <summary>
		/// Gets or sets the CustomIngredientCount
		/// </summary>
		public int CustomIngredientCount { get; set; }

		/// <summary>
		/// Gets or sets the CommentCount
		/// </summary>
		public int CommentCount { get; set; }

		/// <summary>
		/// Gets or sets the RecentCustomIngredients
		/// </summary>
		public IList<IIngredient> RecentCustomIngredients { get; set; }

		/// <summary>
		/// Gets or sets the Last25Users
		/// </summary>
		public IList<UserSummary> Last25Users { get; set; }

		/// <summary>
		/// Gets the most recent recorded account logins.
		/// </summary>
		public IList<UserLogin> Last25UserLogins { get; set; }

		/// <summary>
		/// Gets or sets the Last25Recipes
		/// </summary>
		public IList<RecipeSummary> Last25Recipes { get; set; }

        /// <summary>
        /// Gets or sets the Last25RecipeComments
        /// </summary>
        public IList<RecipeComment> Last25RecipeComments { get; set; }

        /// <summary>
        /// Gets or sets the Last25BrewSessionComments
        /// </summary>
        public IList<BrewSessionComment> Last25BrewSessionComments { get; set; }
	}
}
