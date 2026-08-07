using System;
using System.Collections.Generic;
using System.Linq;
using Openbrew.Web.Core.Model;
using ctorx.Core.Collections;

namespace Openbrew.Web.Models
{
	public class UnCategorizedRecipesViewModel : PageableViewModel
	{
		/// <summary>
		/// Gets or sets the Recipes
		/// </summary>
		public IList<RecipeSummary> Recipes { get; set; }
	}
}