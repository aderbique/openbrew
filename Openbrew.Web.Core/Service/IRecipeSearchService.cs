using System.Collections.Generic;
using Openbrew.Web.Core.Model;

namespace Openbrew.Web.Core.Service
{
	public interface IRecipeSearchService
	{
		/// <summary>
		/// Searches for recipes
		/// </summary>
		IList<RecipeSummary> SearchRecipes(RecipeSearchOptions recipeSearchOptions);
	}
}