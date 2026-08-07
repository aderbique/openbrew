namespace Openbrew.Web.Core.Model
{
	public class PartnerSendToShopSettings
	{
		/// <summary>
		/// Gets or sets the PartnerId
		/// </summary>
		public int PartnerId { get; set; }

		/// <summary>
		/// Gets or sets the SendToShopFormatTypeId
		/// </summary>
		public int SendToShopFormatTypeId { get; set; }

		/// <summary>
		/// Gets or sets the SendToShopMethodTypeId
		/// </summary>
		public int SendToShopMethodTypeId { get; set; }

		/// <summary>
		/// Gets or sets the DayStart
		/// </summary>
		public int DayStart { get; set; }

		/// <summary>
		/// Gets or sets the DayEnd
		/// </summary>
		public int DayEnd { get; set; }

		/// <summary>
		/// Gets or sets the HourStart
		/// </summary>
		public int HourStart { get; set; }

		/// <summary>
		/// Gets or sets the HourEnd
		/// </summary>
		public int HourEnd { get; set; }

		/// <summary>
		/// Gets or sets the AllowOutOfRangeOrders
		/// </summary>
		public bool AllowOutOfRangeOrders { get; set; }

		/// <summary>
		/// Gets or sets the DeliveryEmailAddress
		/// </summary>
		public string DeliveryEmailAddress { get; set; }

		public bool IsInRange()
		{
			if(this.AllowOutOfRangeOrders)
			{
				return true;
			}

			// TODO: We need to handle time zones, or UTC offset 
			// TODO: to know whether or not the order is being placed within range of days/hours

			return true;
		}

		///// <summary>
		///// Gets or sets the UserConfirmationMessageText
		///// </summary>
		//public string ConfirmationMessageText { get; set; }

		///// <summary>
		///// Gets or sets the UserToContactPartnerMessageText
		///// </summary>
		//public string ContactPartnerMessageText { get; set; }

		///// <summary>
		///// Gets or sets the UserOrderReadyForPickupMessageText
		///// </summary>
		//public string ReadyForPickupMessageText { get; set; }
	}
}