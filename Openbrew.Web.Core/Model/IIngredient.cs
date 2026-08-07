using System;
using System.Collections.Generic;
using System.Linq;

namespace Openbrew.Web.Core.Model
{
	public interface IIngredient
	{
		/// <summary>
		/// Gets or sets the Id
		/// </summary>
		int IngredientId { get; set; }

		/// <summary>
		/// Gets or sets the ingredient type id
		/// </summary>
		int IngredientTypeId { get; }

		/// <summary>
		/// Gets or sets the Name
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Gets or sets the CreatedByUserId
		/// </summary>
		int? CreatedByUserId { get; set; }

		/// <summary>
		/// Gets or sets the User
		/// </summary>
		User User { get; set; }

		/// <summary>
		/// Gets or sets the Description
		/// </summary>
		string Description { get; set; }

		/// <summary>
		/// Gets or sets the IsActive
		/// </summary>
		bool IsActive { get; set; }

		/// <summary>
		/// Gets or sets the IsPublid
		/// </summary>
		bool IsPublic { get; set; }

		/// <summary>
		/// Gets or sets the DateCreated
		/// </summary>
		DateTime DateCreated { get; set; }

		/// <summary>
		/// Gets or sets the Date Promoted
		/// </summary>
		DateTime? DatePromoted { get; set; }

		/// <summary>
		/// Gets or sets the Category
		/// </summary>
		string Category { get; set; }
	}
}