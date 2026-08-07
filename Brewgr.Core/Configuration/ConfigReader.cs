using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ctorx.Core.Configuration
{
	public static class ConfigReader
	{
		/// <summary>
		/// Reads App Settings
		/// </summary>
		public static class AppSettings
		{
			static string NormalizeEnvironmentKey(string key)
			{
				var normalized = Regex.Replace(key, "([a-z0-9])([A-Z])", "$1_$2");
				normalized = normalized.Replace("-", "_").Replace(".", "_");
				return normalized.ToUpperInvariant();
			}

			/// <summary>
			/// Reads an app setting from the confioguration file
			/// </summary>
			public static string Read(string key)
			{
				var normalizedKey = NormalizeEnvironmentKey(key);
				var fileValue = new[]
				{
					Environment.GetEnvironmentVariable(key + "_FILE"),
					Environment.GetEnvironmentVariable("Openbrew_" + key + "_FILE"),
					Environment.GetEnvironmentVariable(normalizedKey + "_FILE"),
					Environment.GetEnvironmentVariable("OPENBREW_" + normalizedKey + "_FILE"),
					Environment.GetEnvironmentVariable(key.ToUpperInvariant() + "_FILE"),
					Environment.GetEnvironmentVariable(("OPENBREW_" + key).ToUpperInvariant() + "_FILE"),
					Environment.GetEnvironmentVariable("Brewgr_" + key + "_FILE"),
					Environment.GetEnvironmentVariable("BREWGR_" + normalizedKey + "_FILE"),
					Environment.GetEnvironmentVariable(("BREWGR_" + key).ToUpperInvariant() + "_FILE")
				}.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));

				if (!string.IsNullOrWhiteSpace(fileValue) && File.Exists(fileValue))
				{
					return File.ReadAllText(fileValue).Trim();
				}

				var envValue = new[]
				{
					Environment.GetEnvironmentVariable(key),
					Environment.GetEnvironmentVariable("Openbrew_" + key),
					Environment.GetEnvironmentVariable(normalizedKey),
					Environment.GetEnvironmentVariable("OPENBREW_" + normalizedKey),
					Environment.GetEnvironmentVariable(key.ToUpperInvariant()),
					Environment.GetEnvironmentVariable(("OPENBREW_" + key).ToUpperInvariant()),
					Environment.GetEnvironmentVariable("Brewgr_" + key),
					Environment.GetEnvironmentVariable("BREWGR_" + normalizedKey),
					Environment.GetEnvironmentVariable(("BREWGR_" + key).ToUpperInvariant())
				}.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));

				if (!string.IsNullOrWhiteSpace(envValue))
				{
					return envValue;
				}

				return ConfigurationManager.AppSettings[key];
			}
		}

		/// <summary>
		/// Reads environment variables with an optional legacy fallback.
		/// </summary>
		public static class EnvironmentVariables
		{
			public static string Read(string name, string legacyName = null)
			{
				var value = Environment.GetEnvironmentVariable(name);
				if (!string.IsNullOrWhiteSpace(value))
				{
					return value;
				}

				if (!string.IsNullOrWhiteSpace(legacyName))
				{
					value = Environment.GetEnvironmentVariable(legacyName);
					if (!string.IsNullOrWhiteSpace(value))
					{
						return value;
					}
				}

				return null;
			}

			public static int ReadInt(string name, int fallback, string legacyName = null)
			{
				int parsed;
				if (int.TryParse(Read(name, legacyName), out parsed))
				{
					return parsed;
				}

				return fallback;
			}

			public static bool ReadBool(string name, bool fallback, string legacyName = null)
			{
				bool parsed;
				if (bool.TryParse(Read(name, legacyName), out parsed))
				{
					return parsed;
				}

				return fallback;
			}
		}

		/// <summary>
		/// Reads Connection Strings
		/// </summary>
		public static class ConnectionStrings
		{
			/// <summary>
			/// Reads a Connection String from the configuration file
			/// </summary>
			public static string Read(string name)
			{
				return ConfigurationManager.ConnectionStrings[name].ConnectionString;
			}
		}
	}
}
