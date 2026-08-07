namespace Openbrew.Web.Core.Model
{
	public enum SendToShopOrderStatus
	{
		Cancelled = -100,
		Created = 0,
		SentToShop = 10,
		InProgress = 20,
		OnHold = 30,
		ReadyForPickup = 90,
		Completed = 100
	}
}