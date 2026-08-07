using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Openbrew.Web.Core.Data;
using Openbrew.Web.Core.Model;

namespace Openbrew.Web.Core.Service
{
	public class DefaultAdminService : IAdminService
	{
		readonly IBrewgrRepository Repository;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public DefaultAdminService(IBrewgrRepository brewgrRepository)
		{
			this.Repository = brewgrRepository;
		}

		/// <summary>
		/// Gets Site Stats
		/// </summary>
		public SiteStats GetSiteStats()
		{
			var siteStats = new SiteStats();

			// Recipe Count
			siteStats.RecipeCount = this.Repository.GetSet<Recipe>().Count(x => x.IsActive && x.User.UserAdmin == null);
	
			// Brew Session Count
			siteStats.BrewSessionCount = this.Repository.GetSet<BrewSession>().Count(x => x.IsActive && x.BrewedByUser.UserAdmin == null);

			// User Count
			siteStats.UserCount = this.Repository.GetSet<User>().Count(x => x.IsActive && x.UserAdmin == null);

			// Comment Count
			siteStats.CommentCount = this.Repository.GetSet<RecipeComment>().Count(x => x.IsActive && x.User.UserAdmin == null);

			// Tasting Note Count
			siteStats.TastingNoteCount = this.Repository.GetSet<TastingNote>().Count(x => x.IsActive && x.IsPublic && x.User.UserAdmin == null);

			var loginDateStart = DateTime.Now.AddDays(-1);
			siteStats.LoginCountLast24Hours = this.Repository.GetSet<UserLogin>().Count(x => x.LoginDate >= loginDateStart && x.User.UserAdmin == null);

			// Keep this queue aligned with Admin Tools → Custom Ingredients: active,
			// non-public ingredients are the items that need an admin decision.
			siteStats.CustomIngredientCount =
				this.Repository.GetSet<Fermentable>().Count(x => x.IsActive && !x.IsPublic) +
				this.Repository.GetSet<Hop>().Count(x => x.IsActive && !x.IsPublic) +
				this.Repository.GetSet<Yeast>().Count(x => x.IsActive && !x.IsPublic) +
				this.Repository.GetSet<Adjunct>().Count(x => x.IsActive && !x.IsPublic);
	
			// RecentCustom Ingredients
			var customIngredients = new List<IIngredient>();
			customIngredients.AddRange(this.Repository.GetSet<Fermentable>().Include(x => x.User).Where(x => x.IsActive && !x.IsPublic).OrderByDescending(x => x.DateCreated).Take(25).ToList());
			customIngredients.AddRange(this.Repository.GetSet<Hop>().Include(x => x.User).Where(x => x.IsActive && !x.IsPublic).OrderByDescending(x => x.DateCreated).Take(25).ToList());
			customIngredients.AddRange(this.Repository.GetSet<Yeast>().Include(x => x.User).Where(x => x.IsActive && !x.IsPublic).OrderByDescending(x => x.DateCreated).Take(25).ToList());
			customIngredients.AddRange(this.Repository.GetSet<Adjunct>().Include(x => x.User).Where(x => x.IsActive && !x.IsPublic).OrderByDescending(x => x.DateCreated).Take(25).ToList());

			// The individual queries are kept small, then merged and sorted so the
			// dashboard really shows the newest custom ingredients across all types.
			siteStats.RecentCustomIngredients = customIngredients
				.OrderByDescending(x => x.DateCreated)
				.Take(25)
				.ToList();
		
			// Last 25 Users
			siteStats.Last25Users = Mapper.Map(this.Repository.GetSet<User>().Where(x => x.IsActive && x.UserAdmin == null).OrderByDescending(x => x.DateCreated).Take(25).ToList(), new List<UserSummary>());

			// Recent successful logins, including administrators. An audit view must
			// not hide the very admin accounts that use it.
			siteStats.Last25UserLogins = this.Repository.GetSet<UserLogin>()
				.Include(x => x.User)
				.Where(x => x.User.IsActive)
				.OrderByDescending(x => x.LoginDate)
				.Take(25)
				.ToList();
		
			// Last 25 Recipes
			siteStats.Last25Recipes = this.Repository.GetSet<RecipeSummary>().Where(x => x.IsActive && !x.UserIsAdmin).OrderByDescending(x => x.DateCreated).Take(25).ToList();

            // Last 25 recipe comments
            siteStats.Last25RecipeComments = this.Repository.GetSet<RecipeComment>().Where(x => x.IsActive).OrderByDescending(x => x.DateCreated).Take(25).ToList();

            // Last 25 recipe brew comments
            siteStats.Last25BrewSessionComments = this.Repository.GetSet<BrewSessionComment>().Where(x => x.IsActive).OrderByDescending(x => x.DateCreated).Take(25).ToList();

			return siteStats;
		}

		/// <summary>
		/// Resolves Feedback
		/// </summary>
		public void ResolveFeedback(int userFeedbackId, int userId)
		{
			var userFeedback = this.Repository.GetSet<UserFeedback>()
			    .FirstOrDefault(x => x.UserFeedbackId == userFeedbackId && x.DateResponded == null);

			if (userFeedback == null)
			{
				throw new InvalidOperationException("Feedback already resolved");
			}

			userFeedback.DateResponded = DateTime.Now;
			userFeedback.RespondedBy = userId;
		}
	}
}
