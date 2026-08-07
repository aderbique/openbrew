using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Openbrew.Web.Core.Model;
using Openbrew.Web.Models;

namespace Openbrew.Web.Mappers
{
	public class BrowseRecipesViewModelConverter : ITypeConverter<IList<BjcpStyleSummary>, BrowseRecipesViewModel>
	{
		public BrowseRecipesViewModel Convert(ResolutionContext context)
		{
			var styleList = context.SourceValue as IList<BjcpStyleSummary>;
			var browseRecipesViewModel = context.DestinationValue as BrowseRecipesViewModel;

			// Initialize the List
			browseRecipesViewModel.BjcpCategories = new List<BjcpCategoryViewModel>();

			foreach(var category in styleList.Select(x => new { x.CategoryId, x.CategoryName }).Distinct())
			{
				browseRecipesViewModel.BjcpCategories.Add(new BjcpCategoryViewModel
				{
					CategoryId = category.CategoryId,
					CategoryName = category.CategoryName,
					Styles = styleList.Where(x => x.CategoryId == category.CategoryId).ToList(),
					RecipeCount = styleList.Where(x => x.CategoryId == category.CategoryId).Sum(y => y.RecipeCount)
				});
			}

			// get Total Recipe Count
			browseRecipesViewModel.RecipeCount = styleList.Sum(x => x.RecipeCount);

			return browseRecipesViewModel;
		}
	}
}