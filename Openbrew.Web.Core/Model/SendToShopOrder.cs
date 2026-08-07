using System;
using System.Collections;
using System.Collections.Generic;

namespace Openbrew.Web.Core.Model
{
	public class SendToShopOrder
	{
		/// <summary>
		/// Gets or sets the SendToShopOrderId
		/// </summary>
		public int SendToShopOrderId { get; set; }

		/// <summary>
		/// Gets or sets the PartnerId
		/// </summary>
		public int PartnerId { get; set; }

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
		/// Gets or sets the Items
		/// </summary>
		public IList<SendToShopOrderItem> Items { get; set; }
	}
}