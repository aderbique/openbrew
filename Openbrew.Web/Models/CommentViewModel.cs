using System;
using Openbrew.Web.Core.Model;

namespace Openbrew.Web.Models
{
    public class CommentViewModel
    {
        /// <summary>
        /// Gets or sets the CommentText
        /// </summary>
        public string CommentText { get; set; }

        /// <summary>
        /// Gets or sets the IsActive
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the DateCreated
        /// </summary>
        public DateTime DateCreated { get; set; }

    	/// <summary>
    	/// Gets or sets the UserId
    	/// </summary>
    	public int UserId { get; set; }

    	/// <summary>
    	/// Gets or sets the UserName
    	/// </summary>
    	public string UserName { get; set; }

    	/// <summary>
		/// Gets or sets the UserAvatarUrl
    	/// </summary>
    	public string UserAvatarUrl { get; set; }
    }
}
