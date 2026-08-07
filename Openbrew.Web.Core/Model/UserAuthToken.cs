using System;
using System.Linq;

namespace Openbrew.Web.Core.Model
{
	public class UserAuthToken
	{
		/// <summary>
		/// Gets or sets the UserAuthTokenId
		/// </summary>
		public int UserAuthTokenId { get; set; }

		/// <summary>
		/// Gets or sets the UserId
		/// </summary>
		public int UserId { get; set; }

		/// <summary>
		/// Gets or sets the User
		/// </summary>
		public User User { get; set; }

		/// <summary>
		/// Gets or sets the AuthToken
		/// </summary>
		public string AuthToken { get; set; }

		/// <summary>
		/// Gets or sets the ExpiryDate
		/// </summary>
		public DateTime ExpiryDate { get; set; }
	}
}