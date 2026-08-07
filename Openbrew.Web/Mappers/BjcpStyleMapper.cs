using System;
using System.Collections.Generic;
using AutoMapper;
using Openbrew.Web.Core.Model;
using Openbrew.Web.Models;
using ctorx.Core.Mapping;
using ctorx.Core.Web;

namespace Openbrew.Web.Mappers
{
	public class BjcpStyleMapper : IMappingDefinition
	{
		/// <summary>
		/// Defines Mappings
		/// </summary>
		public void DefineMappings()
		{
			Mapper.CreateMap<IList<BjcpStyleSummary>, BrowseRecipesViewModel>().ConvertUsing(new BrowseRecipesViewModelConverter());

			Mapper.CreateMap<BjcpStyleSummary, OptionViewModel>()
				.ForMember(x => x.Value, y => y.MapFrom(z => z.SubCategoryId))
				.ForMember(x => x.DisplayText, y => y.MapFrom(z => z.SubCategoryName));

			Mapper.CreateMap<BjcpStyle, BJCPStyleJSONModel>();
		}
	}
}