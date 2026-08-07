using System;
using System.Collections.Generic;

namespace Openbrew.Web.Core.Model
{
	public class MashStep : IIngredient
	{
		/// <summary>
		/// Gets or sets the IngredientId
		/// </summary>
		public int IngredientId { get; set; }

		/// <summary>
		/// Gets or sets the ingredient type id
		/// </summary>
		public int IngredientTypeId { get { return (int) IngredientType.MashStep; } }

		/// <summary>
		/// Gets or sets the CreatedByUserId
		/// </summary>
		public int? CreatedByUserId { get; set; }

		/// <summary>
		/// Gets or sets the CreatedByUser
		/// </summary>
		public User User { get; set; }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
		/// Gets or sets the IsActive
		/// </summary>
		public bool IsActive { get; set; }

		/// <summary>
		/// Gets or sets the IsPublic
		/// </summary>
		public bool IsPublic { get; set; }

		/// <summary>
		/// Gets or sets the DateCreated
		/// </summary>
		public DateTime DateCreated { get; set; }

		/// <summary>
		/// Gets or sets the Date Promoted
		/// </summary>
		public DateTime? DatePromoted { get; set; }

        /// <summary>
        /// Gets or sets the Category
        /// </summary>
        public string Category { get; set; }
	}
}
