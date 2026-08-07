using System;
using System.Collections.Generic;
using System.Linq;

namespace Openbrew.Web.Models
{
	public class BrowseRecipesViewModel
	{
		/// <summary>
		/// Gets or sets the Page
		/// </summary>
		public int? Page { get; set; }

		/// <summary>
		/// Gets or sets the list of Bjcp Categories
		/// </summary>
		public IList<BjcpCategoryViewModel> BjcpCategories { get; set; }

		/// <summary>
		/// Gets or sets the RecipeCount
		/// </summary>
		public int RecipeCount { get; set; }

		/// <summary>
		/// Gets or sets the UnCategorizedRecipeCount
		/// </summary>
		public int UnCategorizedRecipeCount { get; set; }
	}
}