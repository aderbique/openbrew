using System;
using System.Collections.Generic;
using System.Web;
using Openbrew.Web.Core.Model;

namespace Openbrew.Web.Models
{
	public interface IRecipeFactsViewModel
	{
		/// <summary>
		/// Gets or sets the RecipeId
		/// </summary>
		int RecipeId { get; set; }

		/// <summary>
		/// Gets the RecipeName
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the BjcpSubCategoryId
		/// </summary>
		string StyleId { get; }

		/// <summary>
		/// Gets the BjcpSubCategoryName
		/// </summary>
		string StyleName { get; }

		/// <summary>
		/// Gets the BatchSize
		/// </summary>
		double BatchSize { get; }

		/// <summary>
		/// Gets the BoilSize
		/// </summary>
		double BoilSize { get; }

		/// <summary>
		/// Gets the BoilTimeInMinutes
		/// </summary>
		int BoilTime { get; }

		/// <summary>
		/// Gets the OriginalGravity
		/// </summary>
		double Og { get; }

		/// <summary>
		/// Gets the FinalGravity
		/// </summary>
		double Fg { get; }

		/// <summary>
		/// Gets the SRM
		/// </summary>
		double Srm { get; }

		/// <summary>
		/// Gets the IBU
		/// </summary>
		double Ibu { get; }

		/// <summary>
		/// Gets the BGGU
		/// </summary>
		double BgGu { get; }

		/// <summary>
		/// Gets the ABV
		/// </summary>
		double Abv { get; }

		/// <summary>
		/// Gets the Calories
		/// </summary>
		int Calories { get; }

        /// <summary>
        /// Gets the Efficiency
        /// </summary>
        double Efficiency { get; }

		/// <summary>
		/// Gets the CreatedBy
		/// </summary>
		int CreatedBy { get; }

		/// <summary>
		/// Determines if the Recipe is a new Recipe
		/// </summary>
		bool IsNewRecipe();

		/// <summary>
		/// Gets the IbuFormula Name
		/// </summary>
		string GetIbuFormulaName();
	}
}