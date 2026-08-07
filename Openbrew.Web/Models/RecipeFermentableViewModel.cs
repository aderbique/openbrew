using System;
using Openbrew.Web.Core.Model;

namespace Openbrew.Web.Models
{
	public class RecipeFermentableViewModel : RecipeIngredientViewModel
	{
		/// <summary>
		/// Gets or sets the Percentage
		/// </summary>
		public string Per { get; set; }

		/// <summary>
		/// Gets or sets the Amount
		/// </summary>
		public string Amt { get; set; }

		/// <summary>
		/// Gets or sets the Ppg
		/// </summary>
		public string Ppg { get; set; }

		/// <summary>
		/// Gets or sets the Lovibond
		/// </summary>
		public string L { get; set; }

		/// <summary>
		/// Gets or sets the Use
		/// </summary>
		public string Use { get; set; }
	}
}