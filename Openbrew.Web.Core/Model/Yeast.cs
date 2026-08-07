using System;
using System.Collections.Generic;
using System.Linq;

namespace Openbrew.Web.Core.Model
{
	public class Yeast : IIngredient
	{
		/// <summary>
		/// Gets or sets the IngredientId
		/// </summary>
		public int IngredientId { get; set; }

		/// <summary>
		/// Gets or sets the ingredient type id
		/// </summary>
		public int IngredientTypeId { get { return (int)IngredientType.Yeast; } }

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
		/// Gets or sets the Attenuation
		/// </summary>
		public double Attenuation { get; set; }

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

        /// <summary>
        /// Gets or sets the country where the yeast producer is based.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the producer's official yeast catalog or product page.
        /// </summary>
        public string WebsiteUrl { get; set; }
	}
}
