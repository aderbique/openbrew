using System.Collections.Generic;

namespace Openbrew.Web.Core.Model
{
	public class RecipeSearchResults
	{
		/// <summary>
		/// Gets or sets the RecipeSearchOptions
		/// </summary>
		public RecipeSearchOptions RecipeSearchOptions { get; set; }

		/// <summary>
		/// Gets or sets the Recipes
		/// </summary>
		public IList<RecipeSummary> Recipes { get; set; }
	}
}