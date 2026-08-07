using System;
using System.Linq;
using ctorx.Core.Configuration;
using ctorx.Core.Email;

namespace Openbrew.Web.Core.Configuration
{
	public class BrewgrSmtpConfiguration : ISmtpConfiguration
	{
		string Read(string environmentName, string legacyEnvironmentName, string appSettingName)
		{
			var value = Environment.GetEnvironmentVariable(environmentName);
			if (string.IsNullOrWhiteSpace(value))
			{
				value = Environment.GetEnvironmentVariable(legacyEnvironmentName);
			}
			return !string.IsNullOrWhiteSpace(value) ? value : ConfigReader.AppSettings.Read(appSettingName);
		}

	    /// <summary>
	    /// Gets the SMTP Host Name
	    /// </summary>
	    public string Host
	    {
	        get { return this.Read("SMTP_HOST", "SmtpHost", "SmtpHost"); }
	    } 

	    /// <summary>
		/// Gets the SMTP Port
		/// </summary>
		public int Port
		{
			get
			{
				int port;
				return int.TryParse(this.Read("SMTP_PORT", "SmtpPort", "SmtpPort"), out port) ? port : 0;
			}
		}

		/// <summary>
		/// Gets a value specifying if the
		/// SMTP server should use SSL
		/// </summary>
		public bool EnableSSL
		{
			get { return true; }
		}

		/// <summary>
		/// Gets a value specifying whether or not default credentials should
		/// be used.
		/// </summary>
		public bool UseDefaultCredentials
		{
			get { return false; }
		}

		/// <summary>
		/// Gets the username
		/// </summary>
		public string Username
		{
			get { return this.Read("SMTP_USERNAME", "SmtpUserName", "SmtpUserName"); }
		}

		/// <summary>
		/// Gets the password
		/// </summary>
		public string Password
		{
			get { return this.Read("SMTP_PASSWORD", "SmtpPassword", "SmtpPassword"); }
		}
	}
}
