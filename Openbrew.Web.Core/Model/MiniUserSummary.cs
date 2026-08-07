using System;

namespace Openbrew.Web.Core.Model
{
	public class MiniUserSummary
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
		/// Gets the Avatar
		/// </summary>
		public string GetAvatar(int size)
		{
			return UserAvatar.GetAvatar(size, this.EmailAddress);
		}
	}
}