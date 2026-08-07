using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ctorx.Core.Crypto;
using Openbrew.Web.Core.Data;
using Openbrew.Web.Core.Model;

namespace Openbrew.Web.Core.Service
{
	public class DefaultOAuthService : IOAuthService
	{
		readonly IGoogleConnectService GoogleService;
		readonly IBrewgrRepository Repository;
		readonly IHasher Hasher;
		readonly IUserService UserService;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public DefaultOAuthService(IGoogleConnectService googleService, IBrewgrRepository repository, IHasher hasher, IUserService userService)
		{
			this.GoogleService = googleService;
			this.Repository = repository;
			this.Hasher = hasher;
			this.UserService = userService;

			Mapper.CreateMap<OAuthUserInfo, User>();
		}

		/// <summary>
		/// Gets user info from an Auth Code
		/// </summary>
		public OAuthUserInfo GetUserInfoFromAuthCode(string stateToken, string authCode, string loginUrl)
		{
			// The auth state token carries the provider prefix so we can route the callback.

			var providerKey = stateToken.Split('-').First().ToLower();

			switch (providerKey)
			{
				case "google":
					return this.GoogleService.GetUserInfoFromOAuthCode(authCode, loginUrl);

				default:
					throw new NotSupportedException("OAuth provider key not recognized");
			}
		}

		/// <summary>
		/// Gets the local user id for an oauth user id and provider
		/// </summary>
		public int? GetLocalUserIdFromOAuthUserInfo(OAuthUserInfo oAuthUserInfo)
		{
			return this.Repository.GetSet<UserOAuthUserId>()
				.Where(x => x.OAuthProviderId == (int)oAuthUserInfo.SourceProvider)
				.Where(x => x.OAuthUserId == oAuthUserInfo.OAuthUserId)
				.Select(x => x.UserId)
				.Cast<int?>()
				.FirstOrDefault();
		}

		/// <summary>
		/// Gets a local user by email address
		/// </summary>
		public int? GetLocalUserIdFromEmailAddress(string emailAddress)
		{
			return this.Repository.GetSet<User>()
				.Where(x => x.EmailAddress == emailAddress)
				.Select(x => x.UserId)
				.Cast<int?>()
				.FirstOrDefault();
		}

		/// <summary>
		/// Connects a local user to an oauthprovider
		/// </summary>
		public void ConnectLocalUserToOAuthProvider(int userId, OAuthUserInfo oAuthUserInfo)
		{
			var userOAuthUserId = new UserOAuthUserId
			{
				UserId = userId,
				OAuthProviderId = (int)oAuthUserInfo.SourceProvider,
				OAuthUserId = oAuthUserInfo.OAuthUserId
			};

			this.Repository.Add(userOAuthUserId);
		}

		/// <summary>
		/// Registers a new user using OAuth Info
		/// </summary>
		public User RegisterNewUser(OAuthUserInfo oAuthUserInfo)
		{
			if (oAuthUserInfo == null)
			{
				throw new ArgumentNullException("oAuthUserInfo");
			}

			var user = Mapper.Map(oAuthUserInfo, new User());
			user.Password = this.Hasher.Hash(Guid.NewGuid().ToString());
			user.IsActive = true;
			user.DateCreated = DateTime.Now;
			user.Username = Guid.NewGuid().ToString().Replace("-", "");
			user.HasCustomUsername = false;

			// Connect the User
			user.UserOAuthUserIds = new List<UserOAuthUserId>
			{
			    new UserOAuthUserId
			        {
			            OAuthProviderId = (int) oAuthUserInfo.SourceProvider,
			            OAuthUserId = oAuthUserInfo.OAuthUserId
			        }
			};

			this.UserService.SubscribeUserToDefaultNotifications(user);

			this.Repository.Add(user);
			
			return user;
		}
	}
}
