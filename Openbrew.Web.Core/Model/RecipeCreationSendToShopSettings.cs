using System;
using System.Collections.Generic;
using System.Linq;

namespace Openbrew.Web.Core.Model
{
	public class RecipeCreationSendToShopSettings
	{
		/// <summary>
		/// Gets or sets the IsEnabled
		/// </summary>
		public bool IsEnabled { get; set; }

		/// <summary>
		/// Gets or sets the PartnerId
		/// </summary>
		public int PartnerId { get; set; }

		/// <summary>
		/// Gets or sets the PartnerName
		/// </summary>
		public string PartnerName { get; set; }

		/// <summary>
		/// Gets or sets the Ingredients
		/// </summary>
		public IList<PartnerSendToShopIngredient> Ingredients { get; set; }

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public RecipeCreationSendToShopSettings()
		{
			this.Ingredients = new List<PartnerSendToShopIngredient>();
		}

		/// <summary>
		/// Determines if the ingredient is available from the partner
		/// </summary>
		public bool IngredientIsEnabled(IIngredient ingredient)
		{
			if(ingredient == null)
			{
				throw new ArgumentNullException("ingredient");
			}

			var ingType = ingredient is Fermentable ? IngredientType.Fermentable : ingredient is Hop ? IngredientType.Hop :
				ingredient is Yeast ? IngredientType.Yeast : IngredientType.Adjunct;

			return this.Ingredients.Any(x => x.IngredientTypeId == (int)ingType && x.IngredientId == ingredient.IngredientId);
		}
	}
}