using System;

namespace Openbrew.Web.Models
{
    public class DashboardItem : IDashboardItem
    {
        /// <summary>
        /// The Item for the dashboard
        /// </summary>
        public Object Item { get; set; }

        /// <summary>
        /// Gets and sets date created, this is here for sorting
        /// </summary>
        public DateTime DateCreated { get; set; }
    }
}