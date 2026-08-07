using System;
using System.Collections.Generic;
using System.Linq;

namespace Openbrew.Web.Core.Model
{
	[Obsolete]
	public class RecipeBrew
	{
		/// <summary>
		/// Gets or sets the BrewSessionId
		/// </summary>
		public int RecipeBrewId { get; set; }

		/// <summary>
		/// Gets or sets the RecipeId
		/// </summary>
		public int RecipeId { get; set; }

		/// <summary>
		/// Gets or sets the BrewedBy
		/// </summary>
		public int BrewedBy { get; set; }

		/// <summary>
		/// Gets or sets the BrewedByUser
		/// </summary>
		public virtual User BrewedByUser { get; set; }

		/// <summary>
		/// Gets or sets the UnitTypeId
		/// </summary>
		public int UnitTypeId { get; set; }

		/// <summary>
		/// Gets or sets the BrewDate
		/// </summary>
		public DateTime BrewDate { get; set; }

		/// <summary>
		/// Gets or sets the Notes
		/// </summary>
		public string Notes { get; set; }

		/// <summary>
		/// Gets or sets the RecipeSummary
		/// </summary>
		public virtual RecipeSummary RecipeSummary { get; set; }

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

		/// <summary>
		/// Determines if a recipe was brewed by a specific user 
		/// </summary>
		public bool WasBrewedBy(int userId)
		{
			return this.BrewedBy == userId;
		}
	}
}