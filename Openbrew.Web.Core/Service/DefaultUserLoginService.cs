using System;
using System.Data.Entity;
using System.Linq;
using ctorx.Core.Crypto;
using ctorx.Core.Data;
using ctorx.Core.Identity;
using Openbrew.Web.Core.Data;
using Openbrew.Web.Core.Model;
using AutoMapper;

namespace Openbrew.Web.Core.Service
{
	public class DefaultUserLoginService : IUserLoginService
	{
		readonly IBrewgrRepository Repository;
		readonly IHasher Hasher;
		readonly IUnitOfWorkFactory<BrewgrContext> UnitOfWorkFactory;
		readonly IUserHostAddressResolver UserHostAddressResolver;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public DefaultUserLoginService(IBrewgrRepository repository, IHasher hasher, IUnitOfWorkFactory<BrewgrContext> unitOfWorkFactory, IUserHostAddressResolver userHostAddressResolver)
		{
			this.Repository = repository;
			this.Hasher = hasher;
			this.UnitOfWorkFactory = unitOfWorkFactory;
			this.UserHostAddressResolver = userHostAddressResolver;
		}

		/// <summary>
		/// Attempts to log the user in
		/// </summary>
		public bool Login(string emailOrUsername, string password, out UserSummary userSummary)
		{
			if (string.IsNullOrWhiteSpace(emailOrUsername))
			{
				throw new ArgumentNullException("emailOrUsername");
			}

			if (string.IsNullOrWhiteSpace(password))
			{
				throw new ArgumentNullException("password");
			}

			userSummary = null;

			var matchingUser = this.Repository.GetSet<User>()
				.Include(x => x.UserAdmin)
				.Include(x => x.UserPartnerAdmins)
				.Where(x => x.IsActive)
				.FirstOrDefault(x => x.Username == emailOrUsername || x.EmailAddress == emailOrUsername);

			if (matchingUser == null)
			{
				return false;
			}

			if (!this.Hasher.Compare(password, matchingUser.Password))
			{
				return false;
			}

			userSummary = Mapper.Map(matchingUser, userSummary);

			// Log Login and get Summary
			this.TrackLogin(matchingUser.UserId);

			return true;
		}

		/// <summary>
		/// Attempts to login a returning user 
		/// </summary>
		public bool ReturnLogin(int userId, out UserSummary userSummary)
		{
			if(userId <= 0)
			{
				throw new ArgumentOutOfRangeException("userId");
			}

			userSummary = null;

			var matchingUser = this.Repository.GetSet<User>()
				.Include(x => x.UserAdmin)
				.Include(x => x.UserPartnerAdmins)
				.FirstOrDefault(x => x.UserId == userId);

			if (matchingUser == null)
			{
				return false;
			}

			// Log Login and get Summary
			this.TrackLogin(matchingUser.UserId);
			userSummary = Mapper.Map(matchingUser, new UserSummary());

			return true;			
		}

		/// <summary>
		/// Tracks a User Login
		/// </summary>
		public void TrackLogin(int userId)
		{
			// Log the Login
			var userLogin = new UserLogin { UserId = userId, LoginDate = DateTime.Now, IPAddress = this.UserHostAddressResolver.Resolve() };
			this.Repository.Add(userLogin);
		}

		/// <summary>
		/// Creates a User Auth Token
		/// </summary>
		public string CreateUserAuthToken(string emailAddress)
		{
			var userId = this.Repository.GetSet<User>()
				.Where(x => x.EmailAddress == emailAddress)
				.Select(x => x.UserId)
				.Cast<int?>()
				.FirstOrDefault();

			if(userId == null)
			{
				return null;
			}

			// Random Characters (3 Guids with no dashes)
            // TODO: User true Random generator here
			var token = string.Concat(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString()).Replace("-", "");

			var userAuthToken = new UserAuthToken
			{
				UserId = userId.Value,
				AuthToken = token,
				ExpiryDate = DateTime.Now.AddHours(8)
			};

			this.Repository.Add(userAuthToken);

			return token;
		}

		/// <summary>
		/// Determines if an auth token is expired
		/// </summary>
		public bool AuthTokenIsExired(string authToken)
		{
			if (string.IsNullOrWhiteSpace(authToken))
			{
				throw new ArgumentNullException("authToken");
			}

			return this.Repository.GetSet<UserAuthToken>()
				.Where(x => x.AuthToken == authToken)
				.Any(x => x.ExpiryDate < DateTime.Now);
		}

		/// <summary>
		/// Sets a password for a user using an auth token as a key
		/// </summary>
		public void SetPasswordUsingAuthToken(string authToken, string password)
		{
			if(string.IsNullOrWhiteSpace(authToken))
			{
				throw new ArgumentNullException("authToken");
			}

			if(string.IsNullOrWhiteSpace(password))
			{
				throw new ArgumentNullException("password");
			}

			var user = this.Repository.GetSet<UserAuthToken>()
				.Where(x => x.AuthToken == authToken)
				.Where(x => x.ExpiryDate >= DateTime.Now)
				.Select(x => x.User)
				.FirstOrDefault();

			if(user == null)
			{
				throw new InvalidOperationException("Auth token does not map to a user");
			}

			// Hash the new Password
			user.Password = this.Hasher.Hash(password);
		}

		/// <summary>
		/// Sets a User Password
		/// </summary>
		public void SetUserPassword(int userId, string newPassword)
		{
			if(userId <= 0)
			{
				throw new ArgumentOutOfRangeException("userId");
			}

			if(string.IsNullOrWhiteSpace(newPassword))
			{
				throw new ArgumentNullException("newPassword");
			}

			var user = this.Repository.GetSet<User>()
				.FirstOrDefault(x => x.UserId == userId);

			user.Password = this.Hasher.Hash(newPassword);
		}

		/// <summary>
		/// Verifies a User Password
		///  </summary>
		public bool VerifyUserPassword(int userId, string currentPassword)
		{
			if (userId <= 0)
			{
				throw new ArgumentOutOfRangeException("userId");
			}

			if (string.IsNullOrWhiteSpace(currentPassword))
			{
				throw new ArgumentNullException("currentPassword");
			}


			var user = this.Repository.GetSet<User>()
				.FirstOrDefault(x => x.UserId == userId);

			return this.Hasher.Compare(currentPassword, user.Password);
		}
	}
}
