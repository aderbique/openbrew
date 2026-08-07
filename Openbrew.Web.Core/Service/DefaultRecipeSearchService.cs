using System;
using System.Collections.Generic;
using System.Linq;
using Openbrew.Web.Core.Data;
using Openbrew.Web.Core.Model;
using ctorx.Core.Linq;

namespace Openbrew.Web.Core.Service
{
	public class DefaultRecipeSearchService : IRecipeSearchService
	{
		readonly IBrewgrRepository Repository;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public DefaultRecipeSearchService(IBrewgrRepository repository)
		{
			this.Repository = repository;
		}

		/// <summary>
		/// Searches for recipes
		/// </summary>
		public IList<RecipeSummary> SearchRecipes(RecipeSearchOptions recipeSearchOptions)
		{
			var query = this.Repository.GetSet<Recipe>()
				.Where(x => x.IsActive && x.IsPublic);

			// Recipe Types
			if(recipeSearchOptions.RecipeTypes.Any())
			{
				query = query.WhereIn(x => x.RecipeTypeId, recipeSearchOptions.RecipeTypes.Select(y => (int)y));
			}

			// Recipe Styles
			if(recipeSearchOptions.RecipeStyles.Any())
			{
				if (recipeSearchOptions.StyleCatalog == "ba-2026")
				{
					query = query.Where(x => x.StyleCatalog == "ba-2026");
					query = query.WhereIn(x => x.CatalogStyleCode, recipeSearchOptions.RecipeStyles);
				}
				else
				{
					query = query.WhereIn(x => x.BjcpStyleSubCategoryId, recipeSearchOptions.RecipeStyles);
				}
			}

			// Appears to be a Clone
			if(recipeSearchOptions.IsClone)
			{
				query = query.Where(x => !x.RecipeName.StartsWith("Clone of"));
				query = query.Where(x => x.RecipeName.Contains(" clone") || x.RecipeName.Contains("clone "));
			}

			// Has Brew Sessions
			if(recipeSearchOptions.HasBrewSessions)
			{
				query = query.Where(x => x.BrewSessions.Any(y => y.IsActive && y.IsPublic));
			}

			// Has Tasting Notes
			if(recipeSearchOptions.HasTastingNotes)
			{
				query = query.Where(x => x.RecipeMetaData.TastingNoteCount > 0);
			}

			// Has Comments
			if(recipeSearchOptions.HasComments)
			{
				query = query.Where(x => x.RecipeComments.Any(y => y.IsActive));
			}

			// Search Terms
			if(!string.IsNullOrWhiteSpace(recipeSearchOptions.SearchTerm))
			{
				var words = recipeSearchOptions.SearchTerm.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
				if(recipeSearchOptions.AndSearchTerm)
				{
					query = query.Where(x => words.All(y => x.RecipeName.Contains(y) || x.Description.Contains(y)));
				}
				else
				{
					query = query.Where(x => words.Any(y => x.RecipeName.Contains(y) || x.Description.Contains(y)));	
				}
				
			}

			// Limit Results to 100 (to show on one page for now)
			return query
				.OrderByDescending(x => x.DateCreated)
				.Take(100)
				.Select(x => x.RecipeSummary)
				.ToList();
		}
	}
}
