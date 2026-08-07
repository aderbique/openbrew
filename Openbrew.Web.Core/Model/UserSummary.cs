using System;
using System.Linq;

namespace Openbrew.Web.Core.Model
{
	public class UserSummary
	{
		/// <summary>
		/// Gets or sets the UserId
		/// </summary>
		public int UserId { get; set; }

		/// <summary>
		/// Gets or sets the Username
		/// </summary>
		public string Username { get; set; }

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
        /// Gets or sets the DateCreated
        /// </summary>
        public DateTime DateCreated { get; set; }

		/// <summary>
		/// Gets or sets the RecipeCount
		/// </summary>
		public int RecipeCount { get; set; }

		/// <summary>
		/// Gets or sets the RecipeCount
		/// </summary>
		public int BrewSessionCount { get; set; }

		/// <summary>
		/// Gets or sets the CommentCount
		/// </summary>
		public int CommentCount { get; set; }

		/// <summary>
		/// Gets or sets the IsActive
		/// </summary>
		public bool IsActive { get; set; }

		/// <summary>
		/// Gets or sets the IsAdmin
		/// </summary>
		public bool IsAdmin { get; set; }

		/// <summary>
		/// Gets or sets the IsPartner
		/// </summary>
		public bool IsPartner { get; set; }

		/// <summary>
		/// Gets or sets the Bio
		/// </summary>
		public string Bio { get; set; }

		/// <summary>
		/// Gets or sets the HasCustomUsername
		/// </summary>
		public bool HasCustomUsername { get; set; }

		/// <summary>
		/// Gets or sets the User
		/// </summary>
		public User User { get; set; }

		/// <summary>
		/// Getsthe FullName
		/// </summary>
		public string FullName
		{
			get { return string.Concat(this.FirstName, " ", this.LastName); }
		}

        /// <summary>
        /// Gets the Avatar
        /// </summary>
        public string GetAvatar(int size)
        {
            return UserAvatar.GetAvatar(size, this.EmailAddress);
        }
	}
}