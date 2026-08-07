using System;

namespace Openbrew.Web.Core.Model
{
	public class TastingNote
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
		/// Gets or sets the BrewSession
		/// </summary>
		public BrewSession BrewSession { get; set; }

		/// <summary>
		/// Gets or sets the RecipeId
		/// </summary>
		public int? RecipeId { get; set; }

		/// <summary>
		/// Gets or sets the Recipe
		/// </summary>
		public Recipe Recipe { get; set; }

		/// <summary>
		/// Gets or sets the UserId
		/// </summary>
		public int UserId { get; set; }

		/// <summary>
		/// Gets or sets the User
		/// </summary>
		public User User { get; set; }

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
		/// Gets or sets the IsPublic
		/// </summary>
		public bool IsPublic { get; set; }

		/// <summary>
		/// Gets or sets the IsActive
		/// </summary>
		public bool IsActive { get; set; }

		/// <summary>
		/// Gets or sets the DateCreated
		/// </summary>
		public DateTime DateCreated { get; set; }

		/// <summary>
		/// Gets or sets the DateModified
		/// </summary>
		public DateTime? DateModified { get; set; }
	}
}