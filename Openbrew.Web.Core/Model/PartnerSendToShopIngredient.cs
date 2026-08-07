namespace Openbrew.Web.Core.Model
{
	public class PartnerSendToShopIngredient
	{
		/// <summary>
		/// Gets or sets the PartnerId
		/// </summary>
		public int PartnerId { get; set; }

		/// <summary>
		/// Gets or sets the Partner
		/// </summary>
		public Partner Partner { get; set; }

		/// <summary>
		/// Gets or sets the IngredientTypeId
		/// </summary>
		public int IngredientTypeId { get; set; }

		/// <summary>
		/// Gets or sets the IngredientId
		/// </summary>
		public int IngredientId { get; set; }
	}
}