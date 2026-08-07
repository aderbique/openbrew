using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Openbrew.Web.Models
{
    public class DashboardStatsViewModel
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

        /// <summary>
        /// Gets or sets the RecipeCountLast6Months
        /// </summary>
        public int RecipeCountLast6Months { get; set; }

        /// <summary>
        /// Gets or sets the SessionCountLast6Months
        /// </summary>
        public int SessionCountLast6Months { get; set; }

        /// <summary>
        /// Gets or sets the CommentCountLast6Months
        /// </summary>
        public int CommentCountLast6Months { get; set; }
    }
}