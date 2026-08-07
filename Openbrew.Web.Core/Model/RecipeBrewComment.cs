using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Openbrew.Web.Core.Model
{
	public class BrewSessionComment
    {
        /// <summary>
        /// Gets or sets the CommentId
        /// </summary>
        public int BrewSessionCommentId { get; set; }

        /// <summary>
        /// Gets or sets the UserId
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the RecipeId
        /// </summary>
        public int BrewSessionId { get; set; }

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
        /// Gets or sets the User
        /// </summary>
        public virtual User User { get; set; }
    }
}
