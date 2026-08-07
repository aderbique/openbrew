using System;
using System.Collections.Generic;
using System.Linq;

namespace Openbrew.Web.Core.Configuration
{
	public abstract class AbstractWebSettings : IWebSettings
	{
		/// <summary>
		/// Gets the RootPath
		/// </summary>
		public abstract string RootPath { get; }

		/// <summary>
		/// Gets the RootPathSecure
		/// </summary>
		public abstract string RootPathSecure { get;  }

		/// <summary>
		/// Gets the static root path
		/// </summary>
		public abstract string StaticRootPath { get;  }

		/// <summary>
		/// Gets the secure static root path
		/// </summary>
		public abstract string StaticRootPathSecure { get;  }

		/// <summary>
		/// Gets a value specifying whether or not https is disabled
		/// </summary>
		public abstract bool DisableHttps { get; }

		/// <summary>
		/// Gets the MediaPhysicalRoot
		/// </summary>
		public abstract string MediaPhysicalRoot { get;  }

		/// <summary>
		/// Gets the MediaUrlRoot
		/// </summary>
		public string MediaUrlRoot 
		{ 
			get { return this.RootPath + "/Media"; }
		}

		/// <summary>
		/// Gets the MediaUrlRoot Secure
		/// </summary>
		public string MediaUrlRootSecure
		{
			get { return this.RootPathSecure + "/Media"; }
		}

		/// <summary>
		/// Gets or sets the SenderName
		/// </summary>
		public virtual string SenderDisplayName
		{
			get { return "OpenBrew"; }
		}

		/// <summary>
		/// Gets or sets the SenderAddress
		/// </summary>
		public virtual string SenderAddress
		{
			get { return "info@openbrew.net"; }
		}

		/// <summary>
		/// Gets the contact form Email Address
		/// </summary>
		public virtual IList<string> ContactFormEmailAddress
		{
			get
			{
				var configuredRecipients = ctorx.Core.Configuration.ConfigReader.EnvironmentVariables.Read(
					"OPENBREW_CONTACT_EMAIL_ADDRESS",
					"Brewgr_ContactFormEmailAddress");

				if (string.IsNullOrWhiteSpace(configuredRecipients))
				{
					return new[] { "support@openbrew.dev" };
				}

				return configuredRecipients
					.Split(new[] { ',', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
					.Select(x => x.Trim())
					.Where(x => !string.IsNullOrWhiteSpace(x))
					.ToArray();
			}
		}

		/// <summary>
		/// Gets the default number of Recipes per page
		/// </summary>
		public int DefaultRecipesPerPage
		{
			get { return 10; }
		}

		/// <summary>
		/// Gets the default image root
		/// </summary>
		public string DefaultRecipeImageRoot
		{
			get { return "/img/mug/"; }
		}
	}
}
