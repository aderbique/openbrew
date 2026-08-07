using System;
using System.Linq;

namespace Openbrew.Web.Core.Model
{
	public class UserNotification
	{
		/// <summary>
		/// Gets or sets the UserNotificationId
		/// </summary>
		public int UserNotificationId { get; set; }

		/// <summary>
		/// Gets or sets the UserId
		/// </summary>
		public int UserId { get; set; }

		/// <summary>
		/// Gets or sets the NotificationTypeId
		/// </summary>
		public int NotificationTypeId { get; set; }

		/// <summary>
		/// Gets or sets the TriggerIdentifier
		/// </summary>
		public int TriggerIdentifier { get; set; }

		/// <summary>
		/// Gets or sets the TargetIdentifier
		/// </summary>
		public int TargetIdentifier { get; set; }

		/// <summary>
		/// Gets or sets the DateCreated
		/// </summary>
		public DateTime DateCreated { get; set; }

		/// <summary>
		/// Gets or sets the DateRead
		/// </summary>
		public DateTime? DateRead { get; set; }
	}
} 