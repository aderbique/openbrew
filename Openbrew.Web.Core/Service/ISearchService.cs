using System.Collections.Generic;
using Openbrew.Web.Core.Model;

namespace Openbrew.Web.Core.Service
{
	public interface ISearchService
	{
		/// <summary>
		/// Performs a Search
		/// </summary>
		SearchResult Search(string searchTerm);
	}
}