using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Openbrew.Web.Core.Model
{
	public class Fermentable : IIngredient
	{
		/// <summary>
		/// Gets or sets the IngredientId
		/// </summary>
		public int IngredientId { get; set; }

		/// <summary>
		/// Gets or sets the ingredient type id
		/// </summary>
		public int IngredientTypeId { get { return (int)IngredientType.Fermentable; } }

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
		/// Gets or sets the Ppg
		/// </summary>
		public int Ppg { get; set; }

		/// <summary>
		/// Gets or sets the Lovibond
		/// </summary>
		public int Lovibond { get; set; }

		/// <summary>
		/// Gets or sets the DefaultUsageTypeId
		/// </summary>
		public int DefaultUsageTypeId { get; set; }

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