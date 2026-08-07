using System;
using System.Linq;

namespace Openbrew.Web.Core.Model
{
	public class UserReputationAward
	{
		/// <summary>
		/// Gets or sets the UserRepuatationAwardId
		/// </summary>
		public long UserReputationAwardId { get; set; }

		/// <summary>
		/// Gets or sets the UserId
		/// </summary>
		public int UserId { get; set; }

		/// <summary>
		/// Gets or sets the ReputationAwardTypeId
		/// </summary>
		public int ReputationAwardTypeId { get; set; }

		/// <summary>
		/// Gets or sets the ReputationObjectTypeId
		/// </summary>
		public int ReputationObjectTypeId { get; set; }

		/// <summary>
		/// Gets or sets the ReputationObjectId
		/// </summary>
		public int ReputationObjectId { get; set; }

		/// <summary>
		/// Gets or sets the DateAwarded
		/// </summary>
		public DateTime DateAwarded { get; set; }
	}
}