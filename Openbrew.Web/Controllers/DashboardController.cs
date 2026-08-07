using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Openbrew.Web.Email;
using ctorx.Core.Data;
using ctorx.Core.Email;
using ctorx.Core.Messaging;
using ctorx.Core.Security;
using Openbrew.Web.Core.Configuration;
using Openbrew.Web.Core.Data;
using Openbrew.Web.Core.Model;
using Openbrew.Web.Core.Service;
using Openbrew.Web.Models;
using ctorx.Core.Web;
using AutoMapper;

namespace Openbrew.Web.Controllers
{
	[Authorize]
	[RoutePrefix("")]
	public class DashboardController : BrewgrController
	{
		readonly IRecipeService RecipeService;
		readonly IUserService UserService;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
        public DashboardController(IUserService userService, IRecipeService recipeService)
		{
			this.UserService = userService;
			this.RecipeService = recipeService;
		}

        /// <summary>
        /// Executes the View for Dashboard
        /// </summary>
        [Route("dashboard")]
        public ViewResult Dashboard()
        {
            var dashboardViewModel = new DashboardViewModel();

            // Followers
            dashboardViewModel.Following = this.UserService.GetFollowedBy(this.ActiveUser.UserId, 4).ToList();
            dashboardViewModel.FollowingCount = this.UserService.GetFollowedByCount(this.ActiveUser.UserId);

            // Username
            dashboardViewModel.Username = this.ActiveUser.Username;

            // Stats
            var dashboardStats = new DashboardStatsViewModel();
            
			var userStats6MonthsAgo = this.UserService.GetUserStats(this.ActiveUser.UserId, DateTime.Now.AddMonths(-6));
            dashboardStats.RecipeCountLast6Months = userStats6MonthsAgo.RecipeCount;
            dashboardStats.SessionCountLast6Months = userStats6MonthsAgo.SessionCount;
            dashboardStats.CommentCountLast6Months = userStats6MonthsAgo.CommentCount;
            
			var userStatsAllTime = this.UserService.GetUserStats(this.ActiveUser.UserId, DateTime.Now.AddYears(-10));
            dashboardStats.RecipeCount = userStatsAllTime.RecipeCount;
            dashboardStats.SessionCount = userStatsAllTime.SessionCount;
            dashboardStats.CommentCount = userStatsAllTime.CommentCount;
            dashboardViewModel.DashboardStatsViewModel = dashboardStats;

			// Show Message to Encourage Adding Username
	        if(!this.ActiveUser.HasCustomUsername && !this.HasMessages())
	        {
		        this.AppendMessage(new InfoMessage { Text = "You have not chosen a username.  <a href=\"/settings\">Click here</a> to choose one and customize your profile." });
	        }

            return View(dashboardViewModel);
        }


        /// <summary>
        /// Gets all users recipes and returns view
        /// </summary>
		[Route("dashboard/all/{numberToReturn?}")]
		[ActionName("all")]
        public ActionResult AllMyDashboardItems(int? numberToReturn)
        {
            if (!numberToReturn.HasValue)
            {
                numberToReturn = 10;
            }

            var searchOlderThan = (!String.IsNullOrWhiteSpace(Request["SearchOlderThan"])) ? DateTime.Parse(Server.UrlDecode(Request["SearchOlderThan"])) : DateTime.Now;

            var dashboardItemHolder = this.RecipeService.GetDashboardItems(this.ActiveUser.UserId, searchOlderThan, (int)numberToReturn);

            var dashboardViewModel = new DashboardViewModel();
            dashboardViewModel.DashboardItems = new List<IDashboardItem>();

            // Recipes
            foreach (var recipeSummary in dashboardItemHolder.RecipeSummaries)
            {
                var commentWrapperViewModel = new CommentWrapperViewModel();
                commentWrapperViewModel.CommentViewModels = Mapper.Map(recipeSummary.RecipeComments, new List<CommentViewModel>());
                commentWrapperViewModel.GenericId = recipeSummary.RecipeId;
                commentWrapperViewModel.CommentType = CommentType.Recipe;
                
				var recipeSummaryViewModel = Mapper.Map(recipeSummary, new RecipeSummaryViewModel());
                recipeSummaryViewModel.ShowAddedBy = true;
                recipeSummaryViewModel.CommentWrapperViewModel = commentWrapperViewModel;
                dashboardViewModel.DashboardItems.Add(new DashboardItem
                {
                    Item = recipeSummaryViewModel,
                    DateCreated = recipeSummaryViewModel.DateCreated
                });
            }

            // Sessions
            foreach (var brewSessionSummary in dashboardItemHolder.BrewSessionSummaries)
            {
                var commentWrapperViewModel = new CommentWrapperViewModel();
                commentWrapperViewModel.CommentViewModels = Mapper.Map(brewSessionSummary.BrewSessionComments, new List<CommentViewModel>());
                commentWrapperViewModel.GenericId = brewSessionSummary.BrewSessionId;
                commentWrapperViewModel.CommentType = CommentType.Session;
                
				var brewSessionSummaryViewModel = Mapper.Map(brewSessionSummary, new BrewSessionSummaryViewModel());
                brewSessionSummaryViewModel.CommentWrapperViewModel = commentWrapperViewModel;
                brewSessionSummaryViewModel.ShowAddedBy = true;
                dashboardViewModel.DashboardItems.Add(new DashboardItem
                {
                    Item = brewSessionSummaryViewModel,
                    DateCreated = brewSessionSummaryViewModel.DateCreated
                });
            }

			// Tasting Notes
	        foreach(var tastingNoteSummary in dashboardItemHolder.TastingNoteSummaries)
	        {
				dashboardViewModel.DashboardItems.Add(new DashboardItem
				{
					Item = tastingNoteSummary,
					DateCreated = tastingNoteSummary.DateCreated
				});
	        }

            return View("_DashboardList", dashboardViewModel);
        }

		/// <summary>
		/// Gets all users recipes and returns view
		/// </summary>
		[Route("dashboard/recipes")]
		[ActionName("recipes")]
		public ActionResult AllMyRecipes()
		{
			var dashboardViewModel = new DashboardViewModel();
			dashboardViewModel.DashboardItems = new List<IDashboardItem>();

			// Recipes
			foreach (var recipeSummary in this.RecipeService.GetUserRecipes(this.ActiveUser.UserId))
			{
				var commentWrapperViewModel = new CommentWrapperViewModel();
				commentWrapperViewModel.CommentViewModels = Mapper.Map(recipeSummary.RecipeComments, new List<CommentViewModel>());
				commentWrapperViewModel.GenericId = recipeSummary.RecipeId;
				commentWrapperViewModel.CommentType = CommentType.Recipe;
				var recipeSummaryViewModel = Mapper.Map(recipeSummary, new RecipeSummaryViewModel());
				recipeSummaryViewModel.ShowAddedBy = false;
				recipeSummaryViewModel.CommentWrapperViewModel = commentWrapperViewModel;
				dashboardViewModel.DashboardItems.Add(new DashboardItem { Item = recipeSummaryViewModel });
			}

			return View("_DashboardList", dashboardViewModel);
		}

		/// <summary>
		/// Gets all users brew sessions and returns view
		/// </summary>
		[Route("dashboard/sessions")]
		[ActionName("sessions")]
		public ActionResult MyBrewSessions()
		{
			var dashboardViewModel = new DashboardViewModel();
			dashboardViewModel.DashboardItems = new List<IDashboardItem>();

			// Sessions
			foreach (var brewSessionSummary in this.RecipeService.GetUserBrewSessions(this.ActiveUser.UserId))
			{
				var commentWrapperViewModel = new CommentWrapperViewModel();
				commentWrapperViewModel.CommentViewModels = Mapper.Map(brewSessionSummary.BrewSessionComments, new List<CommentViewModel>());
				commentWrapperViewModel.GenericId = brewSessionSummary.BrewSessionId;
				commentWrapperViewModel.CommentType = CommentType.Session;
				var brewSessionSummaryViewModel = Mapper.Map(brewSessionSummary, new BrewSessionSummaryViewModel());
				brewSessionSummaryViewModel.CommentWrapperViewModel = commentWrapperViewModel;
				brewSessionSummaryViewModel.ShowAddedBy = false;
				dashboardViewModel.DashboardItems.Add(new DashboardItem { Item = brewSessionSummaryViewModel });
			}

			return View("_DashboardList", dashboardViewModel);
		}
	}
}
