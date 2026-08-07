using System.Collections;
using System.Collections.Generic;

namespace Openbrew.Web.Core.Model
{
	public class RecipeSearchOptions
	{
		/// <summary>
		/// Gets or sets the RecipeTypes
		/// </summary>
		public IList<RecipeType> RecipeTypes { get; set; }

		/// <summary>
		/// Gets or sets the RecipeSty;
		/// </summary>
		public IList<string> RecipeStyles {get; set; }

		/// <summary>Catalog the selected recipe style IDs belong to.</summary>
		public string StyleCatalog { get; set; }

		/// <summary>
		/// Gets or sets the SearchTerm
		/// </summary>
		public string SearchTerm { get; set; }

		/// <summary>
		/// Gets or sets the AndSearchTerm
		/// </summary>
		public bool AndSearchTerm { get; set; }

		/// <summary>
		/// Gets or sets the IsClone
		/// </summary>
		public bool IsClone { get; set; }

		/// <summary>
		/// Gets or sets the HasBrewSessions
		/// </summary>
		public bool HasBrewSessions { get; set; }

		/// <summary>
		/// Gets or sets the HasTastingNotes
		/// </summary>
		public bool HasTastingNotes { get; set; }

		/// <summary>
		/// Gets or sets the HasComments
		/// </summary>
		public bool HasComments { get; set; }

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public RecipeSearchOptions()
		{
			this.RecipeTypes = new List<RecipeType>();
			this.RecipeStyles = new List<string>();
			this.StyleCatalog = "ba-2026";
		}
	}
}
