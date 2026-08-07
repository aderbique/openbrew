using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security;
using System.Text;
using System.Web.Script.Serialization;
using Openbrew.Web.Core.Configuration;
using Openbrew.Web.Core.Model;

namespace Openbrew.Web.Core.Service
{
	public class DefaultGoogleService : IGoogleConnectService
	{
		readonly IGoogleConnectSettings GoogleConnectSettings;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public DefaultGoogleService(IGoogleConnectSettings googleConnectSettings)
		{
			this.GoogleConnectSettings = googleConnectSettings;
		}

		/// <summary>
		/// Gets user info from an oauth response code
		/// </summary>
		public OAuthUserInfo GetUserInfoFromOAuthCode(string code, string loginUrl)
		{
			var accessToken = this.AcquireAccessTokenFromAuthCode(code, loginUrl);
			var userInfo = this.GetUserInfo(accessToken);

			return new OAuthUserInfo
			{
				OAuthUserId = GetString(userInfo, "id"),
				EmailAddress = GetString(userInfo, "email"),
				FirstName = GetString(userInfo, "given_name"),
				LastName = GetString(userInfo, "family_name"),
				SourceProvider = OAuthProvider.Google
			};
		}

		/// <summary>
		/// Gets user info from an ID token
		/// </summary>
		public OAuthUserInfo GetUserInfoFromIdToken(string idToken)
		{
			var tokenInfo = this.GetTokenInfo(idToken);
			this.ValidateIdToken(tokenInfo);

			return new OAuthUserInfo
			{
				OAuthUserId = GetString(tokenInfo, "sub"),
				EmailAddress = GetString(tokenInfo, "email"),
				FirstName = GetString(tokenInfo, "given_name"),
				LastName = GetString(tokenInfo, "family_name"),
				SourceProvider = OAuthProvider.Google
			};
		}

		/// <summary>
		/// Acquires an access token from an auth code
		/// </summary>
		string AcquireAccessTokenFromAuthCode(string code, string loginUrl)
		{
			var request = (HttpWebRequest)WebRequest.Create("https://oauth2.googleapis.com/token");
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";

			var payload = string.Format(
				"code={0}&client_id={1}&client_secret={2}&redirect_uri={3}&grant_type=authorization_code",
				UrlEncode(code),
				UrlEncode(this.GoogleConnectSettings.ApplicationKey),
				UrlEncode(this.GoogleConnectSettings.ApplicationSecret),
				UrlEncode(loginUrl),
				UrlEncode("authorization_code"));

			var bytes = Encoding.UTF8.GetBytes(payload);
			request.ContentLength = bytes.Length;

			using (var requestStream = request.GetRequestStream())
			{
				requestStream.Write(bytes, 0, bytes.Length);
			}

			return GetString(ParseJson(ReadResponse(request)), "access_token");
		}

		Dictionary<string, object> GetUserInfo(string accessToken)
		{
			var request = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/oauth2/v2/userinfo");
			request.Method = "GET";
			request.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + accessToken);
			return ParseJson(ReadResponse(request));
		}

		Dictionary<string, object> GetTokenInfo(string idToken)
		{
			var request = (HttpWebRequest)WebRequest.Create("https://oauth2.googleapis.com/tokeninfo?id_token=" + UrlEncode(idToken));
			request.Method = "GET";
			return ParseJson(ReadResponse(request));
		}

		void ValidateIdToken(Dictionary<string, object> tokenInfo)
		{
			var audience = GetString(tokenInfo, "aud");
			var issuer = GetString(tokenInfo, "iss");
			var emailVerified = GetString(tokenInfo, "email_verified");

			if (!string.Equals(audience, this.GoogleConnectSettings.ApplicationKey, StringComparison.Ordinal))
			{
				throw new SecurityException("Google ID token audience does not match the configured client id.");
			}

			if (!string.Equals(issuer, "accounts.google.com", StringComparison.Ordinal) &&
				!string.Equals(issuer, "https://accounts.google.com", StringComparison.Ordinal))
			{
				throw new SecurityException("Google ID token issuer is not valid.");
			}

			if (!string.Equals(emailVerified, "true", StringComparison.OrdinalIgnoreCase))
			{
				throw new SecurityException("Google email address has not been verified.");
			}
		}

		static Dictionary<string, object> ParseJson(string json)
		{
			return new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(json);
		}

		static string GetString(Dictionary<string, object> values, string key)
		{
			object value;
			return values != null && values.TryGetValue(key, out value) ? Convert.ToString(value) : null;
		}

		static string ReadResponse(WebRequest request)
		{
			using (var response = (HttpWebResponse)request.GetResponse())
			using (var stream = response.GetResponseStream())
			using (var reader = new StreamReader(stream))
			{
				return reader.ReadToEnd();
			}
		}

		static string UrlEncode(string value)
		{
			return Uri.EscapeDataString(value ?? string.Empty);
		}
	}
}
