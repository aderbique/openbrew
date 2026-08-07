using System;
using System.Linq;

namespace Openbrew.Web.Core.Model
{
	public class NewsletterSignup
	{
		/// <summary>
		/// Gets or sets the NewsletterSignupId
		/// </summary>
		public int NewsletterSignupId { get; set; }

		/// <summary>
		/// Gets or sets the EmailAddress
		/// </summary>
		public string EmailAddress { get; set; }

		/// <summary>
		/// Gets or sets the IPAddress
		/// </summary>
		public string IPAddress { get; set; }

		/// <summary>
		/// Gets or sets the Source
		/// </summary>
		public string Source { get; set; }

		/// <summary>
		/// Gets or sets the DateCreated
		/// </summary>
		public DateTime DateCreated { get; set; }

		public string ConfirmationToken { get; set; }

		public bool IsConfirmed { get; set; }

		public bool IsUnsubscribed { get; set; }

		public DateTime? DateConfirmed { get; set; }

		public DateTime? DateUnsubscribed { get; set; }
	}
}
