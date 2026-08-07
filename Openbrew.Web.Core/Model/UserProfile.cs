using System;
using System.Linq;

namespace Openbrew.Web.Core.Model
{
	public class UserProfile
	{
		/// <summary>
		/// Gets or sets the UserId
		/// </summary>
		public int UserId { get; set; }

		/// <summary>
		/// Gets or sets the Bio
		/// </summary>
		public string Bio { get; set; }

		/// <summary>
		/// Gets or sets the Birthdate
		/// </summary>
		public DateTime? Birthdate { get; set; }

		/// <summary>
		/// Gets or sets the DateStartedBrewing
		/// </summary>
		public DateTime? DateStartedBrewing { get; set; }

		/// <summary>
		/// Gets or sets the City
		/// </summary>
		public string City { get; set; }

		/// <summary>
		/// Gets or sets the StateProvince
		/// </summary>
		public string StateProvince { get; set; }

		/// <summary>
		/// Gets or sets the Country
		/// </summary>
		public string Country { get; set; }

		/// <summary>
		/// Gets or sets the DateCreated
		/// </summary>
		public DateTime DateCreated { get; set; }

		/// <summary>
		/// Gets or sets the DateModified
		/// </summary>
		public DateTime? DateModified { get; set; }
	}
}