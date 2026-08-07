using System;
using System.Linq;

namespace Openbrew.Web.Core.Model
{
	public class UserReputationSummary
	{
		/// <summary>
		/// Gets or sets the UserReputationAwardId
		/// </summary>
		public int UserReputationAwardId { get; set; }

		/// <summary>
		/// Gets or sets the UserId
		/// </summary>
		public int UserId { get; set; }

		/// <summary>
		/// Gets or sets the Amount
		/// </summary>
		public int Amount { get; set; }

		/// <summary>
		/// Gets or sets the ReputationAwardTypeId
		/// </summary>
		public int ReputationAwardTypeId { get; set; }

		/// <summary>
		/// Gets or sets the ReputationAwardTypeName
		/// </summary>
		public string ReputationAwardTypeName { get; set; }

		/// <summary>
		/// Gets or sets the ReputationObjectTypeId
		/// </summary>
		public int ReputationObjectTypeId { get; set; }

		/// <summary>
		/// Gets or sets the ReputationObjectTypeName
		/// </summary>
		public string ReputationObjectTypeName { get; set; }

		/// <summary>
		/// Gets or sets the ReputationObjectName
		/// </summary>
		public string ReputationObjectName { get; set; }

		/// <summary>
		/// Gets or sets the DateAwarded
		/// </summary>
		public DateTime DateAwarded { get; set; }
	}
}