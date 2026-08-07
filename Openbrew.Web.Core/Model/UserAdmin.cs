using System;
using System.Linq;

namespace Openbrew.Web.Core.Model
{
	public class UserAdmin
	{
		/// <summary>
		/// Gets or sets the UserId
		/// </summary>
		public int UserId { get; set; }

		/// <summary>
		/// Gets or sets the User
		/// </summary>
		public User User { get; set; }

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