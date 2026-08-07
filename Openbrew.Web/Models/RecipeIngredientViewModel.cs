using System;

namespace Openbrew.Web.Models
{
	public class RecipeIngredientViewModel
	{
		/// <summary>
		/// Gets or sets the Id
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the IngredientId
		/// </summary>
		public string IngId { get; set; }

		/// <summary>
		/// Gets or sets the Name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the CustomName
		/// </summary>
		public string CustomName { get; set; }

		/// <summary>
		/// Gets or sets the Rank
		/// </summary>
		public string Rank { get; set; }

		/// <summary>
		/// Determines if the Ingredient is custom
		/// </summary>
		public bool IsCustom()
		{
			var parsed = 0;
			Int32.TryParse(this.IngId, out parsed);
			return parsed == 0;
		}
	}
}