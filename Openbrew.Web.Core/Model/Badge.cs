using System;
using System.Collections.Generic;
using System.Linq;

namespace Openbrew.Web.Core.Model
{
	public class Badge
	{
		/// <summary>
		/// Gets or sets the BadgeId
		/// </summary>
		public int BadgeId { get; set; }

		/// <summary>
		/// Gets or sets the BadgeTypeId
		/// </summary>
		public int BadgeTypeId { get; set; }

		/// <summary>
		/// Gets or sets the BadgeName
		/// </summary>
		public string BadgeName { get; set; }

		/// <summary>
		/// Gets or sets the BadgeDescription
		/// </summary>
		public string BadgeDescription { get; set; }

		/// <summary>
		/// Gets or sets the Users
		/// </summary>
		public IList<User> Users { get; set; }

		/// <summary>
		/// Gets or sets the IsActive
		/// </summary>
		public bool IsActive { get; set; }

		/// <summary>
		/// Gets or sets the DateCreated
		/// </summary>
		public DateTime DateCreated { get; set; }
	}
}