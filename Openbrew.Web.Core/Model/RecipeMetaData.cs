using System;

namespace Openbrew.Web.Core.Model
{
	public class RecipeMetaData
	{
		/// <summary>
		/// Gets or sets the RecipeId
		/// </summary>
		public int RecipeId { get; set; }

		/// <summary>
		/// Gets or sets the AverageRating
		/// </summary>
		public double AverageRating { get; set; }

		/// <summary>
		/// Gets or sets the TastingNoteCount
		/// </summary>
		public int TastingNoteCount { get; set; }

		/// <summary>
		/// Gets or sets the BrewSessionCount
		/// </summary>
		public int BrewSessionCount { get; set; }

		/// <summary>
		/// Gets or sets the CommentCount
		/// </summary>
		public int CommentCount { get; set; }

		/// <summary>
		/// Gets or sets the CloneCount
		/// </summary>
		public int CloneCount { get; set; }
	}
}