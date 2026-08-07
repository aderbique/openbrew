using System;
using System.Collections.Generic;
using System.Linq;
using Openbrew.Web.Core.Model;

namespace Openbrew.Web.Models
{
	public class SearchViewModel
	{
		/// <summary>
		/// Gets or sets the SearchTerm
		/// </summary>
		public string SearchTerm { get; set; }

		/// <summary>
		/// Gets or sets the Results
		/// </summary>
		public IList<RecipeSummary> RecipeSummaries { get; set; }

		/// <summary>
		/// Gets or sets the BlogPosts
		/// </summary>
		public IList<BlogPost> BlogPosts { get; set; }

		/// <summary>
		/// Gets or sets the UserSummaries
		/// </summary>
		public IList<UserSummary> UserSummaries { get; set; }

		/// <summary>
		/// Determines if the search has results
		/// </summary>
		public bool HasResults()
		{
			return this.RecipeSummaries.Any() || this.UserSummaries.Any() || this.BlogPosts.Any();
		}
	}
}