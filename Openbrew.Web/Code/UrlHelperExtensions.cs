using System;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using ctorx.Core.Formatting;
using ctorx.Core.Ninject;
using Openbrew.Web.Core.Configuration;

namespace Openbrew.Web
{
	public static class UrlHelperExtensions
	{
		/// <summary>
		/// Creates a User Profile Url
		/// </summary>
		public static string UserProfileUrl(this UrlHelper urlHelper, string userName)
		{
			var adjustedUserName = userName.Replace(" ", "-");
			return urlHelper.Action("UserProfile", "User", new { userName = adjustedUserName }, urlHelper.Https());
		}

		/// <summary>
		/// Creates a Recipe Detail Url
		/// </summary>
		public static string RecipeDetailUrl(this UrlHelper urlHelper, int recipeId, string recipeName, string styleName)
		{
			// NOTE: This code duplicated in Openbrew.Web.Core.Model.RecipeUrlBuilder
			if (!string.IsNullOrWhiteSpace(styleName) && styleName.ToLower() != "unknown style")
			{
				recipeName += "-" + StringCleaner.CleanForUrl(styleName);
			}

			if (!recipeName.ToLower().Trim().EndsWith("recipe"))
			{
				recipeName += "-recipe";
			}

			return urlHelper.Action("RecipeDetail", "Recipe", new { recipeId, recipename = StringCleaner.CleanForUrl(recipeName) }, null);
		}

		/// <summary>
		/// Creates a Recipe Edit Url
		/// </summary>
		public static string RecipeEditUrl(this UrlHelper urlHelper, int recipeId)
		{
			return urlHelper.Action("RecipeEdit", "Recipe", new { recipeId }, urlHelper.Https());
		}

		/// <summary>
		/// Creates a Brew Clone Url
		/// </summary>
		public static string RecipeCloneUrl(this UrlHelper urlHelper, int recipeId)
		{
			return urlHelper.Action("RecipeClone", "Recipe", new { recipeId }, urlHelper.Https());
		}


		/// <summary>
		/// Creates a Brew Print Url
		/// </summary>
		public static string RecipePrintUrl(this UrlHelper urlHelper, int recipeId)
		{
			return urlHelper.Action("RecipePrint", "Recipe", new { recipeId }, urlHelper.Https());
		}

		/// <summary>
		/// Creates a new Recipe Brew Url
		/// </summary>
		public static string NewBrewSessionUrl(this UrlHelper urlHelper, int recipeId)
		{
			return urlHelper.Action("NewBrewSession", "BrewSession", new { recipeId }, null);
		}

		/// <summary>
		/// Creates a recipe brew edit url
		/// </summary>
		public static string EditBrewSessionUrl(this UrlHelper urlHelper, int brewSessionId)
		{
			return urlHelper.Action("BrewSessionEdit", "BrewSession", new { brewSessionId }, null);
		}

		/// <summary>
		/// Creates a recipe brew detail url
		/// </summary>
		public static string BrewSessionDetailUrl(this UrlHelper urlHelper, int brewSessionId, string recipeName)
		{
			return urlHelper.Action("BrewSessionDetail", "BrewSession", new { brewSessionId = brewSessionId, recipename = StringCleaner.CleanForUrl(recipeName) }, null);
		}

		/// <summary>
		/// Creates a recipe beer xml url
		/// </summary>
		public static string RecipeBeerXmlUrl(this UrlHelper urlHelper, int recipeId)
		{
			return urlHelper.Action("BeerXml", "Recipe", new { recipeId });
		}


		/// <summary>
		/// Creates a Recipe Detail Url
		/// </summary>
		public static string RecipeDeleteUrl(this UrlHelper urlHelper, int recipeId)
		{
			return urlHelper.Action("RecipeDelete", "Recipe", new { recipeId }, urlHelper.Https());
		}

		/// <summary>
		/// Creates a Recipe Brew Sessions link
		/// </summary>
		public static string RecipeBrewSessionsUrl(this UrlHelper urlHelper, int recipeId, string recipeName)
		{
			return urlHelper.Action("RecipeBrewSessions", "Recipe", new { recipeId = recipeId, recipeName = StringCleaner.CleanForUrl(recipeName) }, null);
		}

		/// <summary>
		/// Creates a Recipe Detail Image Url
		/// </summary>
		public static string RecipeDetailImageUrl(this UrlHelper urlHelper, string recipeImageUrlRoot, double? srm)
		{
			// No Root, use Default
			if (string.IsNullOrWhiteSpace(recipeImageUrlRoot))
			{
				return GetDefaultRecipeImageUrl(urlHelper, srm, false);
			}

			var mediaRootUrl = GetMediaRoot(urlHelper);
			return mediaRootUrl + recipeImageUrlRoot + "_d.jpg";
		}

		/// <summary>
		/// Creates a Recipe Thumbnail Url
		/// </summary>
		public static string RecipeThumbnailUrl(this UrlHelper urlHelper, string recipeImageUrlRoot, double? srm)
		{
			// No Root, use Default
			if (string.IsNullOrWhiteSpace(recipeImageUrlRoot))
			{
				return GetDefaultRecipeImageUrl(urlHelper, srm, true);
			}

			var mediaRootUrl = GetMediaRoot(urlHelper);
			return mediaRootUrl + recipeImageUrlRoot + "_t.jpg";
		}

		/// <summary>
		/// Gets a recipe image for the default image based on srm
		/// </summary>
		static string GetDefaultRecipeImageUrl(this UrlHelper urlHelper, double? srm, bool isForThumbnail = true)
		{
			// Get Actual Srm Value for Image
			var srmValue = 1;
			if (srm.HasValue && srm.Value > 0)
			{
				srmValue = Convert.ToInt16(Math.Round(srm.Value));
				if (srmValue > 41)
				{
					srmValue = 41;
				}
			}

			var webSettings = GetWebSettings();
			var mediaRootUrl = GetMediaRoot(urlHelper);
			return mediaRootUrl + webSettings.DefaultRecipeImageRoot + srmValue + (isForThumbnail ? "_t" : "_d") + ".jpg";
		}

		/// <summary>
		/// Creates a Style Detail Url
		/// </summary>
		public static string StyleDetailUrl(this UrlHelper urlHelper, string urlFriendlyName, int? page = null)
		{
			if (urlFriendlyName.ToLower().IndexOf("-recipes") == -1)
			{
				urlFriendlyName = urlFriendlyName + "-recipes";
			}

			return urlHelper.Action("StyleDetail", "Recipe", new { urlFriendlyName, page }, null);
		}

		/// <summary>
		/// Creates a Fermentable Detail Url
		/// </summary>
		public static string FermentableDetailUrl(this UrlHelper urlHelper, int fermentableId, string name)
		{
			return urlHelper.Action("FermentableDetail", "Ingredient", new { ingredientId = fermentableId, name = StringCleaner.CleanForUrl(name) }, null);
		}

		/// <summary>
		/// Creates a Hop Detail Url
		/// </summary>
		public static string HopDetailUrl(this UrlHelper urlHelper, int hopId, string name)
		{
			return urlHelper.Action("HopDetail", "Ingredient", new { ingredientId = hopId, name = StringCleaner.CleanForUrl(name) }, null);
		}

		/// <summary>
		/// Creates a Yeast Detail Url
		/// </summary>
		public static string YeastDetailUrl(this UrlHelper urlHelper, int yeastId, string name)
		{
			return urlHelper.Action("YeastDetail", "Ingredient", new { ingredientId = yeastId, name = StringCleaner.CleanForUrl(name) }, null);
		}

		/// <summary>
		/// Creates a Adjunct Detail Url
		/// </summary>
		public static string AdjunctDetailUrl(this UrlHelper urlHelper, int adjunctId, string name)
		{
			return urlHelper.Action("AdjunctDetail", "Ingredient", new { ingredientId = adjunctId, name = StringCleaner.CleanForUrl(name) }, null);
		}

		/// <summary>
		/// Creates an affiliate product link
		/// </summary>
		public static string AffiliateProductUrl(string productId)
		{
			// Only uses Midwest at this time
			return string.Format("http://www.jdoqocy.com/click-7028218-11009678?url=https%3A%2F%2Fmidwestsupplies.affiliatetechnology.com%2Fredirect.php%3Fnt_id%3D3%26URL%3Dhttp%3A%2F%2Fwww.midwestsupplies.com%2F{0}.html%3Fref%3D1&cjsku=3030A",
				productId);
		}

		/// <summary>
		/// Generates the url for an image
		/// </summary>
		public static string Image(this UrlHelper urlHelper, string relativeImagePath)
		{
			return urlHelper.Content("~/" + relativeImagePath.Replace("~/", ""));
		}

		/// <summary>
		/// Generates a Secure Content Url
		/// </summary>
		public static string SecureContent(this UrlHelper urlHelper, string relativeContentUrl)
		{
			var webSettings = GetWebSettings();

			return string.Format("{0}/{1}", webSettings.RootPathSecure, relativeContentUrl.Replace("~/", ""));
		}

		/// <summary>
		/// Determines whether to use https
		/// </summary>
		public static string Https(this UrlHelper urlHelper)
		{
			// Internal links must inherit the browser's public origin instead of
			// rebuilding an absolute URL from XSP's internal 8085 request context.
			return null;
		}

		/// <summary>
		/// Gets the Root Url
		/// </summary>
		public static string RootUrl(this UrlHelper urlHelper, bool https = false)
		{
			var webSettings = GetWebSettings();
			var rootPath = https && !webSettings.DisableHttps ? webSettings.RootPathSecure : webSettings.RootPath;
			return rootPath.TrimEnd('/');
		}

		/// <summary>
		/// Gets Web Settings
		/// </summary>
		static IWebSettings GetWebSettings()
		{
			var kernel = KernelPersister.Get();
			var webSettings = kernel.GetService(typeof(IWebSettings)) as IWebSettings;
			return webSettings;
		}

		/// <summary>
		/// Determines if the current request is Https
		/// </summary>
		static bool IsHttps(UrlHelper urlHelper)
		{
			return urlHelper.RequestContext.HttpContext.Request.IsSecureConnection;
		}

		/// <summary>
		/// Gets the Media Root Url (secure or insecure)
		/// </summary>
		static string GetMediaRoot(UrlHelper urlHelper)
		{
			var webSettings = GetWebSettings();

			return IsHttps(urlHelper) ? webSettings.MediaUrlRootSecure : webSettings.MediaUrlRoot;
		}
	}
}
