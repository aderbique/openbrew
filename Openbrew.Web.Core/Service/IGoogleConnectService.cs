using Openbrew.Web.Core.Model;

namespace Openbrew.Web.Core.Service
{
	public interface IGoogleConnectService
	{
		/// <summary>
		/// Gets user info from an oauth response code
		/// </summary>
		OAuthUserInfo GetUserInfoFromOAuthCode(string code, string loginUrl);

		/// <summary>
		/// Gets user info from a Google ID token
		/// </summary>
		OAuthUserInfo GetUserInfoFromIdToken(string idToken);
	}
}
