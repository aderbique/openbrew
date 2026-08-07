using System;
using Openbrew.Web.Core.Model;

namespace Openbrew.Web.Core.Service
{
	public interface IOAuthService
	{
		/// <summary>
		/// Gets user info from an Auth Code
		/// </summary>
		OAuthUserInfo GetUserInfoFromAuthCode(string stateToken, string authCode, string loginUrl);

		/// <summary>
		/// Gets the local user id for an oauth user id and provider
		/// </summary>
		int? GetLocalUserIdFromOAuthUserInfo(OAuthUserInfo oAuthUserInfo);

		/// <summary>
		/// Gets a local user by email address
		/// </summary>
		int? GetLocalUserIdFromEmailAddress(string emailAddress);

		/// <summary>
		/// Connects a local user to an oauthprovider
		/// </summary>
		void ConnectLocalUserToOAuthProvider(int userId, OAuthUserInfo oAuthUserInfo);

		/// <summary>
		/// Registers a new user using OAuth Info
		/// </summary>
		User RegisterNewUser(OAuthUserInfo oAuthUserInfo);
	}
}