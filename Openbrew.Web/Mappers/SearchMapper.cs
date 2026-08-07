using System;
using System.Linq;
using AutoMapper;
using Openbrew.Web.Core.Model;
using Openbrew.Web.Models;
using ctorx.Core.Mapping;

namespace Openbrew.Web.Mappers
{
	public class SearchMapper : IMappingDefinition
	{
		public void DefineMappings()
		{
			Mapper.CreateMap<SearchResult, SearchViewModel>();
		}
	}
}