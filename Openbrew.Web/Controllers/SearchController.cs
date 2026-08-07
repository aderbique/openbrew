using System;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Openbrew.Web.Core.Service;
using Openbrew.Web.Mappers;
using Openbrew.Web.Models;

namespace Openbrew.Web.Controllers
{
	public class SearchController : BrewgrController
	{
		readonly ISearchService SearchService;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public SearchController(ISearchService searchService)
		{
			this.SearchService = searchService;
		}

		/// <summary>
		/// Executes the View for Search
		/// </summary>
		[HttpGet]
		public ActionResult Search(string searchTerm)
		{
			if (string.IsNullOrWhiteSpace(searchTerm))
			{
				return this.Issue404();
			}

			var searchResult = this.SearchService.Search(searchTerm);

			return View(Mapper.Map(searchResult, new SearchViewModel { SearchTerm = searchTerm }));
		}
	}
}