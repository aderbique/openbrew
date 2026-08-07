using System;

namespace Openbrew.Web.Core.Model
{
	public interface IRecipeIngredient
	{
		/// <summary>
		/// Gets or sets the FermentableId
		/// </summary>
		int IngredientId { get; set; }

		/// <summary>
		/// Gets or sets the Rank
		/// </summary>
		int Rank { get; set; }

		/// <summary>
		/// Gets the Ingredient
		/// </summary>
		IIngredient GetIngredient();

		/// <summary>
		/// Sets the Ingredient
		/// </summary>
		void SetIngredient(IIngredient ingredient);
	}
}