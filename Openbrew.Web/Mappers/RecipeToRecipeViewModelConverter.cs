using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Openbrew.Web.Core.Model;
using Openbrew.Web.Models;

namespace Openbrew.Web.Mappers
{
	public class RecipeToRecipeViewModelConverter : ITypeConverter<Recipe, RecipeViewModel>
	{
		/// <summary>
		/// Converts the view model
		/// </summary>
		public RecipeViewModel Convert(ResolutionContext context)
		{
			var source = context.SourceValue as Recipe;
			var target = context.DestinationValue as RecipeViewModel;

			// Top Level
			target.RecipeId = source.RecipeId;
			target.CreatedBy = source.CreatedBy;
			target.RecipeTypeId = source.RecipeTypeId;
			target.OriginalRecipeId = source.OriginalRecipeId;
			target.Name = source.RecipeName;
			target.DateCreated = source.DateCreated;
			target.Description = source.Description;
			target.UnitType = source.GetUnitType() == UnitType.USStandard ? "s" : "m";
			target.BatchSize = source.BatchSize;
			target.BoilSize = source.BoilSize;
			target.BoilTime = source.BoilTime;
			target.IbuFormula = source.GetIbuFormula() == IbuFormula.Tinseth ? "t" : source.GetIbuFormula() == IbuFormula.Rager ? "r" : "b";
			target.StyleId = !string.IsNullOrWhiteSpace(source.CatalogStyleCode) ? source.CatalogStyleCode : source.BjcpStyleSubCategoryId;
			target.StyleName = !string.IsNullOrWhiteSpace(source.CatalogStyleName) ? source.CatalogStyleName : (source.BjcpStyleSummary != null ? source.BjcpStyleSummary.SubCategoryName : null);
			target.StyleCatalog = !string.IsNullOrWhiteSpace(source.StyleCatalog) ? source.StyleCatalog : "legacy-bjcp";
			target.Efficiency = source.Efficiency;
			target.Abv = source.Abv;
			target.BgGu = source.BgGu;
			target.Calories = source.Calories;
			target.Fg = source.Fg;
			target.Ibu = source.Ibu;
			target.Og = source.Og;
			target.Srm = source.Srm;
			target.ImageUrlRoot = source.ImageUrlRoot;
			target.IsPublic = source.IsPublic;
			target.Visibility = source.IsPublic ? "public" : "private";

			if (source.RecipeMetaData != null)
			{
				target.AverageRating = source.RecipeMetaData.AverageRating;
				target.TastingNoteCount = source.RecipeMetaData.TastingNoteCount;
			}

			target.Fermentables = new List<RecipeFermentableViewModel>();
			target.Hops = new List<RecipeHopViewModel>();
			target.Yeasts = new List<RecipeYeastViewModel>();
            target.Others = new List<RecipeOtherViewModel>();
            target.MashSteps = new List<RecipeMashStepViewModel>();
            target.Steps = new List<RecipeStepViewModel>();

			var fermentableTotal = source.Fermentables.Sum(x => x.Amount);

			// Hydrate Fermentables
			foreach (var fermentable in source.Fermentables.OrderBy(x => x.Rank))
			{
				target.Fermentables.Add(new RecipeFermentableViewModel
				{
					Id = fermentable.RecipeFermentableId.ToString(),
					IngId = fermentable.IngredientId.ToString(),
					Rank = fermentable.Rank.ToString(),
					CustomName = fermentable.IngredientId == 0 ? fermentable.Fermentable.Name : "",
					Per = Math.Round(((fermentable.Amount / fermentableTotal) * 100), 0).ToString(),
					Name = fermentable.Fermentable.Name,
					Amt = (fermentable.Amount).ToString(),
					Ppg = fermentable.Ppg.ToString(),
					L = fermentable.Lovibond.ToString(),
					Use = fermentable.GetUsageType().ToString()					
				});
			}

			// Hydrate Hops
			foreach (var hop in source.Hops.OrderBy(x => x.Rank))
			{
				target.Hops.Add(new RecipeHopViewModel
				{
					Id = hop.RecipeHopId.ToString(),
					IngId = hop.IngredientId.ToString(),
					Rank = hop.Rank.ToString(),
					CustomName = hop.IngredientId == 0 ? hop.Hop.Name : "",
					Name = hop.Hop.Name,
					Amt = hop.Amount.ToString(),
					Min = hop.GetUsageType() != HopUsageType.DryHop ? hop.TimeInMinutes.ToString() : "0",
					Day = hop.GetUsageType() == HopUsageType.DryHop ? (hop.TimeInMinutes / 1440).ToString() : "0",
					Type = hop.GetHopType().ToString(),
					Use = hop.GetUsageType().ToString(),
					AA = hop.AlphaAcidAmount.ToString(),
					Ibu = hop.Ibu.ToString()
				});
			}

			// Hydrate Yeasts
			foreach (var yeast in source.Yeasts.OrderBy(x => x.Rank))
			{
				target.Yeasts.Add(new RecipeYeastViewModel
				{
					Id = yeast.RecipeYeastId.ToString(),
					IngId = yeast.IngredientId.ToString(),
					Rank = yeast.Rank.ToString(),
					CustomName = yeast.IngredientId == 0 ? yeast.Yeast.Name : "",
					Name = yeast.Yeast.Name,
					Atten = yeast.Attenuation.ToString()
				});
			}

            // Hydrate Others
            foreach (var adjunct in source.Adjuncts.OrderBy(x => x.Rank))
            {
                target.Others.Add(new RecipeOtherViewModel
                {
                    Id = adjunct.RecipeAdjunctId.ToString(),
                    IngId = adjunct.IngredientId.ToString(),
                    Rank = adjunct.Rank.ToString(),
                    CustomName = adjunct.IngredientId == 0 ? adjunct.Adjunct.Name : "",
                    Name = adjunct.Adjunct.Name,
                    Amt = adjunct.Amount.ToString(),
                    Unit = adjunct.Unit,
                    Use = adjunct.GetUsageType().ToString()
                });
            }

            // Hydrate MashSteps
            foreach (var mashStep in source.MashSteps.OrderBy(x => x.Rank))
            {
                target.MashSteps.Add(new RecipeMashStepViewModel
                {
                    Id = mashStep.RecipeMashStepId.ToString(),
                    IngId = mashStep.IngredientId.ToString(),
                    Rank = mashStep.Rank.ToString(),
                    CustomName = mashStep.IngredientId == 0 ? mashStep.MashStep.Name : "",
                    Name = mashStep.MashStep.Name,
                    Heat = mashStep.Heat,
                    Temp = mashStep.Temp.ToString(),
                    Time = mashStep.Time.ToString()
                });
            }

            // Hydrate Steps
			foreach(var step in source.Steps.OrderBy(x => x.Rank))
			{
				target.Steps.Add(new RecipeStepViewModel
				{
					Id = step.RecipeStepId.ToString(),
					Rank = (step.Rank ?? 0).ToString(),
					Text = step.Description
				});
			}

			return target;
		}
	}
}
