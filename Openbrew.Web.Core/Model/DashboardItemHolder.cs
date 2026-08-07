using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Openbrew.Web.Core.Model
{
    public class DashboardItemHolder
    {
        /// <summary>
        /// Gets or sets the recipe summaries
        /// </summary>
        public IList<RecipeSummary> RecipeSummaries { get; set; }

        /// <summary>
        /// Gets or sets the recipe brew summaries
        /// </summary>
        public IList<BrewSessionSummary> BrewSessionSummaries { get; set; }

	    /// <summary>
	    /// Gets or sets the TastingNoteSummaries
	    /// </summary>
		public IList<TastingNoteSummary> TastingNoteSummaries { get; set; }
    }
}
