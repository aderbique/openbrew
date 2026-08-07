using System;
using System.Collections.Generic;
using Openbrew.Web.Core.Model;

namespace Openbrew.Web.Core.Data
{
	public interface IBrewgrBlogRepository
	{
		/// <summary>
		/// Searches Blog Posts
		/// </summary>
		IEnumerable<BlogPost> SearchBlogPosts(string searchTerm);
	}
}