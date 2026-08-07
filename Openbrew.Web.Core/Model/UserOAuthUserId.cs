using System;
using System.Linq;

namespace Openbrew.Web.Core.Model
{
	public class UserOAuthUserId
	{
		/// <summary>
		/// Gets or sets the UserId
		/// </summary>
		public int UserId { get; set; }

		/// <summary>
		/// Gets or sets the OpenAuthProviderId
		/// </summary>
		public int OAuthProviderId { get; set; }

		/// <summary>
		/// Gets or sets the User
		/// </summary>
		public User User { get; set; }

		/// <summary>
		/// Gets or sets the OpenAuthUserId
		/// </summary>
		public string OAuthUserId { get; set; }
	}
}