using System;
using System.Linq;
using Openbrew.Web.Core.Model;

namespace Openbrew.Web.Core.Service
{
	public interface IUserLoginService
	{
		/// <summary>
		/// Attempts to login a user
		/// </summary>
		bool Login(string emailOrUsername, string password, out UserSummary userSummary);
		
		/// <summary>
		/// Attempts to login a returning user 
		/// </summary>
		bool ReturnLogin(int userId, out UserSummary userSummary);

		/// <summary>
		/// Tracks a User Login
		/// </summary>
		void TrackLogin(int userId);

		/// <summary>
		/// Creates a User Auth Token
		/// </summary>
		string CreateUserAuthToken(string emailAddress);

		/// <summary>
		/// Determines if an auth token is expired
		/// </summary>
		bool AuthTokenIsExired(string authToken);

		/// <summary>
		/// Sets a password for a user using an auth token as a key
		/// </summary>
		void SetPasswordUsingAuthToken(string authToken, string password);

		/// <summary>
		/// Sets a User Password
		/// </summary>
		void SetUserPassword(int userId, string newPassword);

		/// <summary>
		/// Verifies a User Password
		///  </summary>
		bool VerifyUserPassword(int userId, string currentPassword);
	}
}