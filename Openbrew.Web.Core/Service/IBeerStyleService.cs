using System;
using System.Collections.Generic;
using Openbrew.Web.Core.Model;
using ctorx.Core.Collections;
using System.Xml;

namespace Openbrew.Web.Core.Service
{
	public interface IBeerStyleService
	{
		/// <summary>
		/// Gets a list of all selectable beer styles
		/// </summary>
		IList<BjcpStyleSummary> GetStyleSummaries();

		/// <summary>
		/// Gets a Bjcp style by url friendly name
		/// </summary>
		BjcpStyle GetStyleByUrlFriendlyName(string urlFriendlyName);

		/// <summary>
		/// Gets a Bjcp style by sub category Id
		/// </summary>
		BjcpStyle GetStyleBySubCategoryId(string subCategoryId);

		/// <summary>
		/// Gets a page of style recipes
		/// </summary>
		IPagedList<RecipeSummary> GetStyleRecipesPage(string subCategory, Pager pagerSettings);

		/// <summary>
		/// Gets the count of uncategorized Recipes
		/// </summary>
		int GetUnCategorizedRecipeCount();

		/// <summary>
		/// Gets a page of uncategorized Recipes 
		/// </summary>
		IPagedList<RecipeSummary> GetUnCategorizedRecipesPage(Pager pager);

		/// <summary>
		/// Gets the number of pages for a given style
		/// </summary>
		int GetStylePageCount(string subCategoryId);

		/// <summary>
		/// Gets the number of pages of uncategorized recipes
		/// </summary>
		int GetUnCategorizedRecipesPageCount();

        /// <summary>
        /// Updates the BJCP Styles
        /// </summary>
        void UpdateStyles(IList<BjcpStyle> bjcpStyles);

        /// <summary>
        /// Creates a list of bjcpStyles from an XML doc
        /// </summary>
        IList<BjcpStyle> GetStylesFromXMLDoc(XmlDocument document);

        /// <summary>
        /// Converts a list of bjcpStyles into JSON
        /// </summary>
        string GetJSONFromStyles(IList<BjcpStyle> bjcpStyles);

		/// <summary>
		/// Finds a style by style name
		/// </summary>
		BjcpStyleSummary FindStyleByStyleName(string styleName);

		/// <summary>
		/// Gets a list of top rated recipes
		/// </summary>
		IList<RecipeSummary> GetTopRatedRecipes(string subCategoryId, int count);
	}
}