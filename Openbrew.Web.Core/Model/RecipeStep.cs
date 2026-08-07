using System;
using System.Linq;

namespace Openbrew.Web.Core.Model
{
	public class RecipeStep
	{
		/// <summary>
		/// Gets or sets the RecipeStepId
		/// </summary>
		public int RecipeStepId { get; set; }

		/// <summary>
		/// Gets or sets the RecipeId
		/// </summary>
		public int RecipeId { get; set; }

		/// <summary>
		/// Gets or sets the Description
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the Rank
		/// </summary>
		public int? Rank { get; set; }

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