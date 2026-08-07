using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ctorx.Core.Configuration;

namespace Openbrew.Web.Core.Configuration
{
	public class DevWebSettings : AbstractWebSettings
	{
		static string GetEnvironmentValue(string name, string fallback, string legacyName = null)
		{
			var value = ConfigReader.EnvironmentVariables.Read(name, legacyName);
			return string.IsNullOrWhiteSpace(value) ? fallback : value;
		}

		/// <summary>
		/// Gets the RootPath
		/// </summary>
		public override string RootPath
		{
			get { return GetEnvironmentValue("OPENBREW_ROOT_URL", "http://127.0.0.1:8085", "Brewgr_RootUrl"); }
		}

		/// <summary>
		/// Gets the RootPathSecure
		/// </summary>
		public override string RootPathSecure
		{
			get { return GetEnvironmentValue("OPENBREW_ROOT_URL_SECURE", this.RootPath, "Brewgr_RootUrlSecure"); }
		}

		/// <summary>
		/// Gets the static root path
		/// </summary>
		public override string StaticRootPath
		{
			get { return GetEnvironmentValue("OPENBREW_STATIC_ROOT_URL", this.RootPath, "Brewgr_StaticRootUrl"); }
		}

		/// <summary>
		/// Gets the secure static root path
		/// </summary>
		public override string StaticRootPathSecure
		{
			get { return GetEnvironmentValue("OPENBREW_STATIC_ROOT_URL_SECURE", this.StaticRootPath, "Brewgr_StaticRootUrlSecure"); }
		}

		/// <summary>
		/// Gets a value specifying whether or not https is disabled
		/// </summary>
		public override bool DisableHttps
		{
			get { return true; }
		}

		/// <summary>
		/// Gets the MediaPhysicalRoot
		/// </summary>
		public override string MediaPhysicalRoot
		{
			get
			{
				var envRoot = Environment.GetEnvironmentVariable("OPENBREW_MEDIA_PHYSICAL_ROOT") ?? Environment.GetEnvironmentVariable("Setting_MediaPhysicalRoot");
				if (!string.IsNullOrWhiteSpace(envRoot))
				{
					return envRoot;
				}

				var candidates = new[]
				{
					Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Media")),
					Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "Media"))
				};

				return candidates.FirstOrDefault(Directory.Exists) ?? candidates[0];
			}
		}
	}
}
