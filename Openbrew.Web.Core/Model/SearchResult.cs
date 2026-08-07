using System;
using System.Collections.Generic;
using System.Linq;

namespace Openbrew.Web.Core.Model
{
	public class SearchResult
	{
		/// <summary>
		/// Gets or sets the RecipeSummaries
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
	}
}