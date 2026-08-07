using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Openbrew.Web.Core.Configuration;
using Openbrew.Web.Core.Model;
using ctorx.Core.Email;

namespace Openbrew.Web.Email
{
	/// <summary>
	/// Notifies active site administrators when a brewer submits product feedback.
	/// </summary>
	public class FeedbackNotificationEmailMessage : AbstractEmailMessage
	{
		readonly IWebSettings WebSettings;
		readonly string Feedback;
		readonly UserSummary SubmittedBy;
		readonly string IpAddress;

		public FeedbackNotificationEmailMessage(IWebSettings webSettings, IEnumerable<string> recipients, string feedback, UserSummary submittedBy, string ipAddress)
		{
			this.WebSettings = webSettings;
			this.Feedback = feedback;
			this.SubmittedBy = submittedBy;
			this.IpAddress = ipAddress;
			this.SenderAddress = webSettings.SenderAddress;
			this.SenderDisplayName = webSettings.SenderDisplayName;
			this.Subject = "New OpenBrew feedback";
			this.FormatAsHtml = false;

			foreach (var recipient in recipients.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase))
			{
				this.ToRecipients.Add(recipient);
			}
		}

		public override string BuildMessageBody()
		{
			var message = new StringBuilder();
			var submittedBy = this.SubmittedBy == null
				? "Guest"
				: string.Format("{0} ({1})", this.SubmittedBy.FullName, this.SubmittedBy.EmailAddress);

			message.AppendLine("New feedback was submitted to OpenBrew.");
			message.AppendLine("------------------------------------------------------------");
			message.AppendLine("From: " + submittedBy);
			message.AppendLine("IP address: " + (this.IpAddress ?? "Unknown"));
			message.AppendLine("Submitted: " + DateTime.Now.ToString("f"));
			message.AppendLine();
			message.AppendLine("Feedback:");
			message.AppendLine(this.Feedback);
			message.AppendLine();
			message.AppendLine("Review it in Admin Tools: " + this.WebSettings.RootPathSecure.TrimEnd('/') + "/admin/tools");

			return message.ToString();
		}
	}
}
