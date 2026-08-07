using System;
using System.Linq;

namespace Openbrew.Web.Core.Model
{
	public static class RecipeTypeInferer
	{
		/// <summary>
		/// Infers a Recipe Type
		/// </summary>	
		public static RecipeType Infer(Recipe recipe)
		{
			// No fermentables, assume All Grain
			if(!recipe.Fermentables.Any())
			{
				return RecipeType.AllGrain;
			}

			// No Extract Usage, assume All Grain
			if(!recipe.Fermentables.Any(x => x.FermentableUsageTypeId == (int)FermentableUsageType.Extract))
			{
				return RecipeType.AllGrain;
			}

			// No Fermentables with Mash Usage Type, assume Extract
			if (!recipe.Fermentables.Any(x => x.FermentableUsageTypeId == (int) FermentableUsageType.Mash))
			{
				return RecipeType.Extract;
			}

			// Mash Fermentables AND Extract Fermentables, assume Partial Mash
			if(recipe.Fermentables.Any(x => x.FermentableUsageTypeId == (int)FermentableUsageType.Mash) &&
				recipe.Fermentables.Any(x => x.FermentableUsageTypeId == (int)FermentableUsageType.Extract))
			{
				return RecipeType.AllGrainPlusExtract;
			}

			return RecipeType.AllGrain;
		}
	}
}