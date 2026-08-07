using System;
using Openbrew.Web.Core.Model;

namespace Openbrew.Web.Models
{
	public class RecipeHopViewModel : RecipeIngredientViewModel
	{
		/// <summary>
		/// Gets or sets the AmountInOunces
		/// </summary>
		public string Amt { get; set; }

		/// <summary>
		/// Gets or sets the Type
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// Gets or sets the Use
		/// </summary>
		public string Use { get; set; }

		/// <summary>
		/// Gets or sets the TimeInMinutes
		/// </summary>
		public string Min { get; set; }

		/// <summary>
		/// Gets or sets the TimeInMinutes
		/// </summary>
		public string Day { get; set; }

		/// <summary>
		/// Gets or sets the AlphaAcidAmount
		/// </summary>
		public string AA { get; set; }

		/// <summary>
		/// Gets or sets the Ibu
		/// </summary>
		public string Ibu { get; set; }
	}
}