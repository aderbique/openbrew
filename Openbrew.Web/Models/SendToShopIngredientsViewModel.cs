using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Openbrew.Web.Core.Model;

namespace Openbrew.Web.Models
{
	public class SendToShopIngredientsViewModel
	{
		/// <summary>
		/// Gets or sets the PartnerId
		/// </summary>
		public int PartnerId { get; set; }

		/// <summary>
		/// Gets or sets the Ingredients
		/// </summary>
		public IList<PartnerSendToShopIngredient> Ingredients { get; set; }

		/// <summary>
		/// Gets or sets the IngredientJson
		/// </summary>
		public string IngredientJson { get; set; }

		/// <summary>
		/// Determines if the ingredient is in the list
		/// </summary>
		public bool HasIngredient(int typeId, int ingredientId)
		{
			return this.Ingredients.Any(x => x.IngredientTypeId == typeId && x.IngredientId == ingredientId);
		}
	}
}