using System;
using System.Linq;

namespace Openbrew.Web.Core.Model
{
	public class UserStat
	{
        /// <summary>
        /// Gets or sets the RecipeCount
        /// </summary>
        public int RecipeCount { get; set; }

        /// <summary>
        /// Gets or sets the SessionCount
        /// </summary>
        public int SessionCount { get; set; }

        /// <summary>
        /// Gets or sets the CommentCount
        /// </summary>
        public int CommentCount { get; set; }
    }
}