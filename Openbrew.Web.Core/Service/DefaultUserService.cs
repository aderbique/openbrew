using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AutoMapper;
using ctorx.Core.Crypto;
using ctorx.Core.Data;
using Openbrew.Web.Core.Data;
using Openbrew.Web.Core.Model;

namespace Openbrew.Web.Core.Service
{
	public class DefaultUserService : IUserService
	{
		IBrewgrRepository Repository;
		readonly IHasher Hasher;
		readonly INotificationService NotificationService;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public DefaultUserService(IBrewgrRepository repository, IHasher hasher, INotificationService notificationService)
		{
			this.Repository = repository;
			this.Hasher = hasher;
			this.NotificationService = notificationService;
		}

        /// <summary>
        /// Gets a user by Id
        /// </summary>
        public User GetUserById(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException("userId");
            }

            return this.Repository.GetSet<User>()
				.Where(x => x.IsActive)
                .FirstOrDefault(x => x.UserId == userId);
        }

        /// <summary>
        /// Gets a user by username
        /// </summary>
        public User GetUserByUserName(string userName)
        {
            if (String.IsNullOrEmpty(userName))
            {
                throw new ArgumentOutOfRangeException("userName");
            }

			if(userName.ToLower().StartsWith("brewer-"))
			{
				var userId = 0;
				if(!Int32.TryParse(userName.ToLower().Replace("brewer-", ""), out userId))
				{
					throw new InvalidOperationException("The user id portion of the username is invalid");
				}

				return this.Repository.GetSet<User>()
					.Where(x => x.IsActive)
					.FirstOrDefault(x => x.UserId == userId);
			}

            return this.Repository.GetSet<User>()
                .FirstOrDefault(x => x.Username == userName);
        }

        /// <summary>
		/// Adds a User
		/// </summary>
		public void AddUser(User user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			user.DateCreated = DateTime.Now;
			this.Repository.Add(user);
		}

		/// <summary>
		/// Gets a User Summary
		/// </summary>
		public UserSummary GetUserSummaryById(int userId)
		{
			if (userId <= 0)
			{
				throw new ArgumentOutOfRangeException("userId");
			}

			var user = this.Repository.GetSet<User>()
				.Include(x => x.UserAdmin)
				.Include(x => x.UserPartnerAdmins)
				.Where(x => x.UserId == userId)
				.Where(x => x.IsActive)
				.FirstOrDefault();

			if(user == null)
			{
				throw new InvalidOperationException("The specified user does not exist");
			}

			return Mapper.Map(user, new UserSummary());
		}

		/// <summary>
		/// Determines if the provided email address is already in use
		/// </summary>
		public bool EmailAddressIsInUse(string emailAddress, int? userId = null)
		{
			if(string.IsNullOrWhiteSpace(emailAddress))
			{
				throw new ArgumentNullException("emailAddress");
			}

			return this.Repository.GetSet<User>()
				.Any(x => x.EmailAddress == emailAddress && (userId == null || userId != x.UserId));
		}

		/// <summary>
		/// Registers a new User
		/// </summary>
		public User RegisterNewUser(string fullName, string emailAddress, string password)
		{
			if(string.IsNullOrWhiteSpace(fullName))
			{
				throw new ArgumentNullException("fullName");
			}

			if(string.IsNullOrWhiteSpace(emailAddress))
			{
				throw new ArgumentNullException("emailAddress");
			}

			if(string.IsNullOrWhiteSpace(password))
			{
				throw new ArgumentNullException("password");
			}

			var nameParts = fullName.Split(' ');

			var user = new User
			{
				FirstName = nameParts.First(),
				LastName = string.Join(" ", nameParts.Skip(1)),
				EmailAddress = emailAddress,
				Password = this.Hasher.Hash(password),
				IsActive = true,
				Username = Guid.NewGuid().ToString().Replace("-",""),
				HasCustomUsername = false,
				DateCreated = DateTime.Now
			};

			this.SubscribeUserToDefaultNotifications(user);

			this.Repository.Add(user);

			return user;
		}

		/// <summary>
		/// Determines if a username is in use
		/// </summary>
		public bool UsernameIsInUse(int userId, string username)
		{
			if(string.IsNullOrWhiteSpace(username))
			{
				throw new ArgumentNullException("username");
			}

			// We don't allow Brewer Usernames because of blank userames
			if (username.ToLower().Trim().StartsWith("brewer"))
			{
				return true;
			}

			return this.Repository.GetSet<User>()
				.Any(x => x.Username == username && x.UserId != userId);
		}

		/// <summary>
		/// Sets a username on a User
		/// </summary>
		public void SetUserUsername(int userId, string username)
		{
			if(userId <= 0)
			{
				throw new ArgumentOutOfRangeException("userId");
			}

			if(string.IsNullOrWhiteSpace(username))
			{
				throw new ArgumentNullException("username");
			}

			var user = this.Repository.GetSet<User>()
				.FirstOrDefault(x => x.UserId == userId);

			user.Username = username;
			user.HasCustomUsername = true;
			user.DateModified = DateTime.Now;
		}

		/// <summary>
		/// Gets the reputation score for a user
		/// </summary>
		public int GetUserReputationScore(int userId)
		{
			if(userId <= 0)
			{
				throw new ArgumentOutOfRangeException("userId");
			}

			return this.Repository.GetSet<UserReputationSummary>()
				.Where(x => x.UserId == userId)
				.Select(x => x.Amount)
				.Cast<int?>()
				.Sum() ?? 0;
		}

		/// <summary>
		/// Awards Reputation to a User
		/// </summary>
		public void AwardReputation(int userId, ReputationAwardType awardType, ReputationObjectType objectType, int objectId)
		{
			var award = new UserReputationAward
			{
				UserId = userId,
				ReputationAwardTypeId = (int)awardType,
				ReputationObjectTypeId = (int)objectType,
				ReputationObjectId = objectId,
				DateAwarded = DateTime.Now
			};

			this.Repository.Add(award);
		}

		/// <summary>
		/// Gets all users.  Use with Caution.
		/// </summary>
		/// <returns></returns>
		public IList<User> GetAllUsers()
		{
			return this.Repository.GetSet<User>()
				.Where(x => x.IsActive)
				.OrderByDescending(x => x.DateCreated)
				.ThenByDescending(x => x.DateModified)
				.ToList();
		}

		/// <summary>
		/// Determines if a user is an Admin
		/// </summary>
		public bool UserIsAdmin(int userId)
		{
			if(userId <= 0)
			{
				throw new ArgumentOutOfRangeException("userId");
			}

			return this.Repository.GetSet<User>()
				.Include(x => x.UserAdmin)
				.Where(x => x.UserId == userId)
				.Any(x => x.UserAdmin.IsActive);
		}

		/// <summary>
		/// Gets a list of user notification types
		/// </summary>
		public IList<UserNotificationType> GetUserNotificationTypes(int userId)
		{		
			if(userId <= 0)
			{
				throw new ArgumentOutOfRangeException("userId");
			}

			return this.Repository.GetSet<UserNotificationType>()
			    .Where(x => x.UserId == userId)
			    .ToList();
		}

		/// <summary>
		/// Subscribes a user to default user notifications
		/// </summary>
		public void SubscribeUserToDefaultNotifications(User user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			if (user.UserNotificationTypes == null)
			{
				user.UserNotificationTypes = new List<UserNotificationType>();
			}

			user.UserNotificationTypes.Add(new UserNotificationType {NotificationTypeId = (int) NotificationType.RecipeComment, DateCreated = DateTime.Now });
			user.UserNotificationTypes.Add(new UserNotificationType {NotificationTypeId = (int) NotificationType.BrewerFollowed, DateCreated = DateTime.Now });
			user.UserNotificationTypes.Add(new UserNotificationType { NotificationTypeId = (int)NotificationType.SiteFeatures, DateCreated = DateTime.Now });
		}

		/// <summary>
		/// Subscribes a user to a notification type
		/// </summary>
		public void SubscribeUserToNotificationType(User user, NotificationType notificationType)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			var userNotificationType = user.UserNotificationTypes.FirstOrDefault(x => x.NotificationTypeId == (int)notificationType);

			if (userNotificationType != null)
			{
				return;
			}

			userNotificationType = new UserNotificationType
			{
				NotificationTypeId = (int)notificationType,
				DateCreated = DateTime.Now
			};

			user.UserNotificationTypes.Add(userNotificationType);
		}

		/// <summary>
		/// Unsubscribes a user from a notification Type
		/// </summary>
		public void UnsubscribeUserFromNotificationType(User user, NotificationType notificationType)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			var userNotificationType = user.UserNotificationTypes.FirstOrDefault(x => x.NotificationTypeId == (int) notificationType);

			if (userNotificationType == null)
			{
				return;
			}

			this.Repository.Delete(userNotificationType);
		}

		/// <summary>
		/// Determines if a user follows another user
		/// </summary>
		public bool DoesUserFollowUser(int userId, int otherUserId)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets a list of followed users
		/// </summary>
		public IList<int> GetFollowedUsers(int userId)
		{
			if (userId <= 0)
			{
				throw new ArgumentOutOfRangeException("userId");
			}

			return this.Repository.GetSet<UserConnection>()
			    .Where(x => x.UserId == userId)
			    .Where(x => x.IsActive)
			    .Select(x => x.FollowedById)
			    .ToList();
		}

		/// <summary>
		/// Determines if a user is followed by another user
		/// </summary>
		public bool UserIsFollowedBy(int userId, int followerId)
		{
			if (userId <= 0)
			{
				throw new ArgumentOutOfRangeException("userId");
			}

			if (followerId <= 0)
			{
				throw new ArgumentOutOfRangeException("followerId");
			}

			return this.Repository.GetSet<UserConnection>()
			    .Any(x => x.UserId == userId && x.FollowedById == followerId && x.IsActive);
		}

		/// <summary>
		/// Toggles a User Follow
		/// </summary>
		public void ToggleUserFollow(int userId, int followedById)
		{
			if (userId <= 0)
			{
				throw new ArgumentOutOfRangeException("userId");
			}

			if (followedById <= 0)
			{
				throw new ArgumentOutOfRangeException("followedById");
			}

			var currentConnection = this.Repository.GetSet<UserConnection>()
				.FirstOrDefault(x => x.UserId == userId && x.FollowedById == followedById);

			// Not Connected, Connect (and message)
			if (currentConnection == null)
			{
				var userConnection = new UserConnection
				{
					UserId = userId,
					FollowedById = followedById,
					IsActive = true,
					DateCreated = DateTime.Now
				};

				this.Repository.Add(userConnection);

				// Queue the Notification (we only do this for followed the first time)
				this.NotificationService.QueueNotification(NotificationType.BrewerFollowed, new[]{ userId, followedById });

				return;
			}

			// Toggle Existing
			currentConnection.IsActive = !currentConnection.IsActive;
			currentConnection.DateModified = DateTime.Now;
		}

		/// <summary>
		/// Gets a user's followers
		/// </summary>
		public IList<MiniUserSummary> GetFollowersOf(int userId)
		{
			if (userId <= 0)
			{
				throw new ArgumentOutOfRangeException("userId");
			}

			return this.Repository.GetSet<UserConnection>()
				.Where(x => x.UserId == userId)
				.Where(x => x.IsActive)
				.Join(this.Repository.GetSet<MiniUserSummary>(), x => x.FollowedById, y => y.UserId, (x, y) => new { MiniUserSummary = y})
				.Select(x => x.MiniUserSummary)
				.OrderBy(x => x.Username.StartsWith("Brewer") ? "zzz" + x.Username : x.Username)
				.ToList();
		}

        /// <summary>
        /// Gets people followed by a user
        /// </summary>
        public IList<MiniUserSummary> GetFollowedBy(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException("userId");
            }

            return this.Repository.GetSet<UserConnection>()
                .Where(x => x.FollowedById == userId)
                .Where(x => x.IsActive)
                .Join(this.Repository.GetSet<MiniUserSummary>(), x => x.UserId, y => y.UserId, (x, y) => new { MiniUserSummary = y })
                .Select(x => x.MiniUserSummary)
                .OrderBy(x => x.Username.StartsWith("Brewer") ? "zzz" + x.Username : x.Username)
                .ToList();
        }

        /// <summary>
        /// Gets people followed by a user with a count
        /// </summary>
        public IList<MiniUserSummary> GetFollowedBy(int userId, int count)
        {
            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException("userId");
            }

            return this.Repository.GetSet<UserConnection>()
                .Where(x => x.FollowedById == userId)
                .Where(x => x.IsActive)
                .Join(this.Repository.GetSet<MiniUserSummary>(), x => x.UserId, y => y.UserId, (x, y) => new { MiniUserSummary = y })
                .Select(x => x.MiniUserSummary)
                .OrderBy(x => x.Username.StartsWith("Brewer") ? "zzz" + x.Username : x.Username)
                .Take(count)
                .ToList();
        }

        /// <summary>
        /// Gets people followed by a user
        /// </summary>
        public int GetFollowedByCount(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException("userId");
            }

            return this.Repository.GetSet<UserConnection>()
                .Where(x => x.FollowedById == userId)
                .Where(x => x.IsActive)
                .Join(this.Repository.GetSet<MiniUserSummary>(), x => x.UserId, y => y.UserId, (x, y) => new { MiniUserSummary = y })
                .Select(x => x.MiniUserSummary)
                .Count();
        }

        /// <summary>
		/// Gets a list of the top contributors
		/// </summary>
		public IList<UserSummary> GetWeeklyTopContributors(int count)
		{
			// Based on # of Recipes, # of Brew Sessions and # of Comments in last 7 days
			// Has Custom Username + 5 otherwise -5 points
			// Recipe = +2
			// Brew Sesssion = +1
			// Comment = + .5

			var startDate = DateTime.Now.AddDays(-7);

			return this.Repository.GetSet<User>()
				.Include(x => x.Recipes)
				.Include(x => x.BrewSessions)
				.Include(x => x.RecipeComments)
				.Where(x => x.IsActive)
				.Where(x => x.Recipes.Any(y => y.IsActive && y.IsPublic) || x.BrewSessions.Any(y => y.IsActive && y.IsPublic) || x.RecipeComments.Any(y => y.IsActive))
				.Select(x => new { UserSummary = x.UserSummary,
					Score = 
					(
						(x.HasCustomUsername ? 3 : 0)
						+ (x.Recipes.Count(y => y.DateCreated >= startDate && (y.IsActive && y.IsPublic)) * 2)
						+ (x.BrewSessions.Count(y => y.DateCreated >= startDate && (y.IsActive && y.IsPublic)) * 1)
						+ (x.RecipeComments.Count(y => y.DateCreated >= startDate && y.IsActive) * .25)
					)
				})
				.OrderByDescending(x => x.Score)
				.Take(count)
				.Select(x => x.UserSummary)
				.ToList();
		}

		/// <summary>
		/// Determines if a user can change their username
		/// </summary>
		public bool UserCanChangeUsername(int userId)
		{
			if(userId <= 0)
			{
				throw new ArgumentOutOfRangeException("userId");
			}

			return this.Repository.GetSet<User>()
				.Where(x => x.UserId == userId)
				.Select(x => !x.HasCustomUsername)
				.FirstOrDefault();
		}

        /// <summary>
        /// Gets user stats
        /// </summary>
        public UserStat GetUserStats(int userId, DateTime howFarBackToSearch)
        {
            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException("userId");
            }

            var userStat = new UserStat();

            userStat.RecipeCount = this.Repository.GetSet<Recipe>()
                .Where(x => x.CreatedBy == userId)
                .Where(x => x.IsActive == true)
                .Where(x => x.IsPublic == true)
                .Where(x => x.DateCreated > howFarBackToSearch)
                .Count();

            userStat.SessionCount = this.Repository.GetSet<BrewSession>()
               .Where(x => x.UserId == userId)
               .Where(x => x.IsActive == true)
               .Where(x => x.IsPublic == true)
               .Where(x => x.DateCreated > howFarBackToSearch)
               .Count();

            userStat.CommentCount = this.Repository.GetSet<RecipeComment>()
                .Where(x => x.UserId == userId)
                .Where(x => x.IsActive == true)
                .Where(x => x.DateCreated > howFarBackToSearch)
               .Count();

            return userStat;
        }

		#region COUNTRY LIST 

		/// <summary>
		/// Gets a list of countries
		/// </summary>
		public IList<Country> GetCountryList()
		{
			var countries = new List<Country>();

			countries.Add(new Country { CountryCode = "AF", CountryName = "Afghanistan" });
			countries.Add(new Country { CountryCode = "AX", CountryName = "Åland Islands" });
			countries.Add(new Country { CountryCode = "AL", CountryName = "Albania" });
			countries.Add(new Country { CountryCode = "DZ", CountryName = "Algeria" });
			countries.Add(new Country { CountryCode = "AS", CountryName = "American Samoa" });
			countries.Add(new Country { CountryCode = "AD", CountryName = "Andorra" });
			countries.Add(new Country { CountryCode = "AO", CountryName = "Angola" });
			countries.Add(new Country { CountryCode = "AI", CountryName = "Anguilla" });
			countries.Add(new Country { CountryCode = "AQ", CountryName = "Antarctica" });
			countries.Add(new Country { CountryCode = "AG", CountryName = "Antigua and Barbuda" });
			countries.Add(new Country { CountryCode = "AR", CountryName = "Argentina" });
			countries.Add(new Country { CountryCode = "AM", CountryName = "Armenia" });
			countries.Add(new Country { CountryCode = "AW", CountryName = "Aruba" });
			countries.Add(new Country { CountryCode = "AU", CountryName = "Australia" });
			countries.Add(new Country { CountryCode = "AT", CountryName = "Austria" });
			countries.Add(new Country { CountryCode = "AZ", CountryName = "Azerbaijan" });
			countries.Add(new Country { CountryCode = "BS", CountryName = "Bahamas" });
			countries.Add(new Country { CountryCode = "BH", CountryName = "Bahrain" });
			countries.Add(new Country { CountryCode = "BD", CountryName = "Bangladesh" });
			countries.Add(new Country { CountryCode = "BB", CountryName = "Barbados" });
			countries.Add(new Country { CountryCode = "BY", CountryName = "Belarus" });
			countries.Add(new Country { CountryCode = "BE", CountryName = "Belgium" });
			countries.Add(new Country { CountryCode = "BZ", CountryName = "Belize" });
			countries.Add(new Country { CountryCode = "BJ", CountryName = "Benin" });
			countries.Add(new Country { CountryCode = "BM", CountryName = "Bermuda" });
			countries.Add(new Country { CountryCode = "BT", CountryName = "Bhutan" });
			countries.Add(new Country { CountryCode = "BO", CountryName = "Bolivia" });
			countries.Add(new Country { CountryCode = "BA", CountryName = "Bosnia and Herzegovina" });
			countries.Add(new Country { CountryCode = "BW", CountryName = "Botswana" });
			countries.Add(new Country { CountryCode = "BV", CountryName = "Bouvet Island" });
			countries.Add(new Country { CountryCode = "BR", CountryName = "Brazil" });
			countries.Add(new Country { CountryCode = "IO", CountryName = "British Indian Ocean Territory" });
			countries.Add(new Country { CountryCode = "BN", CountryName = "Brunei Darussalam" });
			countries.Add(new Country { CountryCode = "BG", CountryName = "Bulgaria" });
			countries.Add(new Country { CountryCode = "BF", CountryName = "Burkina Faso" });
			countries.Add(new Country { CountryCode = "BI", CountryName = "Burundi" });
			countries.Add(new Country { CountryCode = "KH", CountryName = "Cambodia" });
			countries.Add(new Country { CountryCode = "CM", CountryName = "Cameroon" });
			countries.Add(new Country { CountryCode = "CA", CountryName = "Canada" });
			countries.Add(new Country { CountryCode = "CV", CountryName = "Cape Verde" });
			countries.Add(new Country { CountryCode = "KY", CountryName = "Cayman Islands" });
			countries.Add(new Country { CountryCode = "CF", CountryName = "Central African Republic" });
			countries.Add(new Country { CountryCode = "TD", CountryName = "Chad" });
			countries.Add(new Country { CountryCode = "CL", CountryName = "Chile" });
			countries.Add(new Country { CountryCode = "CN", CountryName = "China" });
			countries.Add(new Country { CountryCode = "CX", CountryName = "Christmas Island" });
			countries.Add(new Country { CountryCode = "CC", CountryName = "Cocos (Keeling) Islands" });
			countries.Add(new Country { CountryCode = "CO", CountryName = "Colombia" });
			countries.Add(new Country { CountryCode = "KM", CountryName = "Comoros" });
			countries.Add(new Country { CountryCode = "CG", CountryName = "Congo" });
			countries.Add(new Country { CountryCode = "CD", CountryName = "Congo, The Democratic Republic of The" });
			countries.Add(new Country { CountryCode = "CK", CountryName = "Cook Islands" });
			countries.Add(new Country { CountryCode = "CR", CountryName = "Costa Rica" });
			countries.Add(new Country { CountryCode = "CI", CountryName = "Cote D'ivoire" });
			countries.Add(new Country { CountryCode = "HR", CountryName = "Croatia" });
			countries.Add(new Country { CountryCode = "CU", CountryName = "Cuba" });
			countries.Add(new Country { CountryCode = "CY", CountryName = "Cyprus" });
			countries.Add(new Country { CountryCode = "CZ", CountryName = "Czech Republic" });
			countries.Add(new Country { CountryCode = "DK", CountryName = "Denmark" });
			countries.Add(new Country { CountryCode = "DJ", CountryName = "Djibouti" });
			countries.Add(new Country { CountryCode = "DM", CountryName = "Dominica" });
			countries.Add(new Country { CountryCode = "DO", CountryName = "Dominican Republic" });
			countries.Add(new Country { CountryCode = "EC", CountryName = "Ecuador" });
			countries.Add(new Country { CountryCode = "EG", CountryName = "Egypt" });
			countries.Add(new Country { CountryCode = "SV", CountryName = "El Salvador" });
			countries.Add(new Country { CountryCode = "GQ", CountryName = "Equatorial Guinea" });
			countries.Add(new Country { CountryCode = "ER", CountryName = "Eritrea" });
			countries.Add(new Country { CountryCode = "EE", CountryName = "Estonia" });
			countries.Add(new Country { CountryCode = "ET", CountryName = "Ethiopia" });
			countries.Add(new Country { CountryCode = "FK", CountryName = "Falkland Islands (Malvinas)" });
			countries.Add(new Country { CountryCode = "FO", CountryName = "Faroe Islands" });
			countries.Add(new Country { CountryCode = "FJ", CountryName = "Fiji" });
			countries.Add(new Country { CountryCode = "FI", CountryName = "Finland" });
			countries.Add(new Country { CountryCode = "FR", CountryName = "France" });
			countries.Add(new Country { CountryCode = "GF", CountryName = "French Guiana" });
			countries.Add(new Country { CountryCode = "PF", CountryName = "French Polynesia" });
			countries.Add(new Country { CountryCode = "TF", CountryName = "French Southern Territories" });
			countries.Add(new Country { CountryCode = "GA", CountryName = "Gabon" });
			countries.Add(new Country { CountryCode = "GM", CountryName = "Gambia" });
			countries.Add(new Country { CountryCode = "GE", CountryName = "Georgia" });
			countries.Add(new Country { CountryCode = "DE", CountryName = "Germany" });
			countries.Add(new Country { CountryCode = "GH", CountryName = "Ghana" });
			countries.Add(new Country { CountryCode = "GI", CountryName = "Gibraltar" });
			countries.Add(new Country { CountryCode = "GR", CountryName = "Greece" });
			countries.Add(new Country { CountryCode = "GL", CountryName = "Greenland" });
			countries.Add(new Country { CountryCode = "GD", CountryName = "Grenada" });
			countries.Add(new Country { CountryCode = "GP", CountryName = "Guadeloupe" });
			countries.Add(new Country { CountryCode = "GU", CountryName = "Guam" });
			countries.Add(new Country { CountryCode = "GT", CountryName = "Guatemala" });
			countries.Add(new Country { CountryCode = "GG", CountryName = "Guernsey" });
			countries.Add(new Country { CountryCode = "GN", CountryName = "Guinea" });
			countries.Add(new Country { CountryCode = "GW", CountryName = "Guinea-bissau" });
			countries.Add(new Country { CountryCode = "GY", CountryName = "Guyana" });
			countries.Add(new Country { CountryCode = "HT", CountryName = "Haiti" });
			countries.Add(new Country { CountryCode = "HM", CountryName = "Heard Island and Mcdonald Islands" });
			countries.Add(new Country { CountryCode = "VA", CountryName = "Holy See (Vatican City State)" });
			countries.Add(new Country { CountryCode = "HN", CountryName = "Honduras" });
			countries.Add(new Country { CountryCode = "HK", CountryName = "Hong Kong" });
			countries.Add(new Country { CountryCode = "HU", CountryName = "Hungary" });
			countries.Add(new Country { CountryCode = "IS", CountryName = "Iceland" });
			countries.Add(new Country { CountryCode = "IN", CountryName = "India" });
			countries.Add(new Country { CountryCode = "ID", CountryName = "Indonesia" });
			countries.Add(new Country { CountryCode = "IR", CountryName = "Iran, Islamic Republic of" });
			countries.Add(new Country { CountryCode = "IQ", CountryName = "Iraq" });
			countries.Add(new Country { CountryCode = "IE", CountryName = "Ireland" });
			countries.Add(new Country { CountryCode = "IM", CountryName = "Isle of Man" });
			countries.Add(new Country { CountryCode = "IL", CountryName = "Israel" });
			countries.Add(new Country { CountryCode = "IT", CountryName = "Italy" });
			countries.Add(new Country { CountryCode = "JM", CountryName = "Jamaica" });
			countries.Add(new Country { CountryCode = "JP", CountryName = "Japan" });
			countries.Add(new Country { CountryCode = "JE", CountryName = "Jersey" });
			countries.Add(new Country { CountryCode = "JO", CountryName = "Jordan" });
			countries.Add(new Country { CountryCode = "KZ", CountryName = "Kazakhstan" });
			countries.Add(new Country { CountryCode = "KE", CountryName = "Kenya" });
			countries.Add(new Country { CountryCode = "KI", CountryName = "Kiribati" });
			countries.Add(new Country { CountryCode = "KP", CountryName = "Korea, Democratic People's Republic of" });
			countries.Add(new Country { CountryCode = "KR", CountryName = "Korea, Republic of" });
			countries.Add(new Country { CountryCode = "KW", CountryName = "Kuwait" });
			countries.Add(new Country { CountryCode = "KG", CountryName = "Kyrgyzstan" });
			countries.Add(new Country { CountryCode = "LA", CountryName = "Lao People's Democratic Republic" });
			countries.Add(new Country { CountryCode = "LV", CountryName = "Latvia" });
			countries.Add(new Country { CountryCode = "LB", CountryName = "Lebanon" });
			countries.Add(new Country { CountryCode = "LS", CountryName = "Lesotho" });
			countries.Add(new Country { CountryCode = "LR", CountryName = "Liberia" });
			countries.Add(new Country { CountryCode = "LY", CountryName = "Libyan Arab Jamahiriya" });
			countries.Add(new Country { CountryCode = "LI", CountryName = "Liechtenstein" });
			countries.Add(new Country { CountryCode = "LT", CountryName = "Lithuania" });
			countries.Add(new Country { CountryCode = "LU", CountryName = "Luxembourg" });
			countries.Add(new Country { CountryCode = "MO", CountryName = "Macao" });
			countries.Add(new Country { CountryCode = "MK", CountryName = "Macedonia, The Former Yugoslav Republic of" });
			countries.Add(new Country { CountryCode = "MG", CountryName = "Madagascar" });
			countries.Add(new Country { CountryCode = "MW", CountryName = "Malawi" });
			countries.Add(new Country { CountryCode = "MY", CountryName = "Malaysia" });
			countries.Add(new Country { CountryCode = "MV", CountryName = "Maldives" });
			countries.Add(new Country { CountryCode = "ML", CountryName = "Mali" });
			countries.Add(new Country { CountryCode = "MT", CountryName = "Malta" });
			countries.Add(new Country { CountryCode = "MH", CountryName = "Marshall Islands" });
			countries.Add(new Country { CountryCode = "MQ", CountryName = "Martinique" });
			countries.Add(new Country { CountryCode = "MR", CountryName = "Mauritania" });
			countries.Add(new Country { CountryCode = "MU", CountryName = "Mauritius" });
			countries.Add(new Country { CountryCode = "YT", CountryName = "Mayotte" });
			countries.Add(new Country { CountryCode = "MX", CountryName = "Mexico" });
			countries.Add(new Country { CountryCode = "FM", CountryName = "Micronesia, Federated States of" });
			countries.Add(new Country { CountryCode = "MD", CountryName = "Moldova, Republic of" });
			countries.Add(new Country { CountryCode = "MC", CountryName = "Monaco" });
			countries.Add(new Country { CountryCode = "MN", CountryName = "Mongolia" });
			countries.Add(new Country { CountryCode = "ME", CountryName = "Montenegro" });
			countries.Add(new Country { CountryCode = "MS", CountryName = "Montserrat" });
			countries.Add(new Country { CountryCode = "MA", CountryName = "Morocco" });
			countries.Add(new Country { CountryCode = "MZ", CountryName = "Mozambique" });
			countries.Add(new Country { CountryCode = "MM", CountryName = "Myanmar" });
			countries.Add(new Country { CountryCode = "NA", CountryName = "Namibia" });
			countries.Add(new Country { CountryCode = "NR", CountryName = "Nauru" });
			countries.Add(new Country { CountryCode = "NP", CountryName = "Nepal" });
			countries.Add(new Country { CountryCode = "NL", CountryName = "Netherlands" });
			countries.Add(new Country { CountryCode = "AN", CountryName = "Netherlands Antilles" });
			countries.Add(new Country { CountryCode = "NC", CountryName = "New Caledonia" });
			countries.Add(new Country { CountryCode = "NZ", CountryName = "New Zealand" });
			countries.Add(new Country { CountryCode = "NI", CountryName = "Nicaragua" });
			countries.Add(new Country { CountryCode = "NE", CountryName = "Niger" });
			countries.Add(new Country { CountryCode = "NG", CountryName = "Nigeria" });
			countries.Add(new Country { CountryCode = "NU", CountryName = "Niue" });
			countries.Add(new Country { CountryCode = "NF", CountryName = "Norfolk Island" });
			countries.Add(new Country { CountryCode = "MP", CountryName = "Northern Mariana Islands" });
			countries.Add(new Country { CountryCode = "NO", CountryName = "Norway" });
			countries.Add(new Country { CountryCode = "OM", CountryName = "Oman" });
			countries.Add(new Country { CountryCode = "PK", CountryName = "Pakistan" });
			countries.Add(new Country { CountryCode = "PW", CountryName = "Palau" });
			countries.Add(new Country { CountryCode = "PS", CountryName = "Palestinian Territory, Occupied" });
			countries.Add(new Country { CountryCode = "PA", CountryName = "Panama" });
			countries.Add(new Country { CountryCode = "PG", CountryName = "Papua New Guinea" });
			countries.Add(new Country { CountryCode = "PY", CountryName = "Paraguay" });
			countries.Add(new Country { CountryCode = "PE", CountryName = "Peru" });
			countries.Add(new Country { CountryCode = "PH", CountryName = "Philippines" });
			countries.Add(new Country { CountryCode = "PN", CountryName = "Pitcairn" });
			countries.Add(new Country { CountryCode = "PL", CountryName = "Poland" });
			countries.Add(new Country { CountryCode = "PT", CountryName = "Portugal" });
			countries.Add(new Country { CountryCode = "PR", CountryName = "Puerto Rico" });
			countries.Add(new Country { CountryCode = "QA", CountryName = "Qatar" });
			countries.Add(new Country { CountryCode = "RE", CountryName = "Reunion" });
			countries.Add(new Country { CountryCode = "RO", CountryName = "Romania" });
			countries.Add(new Country { CountryCode = "RU", CountryName = "Russian Federation" });
			countries.Add(new Country { CountryCode = "RW", CountryName = "Rwanda" });
			countries.Add(new Country { CountryCode = "SH", CountryName = "Saint Helena" });
			countries.Add(new Country { CountryCode = "KN", CountryName = "Saint Kitts and Nevis" });
			countries.Add(new Country { CountryCode = "LC", CountryName = "Saint Lucia" });
			countries.Add(new Country { CountryCode = "PM", CountryName = "Saint Pierre and Miquelon" });
			countries.Add(new Country { CountryCode = "VC", CountryName = "Saint Vincent and The Grenadines" });
			countries.Add(new Country { CountryCode = "WS", CountryName = "Samoa" });
			countries.Add(new Country { CountryCode = "SM", CountryName = "San Marino" });
			countries.Add(new Country { CountryCode = "ST", CountryName = "Sao Tome and Principe" });
			countries.Add(new Country { CountryCode = "SA", CountryName = "Saudi Arabia" });
			countries.Add(new Country { CountryCode = "SN", CountryName = "Senegal" });
			countries.Add(new Country { CountryCode = "RS", CountryName = "Serbia" });
			countries.Add(new Country { CountryCode = "SC", CountryName = "Seychelles" });
			countries.Add(new Country { CountryCode = "SL", CountryName = "Sierra Leone" });
			countries.Add(new Country { CountryCode = "SG", CountryName = "Singapore" });
			countries.Add(new Country { CountryCode = "SK", CountryName = "Slovakia" });
			countries.Add(new Country { CountryCode = "SI", CountryName = "Slovenia" });
			countries.Add(new Country { CountryCode = "SB", CountryName = "Solomon Islands" });
			countries.Add(new Country { CountryCode = "SO", CountryName = "Somalia" });
			countries.Add(new Country { CountryCode = "ZA", CountryName = "South Africa" });
			countries.Add(new Country { CountryCode = "GS", CountryName = "South Georgia and The South Sandwich Islands" });
			countries.Add(new Country { CountryCode = "ES", CountryName = "Spain" });
			countries.Add(new Country { CountryCode = "LK", CountryName = "Sri Lanka" });
			countries.Add(new Country { CountryCode = "SD", CountryName = "Sudan" });
			countries.Add(new Country { CountryCode = "SR", CountryName = "Suriname" });
			countries.Add(new Country { CountryCode = "SJ", CountryName = "Svalbard and Jan Mayen" });
			countries.Add(new Country { CountryCode = "SZ", CountryName = "Swaziland" });
			countries.Add(new Country { CountryCode = "SE", CountryName = "Sweden" });
			countries.Add(new Country { CountryCode = "CH", CountryName = "Switzerland" });
			countries.Add(new Country { CountryCode = "SY", CountryName = "Syrian Arab Republic" });
			countries.Add(new Country { CountryCode = "TW", CountryName = "Taiwan, Province of China" });
			countries.Add(new Country { CountryCode = "TJ", CountryName = "Tajikistan" });
			countries.Add(new Country { CountryCode = "TZ", CountryName = "Tanzania, United Republic of" });
			countries.Add(new Country { CountryCode = "TH", CountryName = "Thailand" });
			countries.Add(new Country { CountryCode = "TL", CountryName = "Timor-leste" });
			countries.Add(new Country { CountryCode = "TG", CountryName = "Togo" });
			countries.Add(new Country { CountryCode = "TK", CountryName = "Tokelau" });
			countries.Add(new Country { CountryCode = "TO", CountryName = "Tonga" });
			countries.Add(new Country { CountryCode = "TT", CountryName = "Trinidad and Tobago" });
			countries.Add(new Country { CountryCode = "TN", CountryName = "Tunisia" });
			countries.Add(new Country { CountryCode = "TR", CountryName = "Turkey" });
			countries.Add(new Country { CountryCode = "TM", CountryName = "Turkmenistan" });
			countries.Add(new Country { CountryCode = "TC", CountryName = "Turks and Caicos Islands" });
			countries.Add(new Country { CountryCode = "TV", CountryName = "Tuvalu" });
			countries.Add(new Country { CountryCode = "UG", CountryName = "Uganda" });
			countries.Add(new Country { CountryCode = "UA", CountryName = "Ukraine" });
			countries.Add(new Country { CountryCode = "AE", CountryName = "United Arab Emirates" });
			countries.Add(new Country { CountryCode = "GB", CountryName = "United Kingdom" });
			countries.Add(new Country { CountryCode = "US", CountryName = "United States" });
			countries.Add(new Country { CountryCode = "UM", CountryName = "United States Minor Outlying Islands" });
			countries.Add(new Country { CountryCode = "UY", CountryName = "Uruguay" });
			countries.Add(new Country { CountryCode = "UZ", CountryName = "Uzbekistan" });
			countries.Add(new Country { CountryCode = "VU", CountryName = "Vanuatu" });
			countries.Add(new Country { CountryCode = "VE", CountryName = "Venezuela" });
			countries.Add(new Country { CountryCode = "VN", CountryName = "Viet Nam" });
			countries.Add(new Country { CountryCode = "VG", CountryName = "Virgin Islands, British" });
			countries.Add(new Country { CountryCode = "VI", CountryName = "Virgin Islands, U.S." });
			countries.Add(new Country { CountryCode = "WF", CountryName = "Wallis and Futuna" });
			countries.Add(new Country { CountryCode = "EH", CountryName = "Western Sahara" });
			countries.Add(new Country { CountryCode = "YE", CountryName = "Yemen" });
			countries.Add(new Country { CountryCode = "ZM", CountryName = "Zambia" });
			countries.Add(new Country { CountryCode = "ZW", CountryName = "Zimbabwe" });

			return countries;
		}

		#endregion
	}
}