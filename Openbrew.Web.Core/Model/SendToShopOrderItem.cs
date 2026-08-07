namespace Openbrew.Web.Core.Model
{
	public class SendToShopOrderItem
	{
		/// <summary>
		/// Gets or sets the SendToShopOrderItemId
		/// </summary>
		public int SendToShopOrderItemId { get; set; }

		/// <summary>
		/// Gets or sets the SendToShopOrderId
		/// </summary>
		public int SendToShopOrderId { get; set; }

		/// <summary>
		/// Gets or sets the IngredientTypeId
		/// </summary>
		public int IngredientTypeId { get; set; }

		/// <summary>
		/// Gets or sets the IngredientId
		/// </summary>
		public int IngredientId { get; set; }

		/// <summary>
		/// Gets or sets the Quantity
		/// </summary>
		public double Quantity { get; set; }

		/// <summary>
		/// Gets or sets the Unit
		/// </summary>
		public string Unit { get; set; }

		/// <summary>
		/// Gets or sets the Instructions
		/// </summary>
		public string Instructions { get; set; }
	}
}