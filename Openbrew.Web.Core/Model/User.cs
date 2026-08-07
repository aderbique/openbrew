using System;
using System.Collections.Generic;
using System.Linq;

namespace Openbrew.Web.Core.Model
{
	public class User
	{
		/// <summary>
		/// Gets or sets the UserId
		/// </summary>
		public int UserId { get; set; }

		/// <summary>
		/// Gets or sets the Username
		/// </summary>
		public string Username { get; set; }

		/// <summary>
		/// Gets or sets the EmailAddress
		/// </summary>
		public string EmailAddress { get; set; }

		/// <summary>
		/// Gets or sets the Password
		/// </summary>
		public byte[] Password { get; set; }

		/// <summary>
		/// Gets or sets the FirstNamne
		/// </summary>
		public string FirstName { get; set; }

		/// <summary>
		/// Gets or sets the LastName
		/// </summary>
		public string LastName { get; set; }

		/// <summary>
		/// Gets or sets the Bio
		/// </summary>
		public string Bio { get; set; }

		/// <summary>
		/// Gets or sets the HasCustomUsername
		/// </summary>
		public bool HasCustomUsername { get; set; }

		/// <summary>
		/// Gets or sets the IsActive
		/// </summary>
		public bool IsActive { get; set; }

		/// <summary>
		/// Gets or sets the DateCreated
		/// </summary>
		public DateTime DateCreated { get; set; }

		/// <summary>
		/// Gets or sets the ComputedUsername
		/// </summary>
		public string CalculatedUsername { get; set; }

		/// <summary>
		/// Gets or sets the DateModified
		/// </summary>
		public DateTime? DateModified { get; set; }

		/// <summary>
		/// Gets or sets the UserSummary
		/// </summary>
		public UserSummary UserSummary { get; set; }

		/// <summary>
		/// Gets or sets the UserAdmin
		/// </summary>
		public UserAdmin UserAdmin { get; set; }

		/// <summary>
		/// Gets or sets the UserPartnerAdmins
		/// </summary>
		public IList<UserPartnerAdmin> UserPartnerAdmins { get; set; }

		/// <summary>
		/// Gets or sets the UserLogins
		/// </summary>
		public IList<UserLogin> UserLogins { get; set; }

		/// <summary>
		/// Gets or sets the UserOAuthUserIds
		/// </summary>
		public IList<UserOAuthUserId> UserOAuthUserIds { get; set; }

		/// <summary>
		/// Gets or sets the UserReputationAwards
		/// </summary>
		public IList<UserReputationAward> UserReputationAwards { get; set; }

		/// <summary>
		/// Gets or sets the UserConnections
		/// </summary>
		public IList<UserConnection> UserConnections { get; set; }

		/// <summary>
		/// Gets or sets the UserNotificationTypes
		/// </summary>
		public virtual IList<UserNotificationType> UserNotificationTypes { get; set; }

		/// <summary>
		/// Gets or sets the Badges
		/// </summary>
		public IList<Badge> Badges { get; set; }

		/// <summary>
		/// Gets or sets the Recipes
		/// </summary>
		public IList<Recipe> Recipes { get; set; }

		/// <summary>
		/// Gets or sets the RecipeComments
		/// </summary>
		public IList<RecipeComment> RecipeComments { get; set; }

		/// <summary>
		/// Gets or sets the BrewSessions
		/// </summary>
		public IList<BrewSession> BrewSessions { get; set; }

		/// <summary>
		/// Gets or sets the TastingNotes
		/// </summary>
		public IList<TastingNote> TastingNotes { get; set; }

		/// <summary>
		/// Gets or sets the Fermentables
		/// </summary>
		public IList<Fermentable> Fermentables { get; set; }

		///// <summary>
		///// Gets or sets the Hops
		///// </summary>
		public IList<Hop> Hops { get; set; }

		///// <summary>
		///// Gets or sets the Yeasts
		///// </summary>
		public IList<Yeast> Yeasts { get; set; }

        ///// <summary>
        ///// Gets or sets the Adjuncts
        ///// </summary>
        public IList<Adjunct> Adjuncts { get; set; }

        ///// <summary>
        ///// Gets or sets the MashSteps
        ///// </summary>
        public IList<MashStep> MashSteps { get; set; }
    }
}