using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Openbrew.Web.Core.Service;
using ctorx.Core.Collections;

namespace Openbrew.Web.Core.Model
{
    public class DefaultIngredientCategorizer : IIngredientCategorizer
    {
	    readonly IList<IngredientCategory> IngredientCategories; 

	    /// <summary>
	    /// ctor the Mighty
	    /// </summary>
	    public DefaultIngredientCategorizer(IRecipeDataService recipeDataService)
	    {
		    this.IngredientCategories = recipeDataService.GetIngredientCategories();
	    }

	    /// <summary>
		/// Categorizes a list of Ingredients
		/// </summary>
		public IList<IngredientGroup<TIngredientType>> Categorize<TIngredientType>(IList<TIngredientType> ingredients) where TIngredientType : IIngredient
	    {
			// Default Uncategorized to "Other"
		    ingredients.Where(x => string.IsNullOrWhiteSpace(x.Category)).ForEach(x => x.Category = "Other");

			// Sorts the Groups by Rank (from IngredientCategory table), then by natural sort, with Other forced to the end
			return ingredients.OrderBy(x => x.Name, new NaturalSortComparer()).GroupBy(x => x.Category)
				.OrderBy(x => this.IngredientCategories.FirstOrDefault(y => y.Category == x.Key) != null ? this.IngredientCategories.FirstOrDefault(y => y.Category == x.Key).Rank : 500)
				.ThenBy(x => x.Key, new NaturalSortComparer())
				.Select(x => new IngredientGroup<TIngredientType> { Category = x.Key, Ingredients = x.ToList() })
				.ToList();
        }
    }
}
