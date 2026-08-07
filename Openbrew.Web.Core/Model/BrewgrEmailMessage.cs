using System;
using System.Linq;
using System.Text;
using Openbrew.Web.Core.Configuration;
using ctorx.Core.Email;

namespace Openbrew.Web.Core.Model
{
	public abstract class BrewgrEmailMessage : AbstractEmailMessage
	{
		readonly IWebSettings WebSettings;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public BrewgrEmailMessage(IWebSettings webSettings)
		{
			this.WebSettings = webSettings;
		}

		/// <summary>
		/// Gets the subscription notice
		/// </summary>
		protected string GetSubscriptionNotice()
		{
			var message = new StringBuilder();

			message.AppendLine();

			message.AppendLine(new string('-', 100));
			message.AppendLine();

			message.AppendLine("You've received this message because you are currently signed up to receive this type of notification. ");
			message.AppendLine("If you would like to stop receiving these notifications, visit the following link to change your preferences.");
			message.AppendLine();
			message.AppendLine(this.WebSettings.RootPathSecure + "/settings#notifications");
			message.AppendLine();

			return message.ToString();
		}
	}
}