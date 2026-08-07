using System;
using System.Collections.Generic;

namespace Openbrew.Web.Core.Model
{
	public interface IIngredientCategorizer
	{
		/// <summary>
		/// Categorizes a list of Ingredients
		/// </summary>
		IList<IngredientGroup<TIngredientType>> Categorize<TIngredientType>(IList<TIngredientType> ingredients) where TIngredientType : IIngredient;
	}
}