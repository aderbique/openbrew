using System;
using System.Collections.Generic;
using System.Linq;
using Openbrew.Web.Core.Model;
using Openbrew.Web.Validators;
using ctorx.Core.Validation;

namespace Openbrew.Web.Models
{
	public class SendToShopOrderViewModel : ValidatesWith<SendToShopOrderViewModelValidator>
	{
		/// <summary>
		/// Gets or sets the SendToShopOrderId
		/// </summary>
		public int SendToShopOrderId { get; set; }

		/// <summary>
		/// Gets or sets the UserId
		/// </summary>
		public int UserId { get; set; }

		/// <summary>
		/// Gets or sets the RecipeId
		/// </summary>
		public int RecipeId { get; set; }

		/// <summary>
		/// Gets or sets the SendToShopOrderStatusId
		/// </summary>
		public int SendToShopOrderStatusId { get; set; }

		/// <summary>
		/// Gets or sets the Name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the EmailAddress
		/// </summary>
		public string EmailAddress { get; set; }

		/// <summary>
		/// Gets or sets the PhoneNumber
		/// </summary>
		public string PhoneNumber { get; set; }

		/// <summary>
		/// Gets or sets the AllowTextMessages
		/// </summary>
		public bool AllowTextMessages { get; set; }

		/// <summary>
		/// Gets or sets the Comments
		/// </summary>
		public string Comments { get; set; }

		/// <summary>
		/// Gets or sets the DateCreated
		/// </summary>
		public DateTime DateCreated { get; set; }

		/// <summary>
		/// Gets or sets the DateModified
		/// </summary>
		public DateTime? DateModified { get; set; }

		/// <summary>
		/// Gets or sets the Partner
		/// </summary>
		public Partner Partner { get; set; }

		/// <summary>
		/// Gets or sets the Recipe
		/// </summary>
		public Recipe Recipe { get; set; }

		/// <summary>
		/// Gets or sets the PartnerIngredients
		/// </summary>
		public IList<PartnerSendToShopIngredient> PartnerIngredients { get; set; }

		/// <summary>
		/// Gets or sets the PartnerSettings
		/// </summary>
		public PartnerSendToShopSettings PartnerSettings { get; set; }

		/// <summary>
		/// Gets or sets the Items
		/// </summary>
		public IList<SendToShopOrderItem> Items { get; set; }

		/// <summary>
		/// Determines if an ingredient is available
		/// </summary>
		public bool IngredientAvailable(IngredientType ingredientType, int ingredientId)
		{
			return this.PartnerIngredients.Any(x => x.IngredientTypeId == (int)ingredientType && x.IngredientId == ingredientId);
		}
	}
}