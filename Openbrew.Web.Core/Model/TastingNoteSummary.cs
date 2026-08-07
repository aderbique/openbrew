using System;

namespace Openbrew.Web.Core.Model
{
	public class TastingNoteSummary
	{
		/// <summary>
		/// Gets or sets the TastingNoteId
		/// </summary>
		public int TastingNoteId { get; set; }

		/// <summary>
		/// Gets or sets the BrewSessionId
		/// </summary>
		public int? BrewSessionId { get; set; }

		/// <summary>
		/// Gets or sets the RecipeId
		/// </summary>
		public int RecipeId { get; set; }

		/// <summary>
		/// Gets or sets the RecipeName
		/// </summary>
		public string RecipeName { get; set; }

		/// <summary>
		/// Gets or sets the RecipeStyleName
		/// </summary>
		public string RecipeStyleName { get; set; }

		/// <summary>
		/// Gets or sets the RecipeImage
		/// </summary>
		public string RecipeImage { get; set; }

		/// <summary>
		/// Gets or sets the RecipeSrm
		/// </summary>
		public double RecipeSrm { get; set; }

		/// <summary>
		/// Gets or sets the UserId
		/// </summary>
		public int UserId { get; set; }

		/// <summary>
		/// Gets or sets the TastingUsername
		/// </summary>
		public string TastingUsername { get; set; }

		/// <summary>
		/// Gets or sets the TastingUserEmailAddress
		/// </summary>
		public string TastingUserEmailAddress { get; set; }

		/// <summary>
		/// Gets or sets the TasteDate
		/// </summary>
		public DateTime TasteDate { get; set; }

		/// <summary>
		/// Gets or sets the Rating
		/// </summary>
		public double Rating { get; set; }

		/// <summary>
		/// Gets or sets the Notes
		/// </summary>
		public string Notes { get; set; }

		/// <summary>
		/// Gets or sets the DateCreated
		/// </summary>
		public DateTime DateCreated { get; set; }
	}
}