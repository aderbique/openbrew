using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Openbrew.Web.Core.Data;
using Openbrew.Web.Core.Model;

namespace Openbrew.Web.Core.Service
{
	public class DefaultSearchService : ISearchService
	{
		readonly IBrewgrRepository Repository;
	    readonly IBrewgrBlogConnection BrewgrBlogConnection;
	    readonly IBrewgrBlogRepository BrewgrBlogRepository;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public DefaultSearchService(IBrewgrRepository repository, IBrewgrBlogConnection brewgrBlogConnection, IBrewgrBlogRepository brewgrBlogRepository)
		{
			this.Repository = repository;
		    this.BrewgrBlogConnection = brewgrBlogConnection;
		    this.BrewgrBlogRepository = brewgrBlogRepository;
		}

		/// <summary>
		/// Performs a Search
		/// </summary>
		public SearchResult Search(string searchTerm)
		{
			if (string.IsNullOrWhiteSpace(searchTerm))
			{
				throw new ArgumentNullException("searchTerm");
			}

			IList<RecipeSummary> recipeMatches = null;
			IList<BlogPost> blogMatches = null;
			IList<UserSummary> userMatches = null;

			Parallel.Invoke(
			// Recipes
			() =>
            {
				recipeMatches = this.Repository.GetSet<RecipeSummary>()
				.Where(x => x.IsActive)
				.Where(x => x.IsPublic)
				.Where(x =>
				       x.RecipeName == searchTerm ||
				       x.RecipeName.StartsWith(searchTerm) ||
				       x.RecipeName.Contains(searchTerm) ||
				       x.Description.Contains(searchTerm) ||
					   x.BJCPStyleName.Contains(searchTerm))
				.ToList();         
            },
			// Users
			() =>
			{
				userMatches = this.Repository.GetSet<User>()
				    .Where(x => x.IsActive)
				    .Where(x =>
				            x.CalculatedUsername == searchTerm ||
							x.CalculatedUsername.Contains(searchTerm) ||
				            x.Bio.Contains(searchTerm))
					.OrderBy(x => x.CalculatedUsername.StartsWith("Brewer") ? "zzz" + x.CalculatedUsername : x.CalculatedUsername)
					.ThenByDescending(x => x.Recipes.Any() ? x.Recipes.Count : 0 )
					.ThenByDescending(x => x.BrewSessions.Any() ? x.BrewSessions.Count : 0)
					.ThenByDescending(x => x.UserConnections.Any() ? x.UserConnections.Count : 0)
					.Select(x => x.UserSummary)
				    .ToList();
			},
			// Blog Posts
			() =>
			{
                // Added to fail gracefully when DEV environment doesn't have blog connection string
			    if(!string.IsNullOrWhiteSpace(this.BrewgrBlogConnection.ConnectionString))
			    {
			        try
			        {
			            blogMatches = this.BrewgrBlogRepository.SearchBlogPosts(searchTerm).ToList();
			        }
			        catch
			        {
			            blogMatches = new List<BlogPost>();
			        }
			    }
			});

			return new SearchResult
			{
				RecipeSummaries = recipeMatches,
				UserSummaries = userMatches,
				BlogPosts = blogMatches
			};
		}
	}
}
