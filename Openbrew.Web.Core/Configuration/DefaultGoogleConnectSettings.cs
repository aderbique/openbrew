using System;

namespace Openbrew.Web.Core.Configuration
{
	public class DefaultGoogleConnectSettings : IGoogleConnectSettings
	{
		/// <summary>
		/// Gets or sets the ApplicationKey
		/// </summary>
		public string ApplicationKey
		{
			get { return Environment.GetEnvironmentVariable("Google_ApplicationKey") ?? string.Empty; }
		}

		/// <summary>
		/// Gets or sets the ApplicationSecret
		/// </summary>
		public string ApplicationSecret
		{
			get { return Environment.GetEnvironmentVariable("Google_ApplicationSecret") ?? string.Empty; }
		}
	}
}
