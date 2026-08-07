using System;
using AutoMapper;
using ctorx.Core.Mapping;

namespace Openbrew.Web.Mappers
{
	public class PrimitiveMapper : IMappingDefinition
	{
		/// <summary>
		/// Defines Mappings
		/// </summary>
		public void DefineMappings()
		{
			Mapper.CreateMap<string, int>().ConvertUsing(Convert.ToInt32);
			Mapper.CreateMap<string, double>().ConvertUsing(Convert.ToDouble);
			Mapper.CreateMap<string, decimal>().ConvertUsing(Convert.ToDecimal);
			Mapper.CreateMap<string, DateTime>().ConvertUsing(Convert.ToDateTime);
		}
	}
}