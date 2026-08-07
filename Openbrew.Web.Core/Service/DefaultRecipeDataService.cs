using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Caching;
using Openbrew.Web.Core.Data;
using Openbrew.Web.Core.Model;
using ctorx.Core.Collections;
using ctorx.Core.Data;

namespace Openbrew.Web.Core.Service
{
	public class DefaultRecipeDataService : IRecipeDataService
	{
		readonly IBrewgrRepository Repository;
		readonly ICachingService CachingService;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public DefaultRecipeDataService(IBrewgrRepository repository, ICachingService cachingService)
		{
			this.Repository = repository;
			this.CachingService = cachingService;
		}

		/// <summary>
		/// Gets a list of public ingredients by type
		/// </summary>
		public IList<TIngredient> GetPublicIngredients<TIngredient>() where TIngredient : class, IIngredient
		{
			var cacheKey = string.Concat("Public", typeof(TIngredient).Name, "s");

			var func = new Func<IList<TIngredient>>(() => this.Repository.GetSet<TIngredient>()
				.Where(x => x.IsActive)
				.Where(x => x.IsPublic)
				.OrderBy(x => x.Name)
				.ToList());

			return this.CachingService.Get(cacheKey, 
				SlidingCacheExpirationSettings.Make(20, CacheItemPriority.High), func);
		}

		/// <summary>
		/// Gets a list of non public ingredients
		/// </summary>
		public IList<TIngredient> GetNonPublicIngredients<TIngredient>(int? count = null) where TIngredient : class, IIngredient
		{
			var query = this.Repository.GetSet<TIngredient>()
				.Include(x => x.User)
				.Where(x => !x.IsPublic)
				.Where(x => x.IsActive);

			if (count != null)
			{
				query = query.Take(count.Value);
			}

			return query.OrderByDescending(x => x.DateCreated).ToList();
		}

		/// <summary>
		/// Gets a list of usable ingredients for a user
		/// </summary>
		public IList<TIngredient> GetUsableIngredients<TIngredient>(int? userId) where TIngredient : class, IIngredient
		{
			var publicIngredients = this.GetPublicIngredients<TIngredient>();

			var userIngredients = this.Repository.GetSet<TIngredient>()
				.Where(x => x.IsActive)
				.Where(x => !x.IsPublic && (userId != null && x.CreatedByUserId == userId))
				.ToList();

			return publicIngredients.Union(userIngredients).ToList();
		}

		/// <summary>
		/// Finds an Ingredient Id
		/// </summary>
		public int? FindIngredientId(IIngredient ingredient, int userId)
		{
			if(ingredient == null)
			{
				throw new ArgumentNullException("ingredient");
			}

			var set = this.Repository.GetSet(ingredient.GetType()) as IQueryable<IIngredient>;

			var match = set
				.Where(x => x.Name == ingredient.Name)
				.Where(x => x.IsActive)
				.Where(x => (x.IsPublic || (!x.IsPublic && x.CreatedByUserId == userId)))
				.Select(x => x.IngredientId)
				.FirstOrDefault();

			return match > 0 ? (int?)match : null;
		}

		/// <summary>
		/// Promotes a custom ingredient
		/// </summary>
		public void PromoteCustomIngredient<TIngredient>(int ingredientId, string category) where TIngredient : class, IIngredient
		{
			if (ingredientId <= 0)
			{
				throw new ArgumentOutOfRangeException("ingredientId");
			}

			var ingredient = this.Repository.GetSet<TIngredient>()
				.FirstOrDefault(x => x.IngredientId == ingredientId);

			if (ingredient == null)
			{
				throw new InvalidOperationException("The ingredient does not exist");
			}

			ingredient.IsPublic = true;
			ingredient.DatePromoted = DateTime.Now;

			if (!string.IsNullOrWhiteSpace(category))
			{
				ingredient.Category = category;
			}
		}

		/// <summary>
		/// Gets Ingredient Categories
		/// </summary>
		public IList<IngredientCategory> GetIngredientCategories()
		{
			var func = new Func<IList<IngredientCategory>>(() =>
			{
				return this.Repository.GetSet<IngredientCategory>()
					.OrderBy(x => x.IngredientTypeId)
					.ThenBy(x => x.Rank)
					.ToList();
			});

			return this.CachingService.Get("IngredientCategories",
				SlidingCacheExpirationSettings.Make(20, CacheItemPriority.High), func);
		}

		/// <summary>
		/// Gets all Public Ingredients
		/// </summary>
		public IList<IIngredient> GetAllPublicIngredients()
		{
			return this.GetPublicIngredients<Fermentable>()
				.Union<IIngredient>(this.GetPublicIngredients<Hop>())
				.Union(this.GetPublicIngredients<Yeast>())
				.Union(this.GetPublicIngredients<Adjunct>())
				.Union(this.GetPublicIngredients<MashStep>())
				.ToList();
		}

		/// <summary>
		/// Gets the top X most used fermentables
		/// </summary>
		public IList<int> GetTopFermentableIds(int count = 10)
		{
			return this.Repository.GetSet<RecipeFermentable>()
				.Where(x => x.Fermentable.IsActive && x.Fermentable.IsPublic)
				.Where(x => x.Recipe.IsActive && x.Recipe.IsPublic)
				.GroupBy(x => x.IngredientId).Select(grp => new { grp.Key, Count = grp.Count() })
				.OrderByDescending(x => x.Count)
				.Select(x => x.Key)
				.Take(count)
				.ToList();
		}

		/// <summary>
		/// Gets the top X most used hops
		/// </summary>
		/// <returns></returns>
		public IList<int> GetTopHopIds(int count = 10)
		{
			return this.Repository.GetSet<RecipeHop>()
				.Where(x => x.Hop.IsActive && x.Hop.IsPublic)
				.Where(x => x.Recipe.IsActive && x.Recipe.IsPublic)
				.GroupBy(x => x.IngredientId).Select(grp => new { grp.Key, Count = grp.Count() })
				.OrderByDescending(x => x.Count)
				.Select(x => x.Key)
				.Take(count)
				.ToList();
		}

		/// <summary>
		/// Gets the top X most used yeasts
		/// </summary>
		public IList<int> GetTopYeastIds(int count = 10)
		{
			return this.Repository.GetSet<RecipeYeast>()
				.Where(x => x.Yeast.IsActive && x.Yeast.IsPublic)
				.Where(x => x.Recipe.IsActive && x.Recipe.IsPublic)
				.GroupBy(x => x.IngredientId).Select(grp => new { grp.Key, Count = grp.Count() })
				.OrderByDescending(x => x.Count)
				.Select(x => x.Key)
				.Take(count)
				.ToList();
		}

		/// <summary>
		/// Gets the top X most used adjuncts
		/// </summary>
		public IList<int> GetTopAdjunctIds(int count = 10)
		{
			return this.Repository.GetSet<RecipeAdjunct>()
				.Where(x => x.Adjunct.IsActive && x.Adjunct.IsPublic)
				.Where(x => x.Recipe.IsActive && x.Recipe.IsPublic)
				.GroupBy(x => x.IngredientId).Select(grp => new { grp.Key, Count = grp.Count() })
				.OrderByDescending(x => x.Count)
				.Select(x => x.Key)
				.Take(count)
				.ToList();
		}

		/// <summary>
		/// Gets a fermentable by Id
		/// </summary>
		public TIngredient GetIngredientById<TIngredient>(int ingredientId) where TIngredient : class, IIngredient
		{
			if (ingredientId <= 0)
			{
				throw new ArgumentOutOfRangeException("ingredientId");
			}

			return this.Repository.GetSet<TIngredient>()
				.Include(x => x.User)
				.Where(x => x.IngredientId == ingredientId)
				.Where(x => x.IsActive && x.IsPublic)
				.FirstOrDefault();
		}
	}
}