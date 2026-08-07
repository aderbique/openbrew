using System;
using System.Linq;

namespace Openbrew.Web.Core.Model
{
	public class UserLogin
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
		/// Gets or sets the LoginDate
		/// </summary>
		public DateTime LoginDate { get; set; }

		/// <summary>
		/// Gets or sets the client IP address recorded at login time.
		/// Older login records may not have this value.
		/// </summary>
		public string IPAddress { get; set; }
	}
}
