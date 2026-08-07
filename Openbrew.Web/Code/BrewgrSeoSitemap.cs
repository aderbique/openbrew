using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml.Linq;
using Openbrew.Web.Core.Configuration;
using Openbrew.Web.Core.Model;
using Openbrew.Web.Core.Service;
using ctorx.Core.Collections;
using ctorx.Core.Web;

namespace Openbrew.Web
{
	public class BrewgrSeoSitemap : ISeoSitemap
	{
		readonly IRecipeService RecipeService;
		readonly IBeerStyleService BeerStyleService;
		readonly IUserService UserService;

		readonly string[] StaticLinks = new[]
		{
			"http://brewgr.com",
			"http://brewgr.com/about",
			"http://brewgr.com/features",
			"https://brewgr.com/login",
			"http://brewgr.com/how-it-works",
			"http://brewgr.com/homebrew-recipes",
			"https://brewgr.com/homebrew-recipe-calculator",
			"https://brewgr.com/contact",
            "http://brewgr.com/calculations",
			"http://brewgr.com/calculations/original-gravity",
			"http://brewgr.com/calculations/final-gravity",
			"http://brewgr.com/calculations/srm-beer-color",
			"http://brewgr.com/calculations/ibu-hop-bitterness",
			"http://brewgr.com/calculations/alcohol-content",
			"http://brewgr.com/calculations/calories",
			"http://brewgr.com/calculators/hydrometer-correction",
			"http://brewgr.com/pliny-the-elder-clone-recipes"
		};

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public BrewgrSeoSitemap(IRecipeService recipeService, IBeerStyleService beerStyleService, IUserService userService)
		{
			this.RecipeService = recipeService;
			this.BeerStyleService = beerStyleService;
			this.UserService = userService;
		}

		/// <summary>
		/// Generates the Xml Sitemap
		/// </summary>
		/// <param name="urlHelper"> </param>
		public string GenerateXml(UrlHelper urlHelper)
		{
			var xml = new StringBuilder();

			xml.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
			xml.Append("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\" " + 
				"xmlns:image=\"http://www.sitemaps.org/schemas/sitemap-image/1.1\" " +
				"xmlns:video=\"http://www.sitemaps.org/schemas/sitemap-video/1.1\">");
			xml.AppendLine();

			#region STATIC LINKS 

			// Add the Static Links
			StaticLinks.ForEach(x => xml.AppendLine(this.CreateUrlString(x, new DateTime(2012, 08, 14), "weekly")));

			#endregion

			#region STYLES

			// Add the Recipe Style Detail Page Links
			var styles = this.BeerStyleService.GetStyleSummaries();

			foreach(var style in styles)
			{
				xml.AppendLine(this.CreateUrlString(urlHelper.StyleDetailUrl(style.UrlFriendlyName), DateTime.Now, "daily", "1.0"));

				var stylePageCount = this.BeerStyleService.GetStylePageCount(style.SubCategoryId);

				for(var page = 2; page <= stylePageCount; page++)
				{
					xml.AppendLine(this.CreateUrlString(urlHelper.StyleDetailUrl(style.UrlFriendlyName, page), DateTime.Now, "daily", "1.0"));
				}
			}

			#endregion

			#region UNCATEGORIZED 

			// Add the Uncategorized Pages
			//http://dev.brewgr.com/recipes/other-homebrew-recipes
			var uncategorizedPageCount = this.BeerStyleService.GetUnCategorizedRecipesPageCount();

			if(uncategorizedPageCount > 0)
			{
				xml.AppendLine(this.CreateUrlString(urlHelper.Action("other-homebrew-recipes", "Recipe", new { page = (int?)null }, "http"), DateTime.Now, "daily", "1.0"));
				if(uncategorizedPageCount > 1)
				{
					for(var page = 2; page <= uncategorizedPageCount; page++)
					{
						xml.AppendLine(this.CreateUrlString(urlHelper.Action("other-homebrew-recipes", "Recipe", new { page = page }, "http"), DateTime.Now, "daily", "1.0"));
					}
				}
			}

			#endregion

			#region RECIPE DETAIL 

			// Add the Recipe Links (this will need to be extracted when we hit thousands of Recipes)
			var recipes = this.RecipeService.GetAllRecipes();
			recipes.ForEach(x => xml.AppendLine(this.CreateUrlString(urlHelper.RecipeDetailUrl(x.RecipeId, x.RecipeName, (x.BjcpStyle != null ? x.BjcpStyle.SubCategoryName : null)), x.DateModified ?? x.DateCreated, "weekly", "1.0")));

			#endregion

			#region BREW SESSION DETAIL 

			var brewSessions = this.RecipeService.GetAllBrewSessionSummaries();
			brewSessions.ForEach(x => xml.AppendLine(this.CreateUrlString(urlHelper.BrewSessionDetailUrl(x.BrewSessionId, x.RecipeName), x.DateModified ?? x.DateCreated, "weekly", "1.0")));

			#endregion

			#region USER PROFILES

			// Add the User Profile Links (this will need to be extracted when we have a lot of users)
			var users = this.UserService.GetAllUsers();
			users.ForEach(x => xml.AppendLine(this.CreateUrlString(urlHelper.UserProfileUrl(x.CalculatedUsername), x.DateModified ?? x.DateCreated, "weekly", "0.6")));

			#endregion

			xml.Append("</urlset>");

			return xml.ToString();
		}

		/// <summary>
		/// Creates a Url String
		/// </summary>
		public string CreateUrlString(string url, DateTime lastmod, string changeFrequency = "daily", string priority = "0.5")
		{
			var builder = new StringBuilder();

			builder.AppendLine("<url>");
			builder.AppendLine("<loc>" + url + "</loc>");
			builder.AppendLine("<lastmod>" + lastmod.ToString("yyyy-MM-dd") + "</lastmod>");
			builder.AppendLine("<changefreq>" + changeFrequency + "</changefreq>");
			builder.AppendLine("<priority>" + priority + "</priority>");
			builder.AppendLine("</url>");

			return builder.ToString();
		}
	}
}
