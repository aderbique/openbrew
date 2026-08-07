using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using Openbrew.Web.Core.Configuration;
using ctorx.Core.Conversion;
using ctorx.Core.Mapping;
using ctorx.Core.Web;
using Openbrew.Web.Core.Model;
using Openbrew.Web.Models;

namespace Openbrew.Web.Mappers
{
	public class RecipeMapper : IMappingDefinition
	{
		readonly IUserResolver UserResolver;
		readonly IWebSettings WebSettings;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public RecipeMapper(IUserResolver userResolver, IWebSettings webSettings)
		{
			this.UserResolver = userResolver;
			this.WebSettings = webSettings;
		}

		public void DefineMappings()
		{
			Mapper.CreateMap<Recipe, Recipe>();
			Mapper.CreateMap<RecipeFermentable, RecipeFermentable>();
			Mapper.CreateMap<RecipeHop, RecipeHop>();
			Mapper.CreateMap<RecipeYeast, RecipeYeast>();
            Mapper.CreateMap<RecipeAdjunct, RecipeAdjunct>();
            Mapper.CreateMap<RecipeMashStep, RecipeMashStep>();

			// Recipe to PostedRecipeViewModel
			Mapper.CreateMap<Recipe, RecipeViewModel>().ConvertUsing<RecipeToRecipeViewModelConverter>();

			Mapper.CreateMap<RecipeFermentable, RecipeFermentableViewModel>()
				.ForMember(x => x.Name, y => y.MapFrom(z => z.Fermentable.Name))
				.ForMember(x => x.Amt, y => y.MapFrom(z => z.Amount))
				.ForMember(x => x.Use, y => y.MapFrom(z => ((FermentableUsageType)z.FermentableUsageTypeId).ToString()));

            Mapper.CreateMap<RecipeHop, RecipeHopViewModel>()
                .ForMember(x => x.Name, y => y.MapFrom(z => z.Hop.Name))
                .ForMember(x => x.Type, y => y.MapFrom(z => ((HopType)z.HopTypeId).ToString()))
                .ForMember(x => x.Use, y => y.MapFrom(z => ((HopUsageType)z.HopUsageTypeId)))
                .ForMember(x => x.Day, y => y.MapFrom(z => z.TimeInMinutes / 1440));

            Mapper.CreateMap<RecipeYeast, RecipeYeastViewModel>()
				.ForMember(x => x.Name, y => y.MapFrom(z => z.Yeast.Name))
				.ForMember(x => x.Atten, y => y.MapFrom(z => z.Attenuation));

			Mapper.CreateMap<RecipeAdjunct, RecipeOtherViewModel>()
				.ForMember(x => x.Name, y => y.MapFrom(z => z.Adjunct.Name))
				.ForMember(x => x.Unit, y => y.MapFrom(z => z.Unit))
				.ForMember(x => x.Use, y => y.MapFrom(z => ((AdjunctUsageType)z.AdjunctUsageTypeId).ToString()));

			Mapper.CreateMap<RecipeMashStep, RecipeMashStepViewModel>()
				.ForMember(x => x.Name, y => y.MapFrom(z => z.MashStep.Name));

			Mapper.CreateMap<RecipeStep, RecipeStepViewModel>()
				.ForMember(x => x.Id, y => y.MapFrom(z => z.RecipeStepId))
				.ForMember(x => x.Text, y => y.MapFrom(z => z.Description));

			// View Model to Recipe
			Mapper.CreateMap<RecipeViewModel, Recipe>()
				.ForMember(x => x.RecipeName, y => y.MapFrom(z => z.Name.Trim()))
				.ForMember(x => x.CreatedBy, y => y.Ignore())
				.ForMember(x => x.Fermentables, y => y.Ignore())
				.ForMember(x => x.Hops, y => y.Ignore())
				.ForMember(x => x.Yeasts, y => y.Ignore())
                .ForMember(x => x.Adjuncts, y => y.Ignore())
                .ForMember(x => x.MashSteps, y => y.Ignore())
                .ForMember(x => x.Steps, y => y.Ignore())
				.ForMember(x => x.ImageUrlRoot, y => y.Ignore())
				.ForMember(x => x.DateCreated, y => y.Ignore())
				// BA 2026 style identifiers are numeric; use that as a safe fallback for older
				// calculator clients that do not send StyleCatalog yet.
				.ForMember(x => x.BjcpStyleSubCategoryId, y => y.MapFrom(z => (z.StyleCatalog == "legacy-bjcp" || (string.IsNullOrWhiteSpace(z.StyleCatalog) && !string.IsNullOrWhiteSpace(z.StyleId) && !z.StyleId.All(char.IsDigit))) && !string.IsNullOrWhiteSpace(z.StyleId) ? z.StyleId : null))
				.ForMember(x => x.StyleCatalog, y => y.MapFrom(z => string.IsNullOrWhiteSpace(z.StyleCatalog) ? (!string.IsNullOrWhiteSpace(z.StyleId) && z.StyleId.All(char.IsDigit) ? "ba-2026" : "legacy-bjcp") : z.StyleCatalog))
				.ForMember(x => x.CatalogStyleCode, y => y.MapFrom(z => !string.IsNullOrWhiteSpace(z.StyleId) ? z.StyleId : null))
				.ForMember(x => x.CatalogStyleName, y => y.MapFrom(z => z.StyleName))
				.ForMember(x => x.UnitTypeId, y => y.MapFrom(z => z.UnitType.ToLower() == "s" ? (int)UnitType.USStandard : (int)UnitType.Metric))
				.ForMember(x => x.IbuFormulaId, y => y.MapFrom(z => z.IbuFormula.ToLower() == "r" ? (int)IbuFormula.Rager : z.IbuFormula.ToLower() == "b" ? (int)IbuFormula.Brewgr : (int)IbuFormula.Tinseth));

			// Recipe Fermentables
			Mapper.CreateMap<RecipeFermentableViewModel, RecipeFermentable>()
				.ForMember(x => x.IngredientId, y => y.MapFrom(z => Converter.Convert<int>(z.IngId)))
				.ForMember(x => x.Amount, y => y.MapFrom(z => Converter.Convert<double>(z.Amt)))
				.ForMember(x => x.Lovibond, y => y.MapFrom(z => Converter.Convert<int>(z.L)))
				.ForMember(x => x.Fermentable, y => y.MapFrom(z => z))
				.ForMember(x => x.Fermentable, y => y.Condition(z => Converter.Convert<int>(z.IngId) == 0))
				.ForMember(x => x.FermentableUsageTypeId, y => y.MapFrom(z => Enum.Parse(typeof(FermentableUsageType), z.Use)));

			Mapper.CreateMap<RecipeFermentableViewModel, Fermentable>()
				.ForMember(x => x.Name, y => y.MapFrom(z => z.CustomName.Trim()))
				.ForMember(x => x.IsActive, y => y.UseValue(true))
				.ForMember(x => x.IsPublic, y => y.UseValue(false))
				.ForMember(x => x.CreatedByUserId, y => y.MapFrom(z => this.UserResolver.Resolve() != null ? this.UserResolver.Resolve().UserId : (int?)null))
				.ForMember(x => x.User, y => y.Ignore())
				.ForMember(x => x.IngredientId, y => y.Ignore())
				.ForMember(x => x.Lovibond, y => y.MapFrom(z => z.L))
				.ForMember(x => x.Ppg, y => y.MapFrom(z => z.Ppg))
				.ForMember(x => x.DatePromoted, y => y.Ignore())
				.ForMember(x => x.DateCreated, y => y.Ignore())
				.ForMember(x => x.DateCreated, y => y.UseValue(DateTime.Now))
				.ForMember(x => x.DefaultUsageTypeId, y => y.MapFrom(z => Enum.Parse(typeof(FermentableUsageType), z.Use)))
				.ForAllMembers(x => x.Condition(y => Converter.Convert<int>(y.IngId) == 0));

			// Recipe Hop
			Mapper.CreateMap<RecipeHopViewModel, RecipeHop>()
				.ForMember(x => x.IngredientId, y => y.MapFrom(z => Converter.Convert<int>(z.IngId)))
				.ForMember(x => x.Amount, y => y.MapFrom(z => Converter.Convert<double>(z.Amt)))
				.ForMember(x => x.AlphaAcidAmount, y => y.MapFrom(z => Converter.Convert<double>(z.AA)))
				.ForMember(x => x.Hop, y => y.MapFrom(z => z))
				.ForMember(x => x.Hop, y => y.Condition(z => Converter.Convert<int>(z.IngId) == 0))
				.ForMember(x => x.HopTypeId, y => y.MapFrom(z => Enum.Parse(typeof(HopType), z.Type)))
				.ForMember(x => x.HopUsageTypeId, y => y.MapFrom(z => Enum.Parse(typeof(HopUsageType), z.Use)))
				.ForMember(x => x.TimeInMinutes, y => y.MapFrom(z => z.Use == "DryHop" ? Converter.Convert<int>(z.Day) * 1440 : Converter.Convert<int>(z.Min)));


			Mapper.CreateMap<RecipeHopViewModel, Hop>()
				.ForMember(x => x.Name, y => y.MapFrom(z => z.CustomName.Trim()))
				.ForMember(x => x.IsActive, y => y.UseValue(true))
				.ForMember(x => x.IsPublic, y => y.UseValue(false))
				.ForMember(x => x.CreatedByUserId, y => y.MapFrom(z => this.UserResolver.Resolve() != null ? this.UserResolver.Resolve().UserId : (int?)null))
				.ForMember(x => x.DateCreated, y => y.UseValue(DateTime.Now))
				.ForMember(x => x.AA, y => y.MapFrom(z => z.AA))
				.ForMember(x => x.User, y => y.Ignore())
				.ForMember(x => x.IngredientId, y => y.Ignore())
				.ForMember(x => x.DatePromoted, y => y.Ignore())
				.ForAllMembers(x => x.Condition(y => Converter.Convert<int>(y.IngId) == 0));

			// Recipe Yeast
			Mapper.CreateMap<RecipeYeastViewModel, RecipeYeast>()
				.ForMember(x => x.IngredientId, y => y.MapFrom(z => Converter.Convert<int>(z.IngId)))
				.ForMember(x => x.Attenuation, y => y.MapFrom(z => Converter.Convert<double>(z.Atten)))
				.ForMember(x => x.Yeast, y => y.MapFrom(z => z))
				.ForMember(x => x.Yeast, y => y.Condition(z => Converter.Convert<int>(z.IngId) == 0));

			Mapper.CreateMap<RecipeYeastViewModel, Yeast>()
				.ForMember(x => x.Name, y => y.MapFrom(z => z.CustomName.Trim()))
				.ForMember(x => x.IsActive, y => y.UseValue(true))
				.ForMember(x => x.IsPublic, y => y.UseValue(false))
				.ForMember(x => x.CreatedByUserId, y => y.MapFrom(z => this.UserResolver.Resolve() != null ? this.UserResolver.Resolve().UserId : (int?)null))
				.ForMember(x => x.DateCreated, y => y.UseValue(DateTime.Now))
				.ForMember(x => x.Attenuation, y => y.MapFrom(z => z.Atten))
				.ForMember(x => x.User, y => y.Ignore())
				.ForMember(x => x.IngredientId, y => y.Ignore())
				.ForMember(x => x.DatePromoted, y => y.Ignore())
				.ForAllMembers(x => x.Condition(y => Converter.Convert<int>(y.IngId) == 0));

			// Recipe Adjunct
			Mapper.CreateMap<RecipeOtherViewModel, RecipeAdjunct>()
				.ForMember(x => x.IngredientId, y => y.MapFrom(z => Converter.Convert<int>(z.IngId)))
				.ForMember(x => x.Amount, y => y.MapFrom(z => Converter.Convert<double>(z.Amt)))
				.ForMember(x => x.Adjunct, y => y.MapFrom(z => z))
				.ForMember(x => x.Adjunct, y => y.Condition(z => Converter.Convert<int>(z.IngId) == 0))
				.ForMember(x => x.AdjunctUsageTypeId, y => y.MapFrom(z => Enum.Parse(typeof(AdjunctUsageType), z.Use)));

            Mapper.CreateMap<RecipeOtherViewModel, Adjunct>()
                .ForMember(x => x.Name, y => y.MapFrom(z => z.CustomName.Trim()))
                .ForMember(x => x.IsActive, y => y.UseValue(true))
                .ForMember(x => x.IsPublic, y => y.UseValue(false))
                .ForMember(x => x.CreatedByUserId, y => y.MapFrom(z => this.UserResolver.Resolve() != null ? this.UserResolver.Resolve().UserId : (int?)null))
                .ForMember(x => x.DateCreated, y => y.UseValue(DateTime.Now))
				.ForMember(x => x.User, y => y.Ignore())
				.ForMember(x => x.IngredientId, y => y.Ignore())
				.ForMember(x => x.DatePromoted, y => y.Ignore())
                .ForAllMembers(x => x.Condition(y => Converter.Convert<int>(y.IngId) == 0));

            // Recipe MashStep
            Mapper.CreateMap<RecipeMashStepViewModel, RecipeMashStep>()
                .ForMember(x => x.IngredientId, y => y.MapFrom(z => Converter.Convert<int>(z.IngId)))
				.ForMember(x => x.Temp, y => y.MapFrom(z => Converter.Convert<double>(z.Temp)))
                .ForMember(x => x.MashStep, y => y.MapFrom(z => z))
                .ForMember(x => x.MashStep, y => y.Condition(z => Converter.Convert<int>(z.IngId) == 0));

            Mapper.CreateMap<RecipeMashStepViewModel, MashStep>()
                .ForMember(x => x.Name, y => y.MapFrom(z => z.CustomName.Trim()))
                .ForMember(x => x.IsActive, y => y.UseValue(true))
                .ForMember(x => x.IsPublic, y => y.UseValue(false))
                .ForMember(x => x.CreatedByUserId, y => y.MapFrom(z => this.UserResolver.Resolve() != null ? this.UserResolver.Resolve().UserId : (int?)null))
                .ForMember(x => x.DateCreated, y => y.UseValue(DateTime.Now))
				.ForMember(x => x.User, y => y.Ignore())
				.ForMember(x => x.IngredientId, y => y.Ignore())
				.ForMember(x => x.DatePromoted, y => y.Ignore())
                .ForAllMembers(x => x.Condition(y => Converter.Convert<int>(y.IngId) == 0));


            Mapper.CreateMap<RecipeStepViewModel, RecipeStep>()
			      .ForMember(x => x.Description, y => y.MapFrom(z => z.Text.Trim()))
			      .ForMember(x => x.DateCreated, y => y.Ignore())
			      .ForMember(x => x.DateModified, y => y.Ignore());

			Mapper.CreateMap<RecipeCommentSummary, CommentViewModel>()
			      .ForMember(x => x.UserAvatarUrl, y => y.MapFrom(z => UserAvatar.GetAvatar(59, z.EmailAddress)))
			      .ForMember(x => x.CommentText, y => y.MapFrom(z => z.CommentText.Replace("\n", Environment.NewLine)));

		    Mapper.CreateMap<RecipeSummary, RecipeSummaryViewModel>()
		        .ForMember(x => x.StyleId, y => y.MapFrom(z => z.BjcpStyleSubCategoryId))
		        .ForMember(x => x.StyleName, y => y.MapFrom(z => z.BJCPStyleName))
		        .ForMember(x => x.Name, y => y.MapFrom(z => z.RecipeName))
				.ForMember(x => x.UnitType, y => y.MapFrom(z => z.UnitTypeId == (int)UnitType.USStandard ? "s" : "m"))
				.ForMember(x => x.IbuFormula, y => y.MapFrom(z => z.IbuFormulaId == (int)IbuFormula.Tinseth ? "t" : z.IbuFormulaId == (int)IbuFormula.Brewgr ? "b" : "r"));

		    Mapper.CreateMap<RecipeComment, CommentViewModel>()
		        .ForMember(x => x.UserName, y => y.MapFrom(z => z.User.CalculatedUsername))
		        .ForMember(x => x.UserAvatarUrl, y => y.MapFrom(z => UserAvatar.GetAvatar(32, z.User.EmailAddress)));
		}
	}
}
