using System;
using System.Linq;

namespace Openbrew.Web.Core.Model
{
	public class UserNotificationType
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
		/// Gets or sets the NotificationTypeId
		/// </summary>
		public int NotificationTypeId { get; set; }

		/// <summary>
		/// Gets or sets the DateCreated
		/// </summary>
		public DateTime DateCreated { get; set; }
	}
}