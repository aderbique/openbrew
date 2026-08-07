using Openbrew.Web.Core.Model;

namespace Openbrew.Web.Core.Service
{
	public interface ISendToShopService
	{
		/// <summary>
		/// gets the Recipe Creation Send To Shop Settings for a Partner
		/// </summary>
		RecipeCreationSendToShopSettings GetRecipeCreationSendToShopSettings(bool includeIngredients = true);

		/// <summary>
		/// Gets a Send to Shop Order Shell
		/// </summary>
		SendToShopOrder GetSendToShopOrderShell(int userId, int recipeId);

		/// <summary>
		/// Places a send to shop order
		/// </summary>
		void PlaceOrder(SendToShopOrder sendToShopOrder);

		/// <summary>
		/// Updates the status of a send to shop order
		/// </summary>
		void UpdateStatus(int orderId, SendToShopOrderStatus status);

		/// <summary>
		/// Sends a notification for a send to shop order at a given status
		/// </summary>
		void Notify(int sendToShopOrderId, string emailAddressOverride = null);
	}
}