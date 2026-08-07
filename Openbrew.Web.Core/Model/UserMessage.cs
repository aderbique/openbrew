using System;

namespace Openbrew.Web.Core.Model
{
	public class UserMessage
	{
		/// <summary>
		/// Gets or sets the UserMessageId
		/// </summary>
		public int UserMessageId { get; set; }

		/// <summary>
		/// Gets or sets the SenderId
		/// </summary>
		public int SenderId { get; set; }

		/// <summary>
		/// Gets or sets the RecipientId
		/// </summary>
		public int RecipientId { get; set; }

		/// <summary>
		/// Gets or sets the Message
		/// </summary>
		public string Message { get; set; }

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