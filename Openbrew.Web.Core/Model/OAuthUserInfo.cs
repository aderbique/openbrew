using System;
using System.Linq;

namespace Openbrew.Web.Core.Model
{
	public class OAuthUserInfo
	{
		/// <summary>
		/// Gets or sets the OAuthUserId
		/// </summary>
		public string OAuthUserId { get; set; }

		/// <summary>
		/// Gets or sets the EmailAddress
		/// </summary>
		public string EmailAddress { get; set; }

		/// <summary>
		/// Gets or sets the FirstName
		/// </summary>
		public string FirstName { get; set; }

		/// <summary>
		/// Gets or sets the LastName
		/// </summary>
		public string LastName { get; set; }

		/// <summary>
		/// Gets or sets the SourceProvider
		/// </summary>
		public OAuthProvider SourceProvider { get; set; }
	}
}