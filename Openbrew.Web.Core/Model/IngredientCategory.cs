using System;

namespace Openbrew.Web.Core.Model
{
	public class IngredientCategory
	{
		/// <summary>
		/// Gets or sets the IngredientTypeId
		/// </summary>
		public int IngredientTypeId { get; set; }

		/// <summary>
		/// Gets or sets the Category
		/// </summary>
		public string Category { get; set; }

		/// <summary>
		/// Gets or sets the Rank
		/// </summary>
		public int Rank { get; set; }
	}
}