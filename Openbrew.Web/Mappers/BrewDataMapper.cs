using System;
using System.Linq;
using AutoMapper;
using Openbrew.Web.Core.Model;
using ctorx.Core.Mapping;

namespace Openbrew.Web.Mappers
{
	public class BrewDataMapper : IMappingDefinition
	{
		public void DefineMappings()
		{
			Mapper.CreateMap<RecipeFermentable, Fermentable>()
				.ForMember(x => x.Name, y => y.MapFrom(z => z.Fermentable != null ? z.Fermentable.Name : null));

			Mapper.CreateMap<RecipeHop, Hop>()
				.ForMember(x => x.Name, y => y.MapFrom(z => z.Hop != null ? z.Hop.Name : null))
				.ForMember(x => x.AA, y => y.MapFrom(z => z.AlphaAcidAmount));

			Mapper.CreateMap<RecipeYeast, Yeast>()
				.ForMember(x => x.Name, y => y.MapFrom(z => z.Yeast != null ? z.Yeast.Name : null));

            Mapper.CreateMap<RecipeAdjunct, Adjunct>()
                .ForMember(x => x.Name, y => y.MapFrom(z => z.Adjunct != null ? z.Adjunct.Name : null));

            Mapper.CreateMap<RecipeMashStep, MashStep>()
                    .ForMember(x => x.Name, y => y.MapFrom(z => z.MashStep != null ? z.MashStep.Name : null));
        }
	}
}