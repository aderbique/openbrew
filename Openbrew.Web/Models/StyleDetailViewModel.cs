using System;
using System.Collections.Generic;
using System.Linq;
using Openbrew.Web.Core.Model;

namespace Openbrew.Web.Models
{
	public class StyleDetailViewModel : PageableViewModel
	{
		/// <summary>
		/// Gets or sets the BjcpStyle
		/// </summary>
		public BjcpStyle BjcpStyle { get; set; }

		/// <summary>
		/// Gets or sets the Recipes
		/// </summary>
		public IList<RecipeSummary> Recipes { get; set; }

		/// <summary>
		/// Gets or sets the top rated recipes
		/// </summary>
		public IList<RecipeSummary> TopRatedRecipes { get; set; }
	}
}