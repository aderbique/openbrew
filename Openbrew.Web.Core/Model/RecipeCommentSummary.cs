using System;
using System.Linq;

namespace Openbrew.Web.Core.Model
{
	public class RecipeCommentSummary
	{
		/// <summary>
		/// Gets or sets the RecipeCommentID
		/// </summary>
		public int RecipeCommentId { get; set; }

		/// <summary>
		/// Gets or sets the UserId
		/// </summary>
		public int UserId { get; set; }

		/// <summary>
		/// Gets or sets the RecipeId
		/// </summary>
		public int RecipeId { get; set; }

		/// <summary>
		/// Gets or sets the Recipe
		/// </summary>
		public Recipe Recipe { get; set; }

		/// <summary>
		/// Gets or sets the UserName
		/// </summary>
		public string UserName { get; set; }

		/// <summary>
		/// Gets or sets the EmailAddress
		/// </summary>
		public string EmailAddress { get; set; }

		/// <summary>
		/// Gets or sets the CommentText
		/// </summary>
		public string CommentText { get; set; }

		/// <summary>
		/// Gets or sets the DateCreated
		/// </summary>
		public DateTime DateCreated { get; set; }

		/// <summary>
		/// Gets or sets the IsActive
		/// </summary>
		public bool IsActive { get; set; }

	}
}