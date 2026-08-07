using System;
using System.Collections.Generic;
using Openbrew.Web.Core.Model;
using ctorx.Core.Collections;

namespace Openbrew.Web.Core.Service
{
	public interface IRecipeDataService
	{
		/// <summary>
		/// Gets a list of public ingredients by type
		/// </summary>
		IList<TIngredient> GetPublicIngredients<TIngredient>() where TIngredient : class, IIngredient;

		/// <summary>
		/// Gets a list of non public ingredients
		/// </summary>
		IList<TIngredient> GetNonPublicIngredients<TIngredient>(int? count = null) where TIngredient : class, IIngredient;

		/// <summary>
		/// Gets a list of usable ingredients for a user
		/// </summary>
		IList<TIngredient> GetUsableIngredients<TIngredient>(int? userId) where TIngredient : class, IIngredient;

		/// <summary>
		/// Finds an Ingredient Id
		/// </summary>
		int? FindIngredientId(IIngredient ingredient, int userId);

		/// <summary>
		/// Promotes a custom ingredient
		/// </summary>
		void PromoteCustomIngredient<TIngredient>(int ingredientId, string category) where TIngredient : class, IIngredient;

		/// <summary>
		/// Gets Ingredient Categories
		/// </summary>
		/// <returns></returns>
		IList<IngredientCategory> GetIngredientCategories();

		/// <summary>
		/// Gets all Public Ingredients
		/// </summary>
		IList<IIngredient> GetAllPublicIngredients();

		///// <summary>
		///// Gets the top X most used ingredients by type
		///// </summary>
		//IList<int> GetTopIngredientIds<TRecipeIngredient>(int count = 10) where TRecipeIngredient : class, IRecipeIngredient;

		/// <summary>
		/// Gets the top X most used fermentables
		/// </summary>
		IList<int> GetTopFermentableIds(int count = 10);

		/// <summary>
		/// Gets the top X most used hops
		/// </summary>
		/// <returns></returns>
		IList<int> GetTopHopIds(int count = 10);

		/// <summary>
		/// Gets the top X most used yeasts
		/// </summary>
		IList<int> GetTopYeastIds(int count = 10);

		/// <summary>
		/// Gets the top X most used adjuncts
		/// </summary>
		IList<int> GetTopAdjunctIds(int count = 10);

		/// <summary>
		/// Gets a fermentable by Id
		/// </summary>
		TIngredient GetIngredientById<TIngredient>(int ingredientId) where TIngredient : class, IIngredient;
	}
}