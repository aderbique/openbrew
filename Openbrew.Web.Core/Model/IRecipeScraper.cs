using System;

namespace Openbrew.Web.Core.Model
{
	public interface IRecipeScraper
	{
		/// <summary>
		/// Scrapes a Recipe from a Url
		/// </summary>
		Recipe Scrape(string url);
	}
}