using System;
using System.Collections.Generic;

namespace Openbrew.Web.Core.Model
{
	public class IngredientGroup<TIngredientType> where TIngredientType : IIngredient
	{
		/// <summary>
		/// Gets or sets the Category
		/// </summary>
		public string Category { get; set; }

		/// <summary>
		/// Gets or sets the Ingredients
		/// </summary>
		public IList<TIngredientType> Ingredients { get; set; }
	}
}