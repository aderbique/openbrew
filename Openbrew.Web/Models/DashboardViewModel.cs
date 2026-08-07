using Openbrew.Web.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Openbrew.Web.Models
{
    public class DashboardViewModel
    {
        /// <summary>
        /// Gets or sets the DashboardItems
        /// </summary>
        public IList<IDashboardItem> DashboardItems { get; set; }

        /// <summary>
        /// Gets or sets the Following
        /// </summary>
        public IList<MiniUserSummary> Following { get; set; }

        /// <summary>
        /// Gets or sets the FollowingCount
        /// </summary>
        public int FollowingCount { get; set; }

        /// <summary>
        /// Gets or sets the Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the Username
        /// </summary>
        public DashboardStatsViewModel DashboardStatsViewModel { get; set; }


    }
}