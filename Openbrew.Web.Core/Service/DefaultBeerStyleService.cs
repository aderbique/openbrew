using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Caching;
using Openbrew.Web.Core.Configuration;
using Openbrew.Web.Core.Data;
using Openbrew.Web.Core.Model;
using ctorx.Core.Collections;
using ctorx.Core.Data;
using ctorx.Core.Linq;
using System.Xml;
using System.Text;

namespace Openbrew.Web.Core.Service
{
	public class DefaultBeerStyleService : IBeerStyleService
	{
		readonly IBrewgrRepository Repository;
		readonly ICachingService CachingService;
		readonly IWebSettings WebSettings;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public DefaultBeerStyleService(IBrewgrRepository repository, ICachingService cachingService, IWebSettings webSettings)
		{
			this.Repository = repository;
			this.CachingService = cachingService;
			this.WebSettings = webSettings;
		}

		/// <summary>
        /// Gets a list of all selectable beer styles
        /// </summary>
		public IList<BjcpStyleSummary> GetStyleSummaries()
        {
			var func = new Func<IList<BjcpStyleSummary>>(() =>
			{
				var queryable = this.Repository.GetSet<BjcpStyleSummary>()
					.OrderBy(x => x.SubCategoryName);

				return queryable.ToList();
			});

			return this.CachingService.Get("AllSelectableBjcpStyles", 
				NeverExpiresCacheExpirationSettings.Make(CacheItemPriority.NotRemovable), func);
        }

		/// <summary>
		/// Gets a Bjcp style
		/// </summary>
		public BjcpStyle GetStyleByUrlFriendlyName(string urlFriendlyName)
		{
			if (string.IsNullOrWhiteSpace(urlFriendlyName))
			{
				throw new ArgumentNullException("urlFriendlyName");
			}

			return this.Repository.GetSet<BjcpStyleUrlFriendlyName>()
				.Include(x => x.BjcpStyle)
				.Where(x => x.UrlFriendlyName == urlFriendlyName)
				.Select(x => x.BjcpStyle)
				.FirstOrDefault();
		}

		/// <summary>
		/// Gets a Bjcp style by sub category Id
		/// </summary>
		public BjcpStyle GetStyleBySubCategoryId(string subCategoryId)
		{
			if (string.IsNullOrWhiteSpace(subCategoryId))
			{
				throw new ArgumentNullException("subCategoryId");
			}

			return this.Repository.GetSet<BjcpStyle>()
				.FirstOrDefault(x => x.SubCategoryId == subCategoryId);
		}

		/// <summary>
		/// Gets a page of style recipes
		/// </summary>
		public IPagedList<RecipeSummary> GetStyleRecipesPage(string subCategoryId, Pager pager)
		{
			if(pager == null)
			{
				throw new ArgumentNullException("pager");
			}

			var query =
				this.Repository.GetSet<RecipeSummary>()
				.Where(x => x.IsActive)
				.Where(x => x.IsPublic)
				.Where(x => x.BjcpStyleSubCategoryId == subCategoryId)
				.OrderByDescending(x => x.DateModified ?? x.DateCreated);

			return pager.Page(query);
		}

		/// <summary>
		/// Gets the count of uncategorized Recipes
		/// </summary>
		public int GetUnCategorizedRecipeCount()
		{
			return this.Repository.GetSet<Recipe>()
				.Where(x => x.IsActive)
				.Where(x => x.IsPublic)
				.Count(x => x.BjcpStyleSubCategoryId == null);
		}

		/// <summary>
		/// Gets a page of uncategorized Recipes 
		/// </summary>
		public IPagedList<RecipeSummary> GetUnCategorizedRecipesPage(Pager pager)
		{
			if (pager == null)
			{
				throw new ArgumentNullException("pager");
			}

			var query =
				this.Repository.GetSet<RecipeSummary>()
				.Where(x => x.BjcpStyleSubCategoryId == null)
				.Where(x => x.IsActive)
				.Where(x => x.IsPublic)
				.OrderByDescending(x => x.DateModified ?? x.DateCreated);

			return pager.Page(query);
		}

		/// <summary>
		/// Gets the number of pages for a given style
		/// </summary>
		public int GetStylePageCount(string subCategoryId)
		{
			var styleCount = this.Repository.GetSet<Recipe>()
				.Where(x => x.BjcpStyleSubCategoryId == subCategoryId)
				.Where(x => x.IsActive)
				.Where(x => x.IsPublic)
				.Count();

			return Convert.ToInt32(Math.Ceiling(styleCount * 1.00 / this.WebSettings.DefaultRecipesPerPage * 1.00));
		}

		/// <summary>
		/// Gets the number of pages of uncategorized recipes
		/// </summary>
		public int GetUnCategorizedRecipesPageCount()
		{
			var uncategorizedRecipeCount = this.GetUnCategorizedRecipeCount();
			return Convert.ToInt32(Math.Ceiling(uncategorizedRecipeCount * 1.00 / this.WebSettings.DefaultRecipesPerPage * 1.00));
		}

        /// <summary>
        /// Update the styles
        /// </summary>
        public void UpdateStyles(IList<BjcpStyle> bjcpStyles)
        {
            var bjcpStylesInRepository = this.Repository.GetSet<BjcpStyle>().ToList();

            foreach (BjcpStyle bjcpStyle in bjcpStyles)
            {
                if (bjcpStylesInRepository.FirstOrDefault(x => x.SubCategoryId == bjcpStyle.SubCategoryId) != null)
                {
                    var styleThatNeedsToBeUpdated = bjcpStylesInRepository.FirstOrDefault(x => x.SubCategoryId == bjcpStyle.SubCategoryId);
                    styleThatNeedsToBeUpdated.Abv_High = bjcpStyle.Abv_High;
                    styleThatNeedsToBeUpdated.Abv_Low = bjcpStyle.Abv_Low;
                    styleThatNeedsToBeUpdated.Appearance = bjcpStyle.Appearance;
                    styleThatNeedsToBeUpdated.Aroma = bjcpStyle.Aroma;
                    //styleThatNeedsToBeUpdated.BjcpStyleUrlFriendlyName = bjcpStyle.BjcpStyleUrlFriendlyName;
                    styleThatNeedsToBeUpdated.CategoryId = bjcpStyle.CategoryId;
                    styleThatNeedsToBeUpdated.Class = bjcpStyle.Class;
                    styleThatNeedsToBeUpdated.Comments = bjcpStyle.Comments;
                    styleThatNeedsToBeUpdated.Examples = bjcpStyle.Examples;
                    styleThatNeedsToBeUpdated.Fg_High = bjcpStyle.Fg_High;
                    styleThatNeedsToBeUpdated.Fg_Low = bjcpStyle.Fg_Low;
                    styleThatNeedsToBeUpdated.Flavor = bjcpStyle.Flavor;
                    styleThatNeedsToBeUpdated.Ibu_High = bjcpStyle.Ibu_High;
                    styleThatNeedsToBeUpdated.Ibu_Low = bjcpStyle.Ibu_Low;
                    styleThatNeedsToBeUpdated.Impression = bjcpStyle.Impression;
                    styleThatNeedsToBeUpdated.Ingredients = bjcpStyle.Ingredients;
                    styleThatNeedsToBeUpdated.Mouthfeel = bjcpStyle.Mouthfeel;
                    styleThatNeedsToBeUpdated.Og_High = bjcpStyle.Og_High;
                    styleThatNeedsToBeUpdated.Og_Low = bjcpStyle.Og_Low;
                    //styleThatNeedsToBeUpdated.Recipes = bjcpStyle.Recipes;
                    styleThatNeedsToBeUpdated.Srm_High = bjcpStyle.Srm_High;
                    styleThatNeedsToBeUpdated.Srm_Low = bjcpStyle.Srm_Low;
                    //styleThatNeedsToBeUpdated.SubCategoryName = bjcpStyle.SubCategoryName;
                }
                else
                {
                    this.Repository.Add<BjcpStyle>(bjcpStyle);
                }
            }
        }

        /// <summary>
        /// Creates a list of bjcpStyles from an XML doc
        /// </summary>
        public IList<BjcpStyle> GetStylesFromXMLDoc(XmlDocument document)
        {
            var bjcpStyles = new List<BjcpStyle>();

            foreach (XmlNode currentClass in document.SelectNodes("//styleguide//class"))
            {
                var currentClassType = currentClass.Attributes["type"].Value;

                if (currentClassType != "")
                {
                    foreach (XmlNode category in currentClass.SelectNodes("category"))
                    {
                        var categoryid = Int32.Parse(category.Attributes["id"].Value);
                        var categoryname = category.SelectSingleNode("name").InnerText;

                        foreach (XmlNode subcategory in category.SelectNodes("subcategory"))
                        {
                            var subcategoryid = (subcategory.Attributes["id"].Value != null) ? subcategory.Attributes["id"].Value : "";
                            var subcategoryname = (subcategory.SelectSingleNode("name") != null) ? subcategory.SelectSingleNode("name").InnerText : "";
                            var subcategoryaroma = (subcategory.SelectSingleNode("aroma") != null) ? subcategory.SelectSingleNode("aroma").InnerText : "";
                            var subcategoryappearance = (subcategory.SelectSingleNode("appearance") != null) ? subcategory.SelectSingleNode("appearance").InnerText : "";
                            var subcategoryflavor = (subcategory.SelectSingleNode("flavor") != null) ? subcategory.SelectSingleNode("flavor").InnerText : "";
                            var subcategorymouthfeel = (subcategory.SelectSingleNode("mouthfeel") != null) ? subcategory.SelectSingleNode("mouthfeel").InnerText : "";
                            var subcategoryimpression = (subcategory.SelectSingleNode("impression") != null) ? subcategory.SelectSingleNode("impression").InnerText : "";
                            var subcategorycomments = (subcategory.SelectSingleNode("comments") != null) ? subcategory.SelectSingleNode("comments").InnerText : "";
                            var subcategoryingredients = (subcategory.SelectSingleNode("ingredients") != null) ? subcategory.SelectSingleNode("ingredients").InnerText : "";
                            var subcategoryexamples = (subcategory.SelectSingleNode("examples") != null) ? subcategory.SelectSingleNode("examples").InnerText : "";

                            var subcategoryoglow = (subcategory.SelectSingleNode("stats//og//low") != null) ? Double.Parse(subcategory.SelectSingleNode("stats//og//low").InnerText) : 0;
                            var subcategoryoghigh = (subcategory.SelectSingleNode("stats//og//high") != null) ? Double.Parse(subcategory.SelectSingleNode("stats//og//high").InnerText) : 0;
                            var subcategoryfglow = (subcategory.SelectSingleNode("stats//fg//low") != null) ? Double.Parse(subcategory.SelectSingleNode("stats//fg//low").InnerText) : 0;
                            var subcategoryfghigh = (subcategory.SelectSingleNode("stats//fg//high") != null) ? Double.Parse(subcategory.SelectSingleNode("stats//fg//high").InnerText) : 0;
                            var subcategoryibulow = (subcategory.SelectSingleNode("stats//ibu//low") != null) ? Int32.Parse(subcategory.SelectSingleNode("stats//ibu//low").InnerText) : 0;
                            var subcategoryibuhigh = (subcategory.SelectSingleNode("stats//ibu//high") != null) ? Int32.Parse(subcategory.SelectSingleNode("stats//ibu//high").InnerText) : 0;
                            var subcategorysrmlow = (subcategory.SelectSingleNode("stats//srm//low") != null) ? Double.Parse(subcategory.SelectSingleNode("stats//srm//low").InnerText) : 0;
                            var subcategorysrmhigh = (subcategory.SelectSingleNode("stats//srm//high") != null) ? Double.Parse(subcategory.SelectSingleNode("stats//srm//high").InnerText) : 0;
                            var subcategoryabvlow = (subcategory.SelectSingleNode("stats//abv//low") != null) ? Double.Parse(subcategory.SelectSingleNode("stats//abv//low").InnerText) : 0;
                            var subcategoryabvhigh = (subcategory.SelectSingleNode("stats//abv//high") != null) ? Double.Parse(subcategory.SelectSingleNode("stats//abv//high").InnerText) : 0;


                            var bjcpStyle = new Core.Model.BjcpStyle();
                            bjcpStyles.Add(new Core.Model.BjcpStyle
                            {
                                Class = currentClassType,
                                CategoryName = categoryname,
                                CategoryId = categoryid,
                                SubCategoryId = subcategoryid,
                                SubCategoryName = subcategoryname,
                                Aroma = subcategoryaroma,
                                Appearance = subcategoryappearance,
                                Flavor = subcategoryflavor,
                                Mouthfeel = subcategorymouthfeel,
                                Impression = subcategoryimpression,
                                Comments = subcategorycomments,
                                Ingredients = subcategoryingredients,
                                Examples = subcategoryexamples,
                                Og_Low = subcategoryoglow,
                                Og_High = subcategoryoghigh,
                                Fg_High = subcategoryfghigh,
                                Fg_Low = subcategoryfglow,
                                Srm_Low = subcategorysrmlow,
                                Srm_High = subcategorysrmhigh,
                                Ibu_Low = subcategoryibulow,
                                Ibu_High = subcategoryibuhigh,
                                Abv_Low = subcategoryabvlow,
                                Abv_High = subcategoryabvhigh
                            });
                        }
                    }
                }
            }

            return bjcpStyles;           
        }

        /// <summary>
        /// Converts a list of bjcpStyles into JSON
        /// </summary>
        public string GetJSONFromStyles(IList<BjcpStyle> bjcpStyles)
        {
            var json = new StringBuilder();
            json.Append("allStyles: [");
            json.Append("{ \"SubCategoryID\": \"0A\", \"SubCategoryName\": \"Water\", \"og_low\": 1.000, \"og_high\": 1.000, \"fg_low\": 1.000, \"fg_high\": 1.000, \"ibu_low\": 0, \"ibu_high\": 0.01, \"srm_low\": 0, \"srm_high\": 0.01, \"abv_low\": 0.0, \"abv_high\": 0.01 },");
            
            foreach(BjcpStyle bjcpStyle in  bjcpStyles)
            {
                json.Append("{\"SubCategoryID\": \"" + bjcpStyle.SubCategoryId + "\", \"SubCategoryName\": \"" + bjcpStyle.SubCategoryName + "\", \"og_low\": " + bjcpStyle.Og_Low.ToString() + ", \"og_high\": " + bjcpStyle.Og_High.ToString() + ", \"fg_low\": " + bjcpStyle.Fg_Low.ToString() + ", \"fg_high\": " + bjcpStyle.Fg_High.ToString() + ", \"ibu_low\": " + bjcpStyle.Ibu_Low.ToString() + ", \"ibu_high\": " + bjcpStyle.Ibu_High.ToString() + ", \"srm_low\": " + bjcpStyle.Srm_Low.ToString() + ", \"srm_high\": " + bjcpStyle.Srm_High.ToString() + ", \"abv_low\": " + bjcpStyle.Abv_Low.ToString() + ", \"abv_high\": " + bjcpStyle.Abv_High.ToString() + " },");
            }

            json.Append("]");

            return json.ToString();
        }

		/// <summary>
		/// Finds a style id by style name
		/// </summary>
		public BjcpStyleSummary FindStyleByStyleName(string styleName)
		{
			return this.GetStyleSummaries()
				.FirstOrDefault(x => x.SubCategoryName.ToLower() == styleName.ToLower());
		}

		/// <summary>
		/// Gets a list of the top rated recipes by style
		/// </summary>
		public IList<RecipeSummary> GetTopRatedRecipes(string subCategoryId, int count)
		{
			if (string.IsNullOrWhiteSpace(subCategoryId))
			{
				throw new ArgumentNullException("subCategoryId");
			}

			if (count <= 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}

			return this.Repository.GetSet<Recipe>()
				.Where(x => x.BjcpStyleSubCategoryId == subCategoryId)
				.Where(x => x.IsActive && x.IsPublic)
				.OrderByDescending(x => x.RecipeMetaData.AverageRating)
				.Select(x => x.RecipeSummary)
				.Take(count)
				.ToList();
		}
	}
}