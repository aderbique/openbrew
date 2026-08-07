using System;
using System.Linq;
using AutoMapper;
using Openbrew.Web.Core.Model;
using Openbrew.Web.Models;
using ctorx.Core.Mapping;

namespace Openbrew.Web.Mappers
{
	public class BrewSessionMapper : IMappingDefinition
	{
		readonly IUserResolver UserResolver;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public BrewSessionMapper(IUserResolver userResolver)
		{
			this.UserResolver = userResolver;
		}

		/// <summary>
		/// Defines Mappings
		/// </summary>
		public void DefineMappings()
		{
			Mapper.CreateMap<BrewSession, BrewSession>()
				.ForMember(x => x.DateCreated, y => y.Ignore())
				.ForMember(x => x.IsActive, y => y.Ignore())
				.ForMember(x => x.IsPublic, y => y.Ignore())
				.ForMember(x => x.TastingNotes, y => y.Ignore())
				.ForMember(x => x.RecipeSummary, y => y.Ignore())
				.ForMember(x => x.BrewedByUser, y => y.Ignore())
				.ForMember(x => x.UserId, y => y.Ignore())
				.ForMember(x => x.Notes, y => y.MapFrom(z => z.Notes.Replace("\n", Environment.NewLine)));

			Mapper.CreateMap<BrewSession, BrewSessionViewModel>()
				.ForMember(x => x.BrewedByUsername, y => y.MapFrom(z => z.BrewedByUser.CalculatedUsername));

			Mapper.CreateMap<BrewSessionViewModel, BrewSession>()
				.ForMember(x => x.DateCreated, y => y.Ignore())
				.ForMember(x => x.UserId, y => y.Ignore())
				.ForMember(x => x.DateModified, y => y.Ignore())
				.ForMember(x => x.IsActive, y => y.Ignore())
				.ForMember(x => x.IsPublic, y => y.Ignore())
				.ForMember(x => x.BrewedByUser, y => y.Ignore())
				.ForMember(x => x.RecipeSummary, y => y.Ignore());

            Mapper.CreateMap<BrewSessionSummary, BrewSessionSummaryViewModel>();

            Mapper.CreateMap<BrewSessionComment, CommentViewModel>()
              .ForMember(x => x.UserName, y => y.MapFrom(z => z.User.CalculatedUsername))
              .ForMember(x => x.UserAvatarUrl, y => y.MapFrom(z => UserAvatar.GetAvatar(59, z.User.EmailAddress)))
              .ForMember(x => x.CommentText, y => y.MapFrom(z => z.CommentText.Replace("\n", Environment.NewLine)));
		}
	}
}