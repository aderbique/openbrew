using System;
using System.Linq;

namespace Openbrew.Web.Core.Model
{
	public class UserFeedback
	{
		/// <summary>
		/// Gets or sets the FeedbackId
		/// </summary>
		public int UserFeedbackId { get; set; }

		/// <summary>
		/// Gets or sets the UserId
		/// </summary>
		public int? UserId { get; set; }

		/// <summary>
		/// Gets or sets the User
		/// </summary>
		public User User { get; set; }

		/// <summary>
		/// Gets or sets the Feedback
		/// </summary>
		public string Feedback { get; set; }

		/// <summary>
		/// Gets or sets the UserHostAddress
		/// </summary>
		public string UserHostAddress { get; set; }

		/// <summary>
		/// Gets or sets the DateCreated
		/// </summary>
		public DateTime DateCreated { get; set; }

		/// <summary>
		/// Gets or sets the DateResponded
		/// </summary>
		public DateTime? DateResponded { get; set; }

		/// <summary>
		/// Gets or sets the RespondedBy
		/// </summary>
		public int? RespondedBy { get; set; }
	}
}