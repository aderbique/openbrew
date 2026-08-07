using System;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.IO;
using System.Net.Security;
using System.Text;
using System.Linq;
using ctorx.Core.Collections;

namespace ctorx.Core.Email
{
	public class SmtpEmailSender : IEmailSender
	{
		readonly ISmtpConfiguration SMTPConfiguration;

		public SmtpEmailSender(ISmtpConfiguration smtpConfiguration)
		{
			this.SMTPConfiguration = smtpConfiguration;
		}

		/// <summary>
		/// Sends the provided email message
		/// </summary>
		public void Send(IEmailMessage emailMessage)
		{
			if (emailMessage == null)
			{
				throw new ArgumentNullException("emailMessage");
			}

            // Fail Gracefully if SMTP Settings are not set
		    if(string.IsNullOrWhiteSpace(this.SMTPConfiguration.Host) || this.SMTPConfiguration.Port == 0)
		    {
		        return;
		    }

			if (this.SMTPConfiguration.Port == 465)
			{
				this.SendImplicitTls(emailMessage);
				return;
			}

			var message = new MailMessage();

			// Set Sender
			message.From = new MailAddress(emailMessage.SenderAddress, emailMessage.SenderDisplayName);

			// Set Recipients
			emailMessage.ToRecipients.ForEach(x => message.To.Add(x));
			emailMessage.CcRecipients.ForEach(x => message.CC.Add(x));
			emailMessage.BccRecipients.ForEach(x => message.Bcc.Add(x));
			emailMessage.ReplyToRecipients.ForEach(x => message.ReplyToList.Add(x));

			// Set Content
			message.IsBodyHtml = emailMessage.FormatAsHtml;
			message.Body = emailMessage.BuildMessageBody();
			message.Subject = emailMessage.Subject;

			// Set Attachments
			if(emailMessage.Attachments.Count > 0)
			{
				emailMessage.Attachments.ForEach(x =>
				{
					var attachment = new Attachment(x.GetContentStream(), x.Name);
					message.Attachments.Add(attachment);
				});
			}

			// Set SMTP Settings
			var smtpClient = new SmtpClient(this.SMTPConfiguration.Host, this.SMTPConfiguration.Port);
			smtpClient.EnableSsl = this.SMTPConfiguration.EnableSSL;

			// Set Credentials
			if (!string.IsNullOrWhiteSpace(this.SMTPConfiguration.Username) || !string.IsNullOrWhiteSpace(this.SMTPConfiguration.Password))
			{
				smtpClient.UseDefaultCredentials = this.SMTPConfiguration.UseDefaultCredentials;
				smtpClient.Credentials = new NetworkCredential(this.SMTPConfiguration.Username, this.SMTPConfiguration.Password);
			}

			// Send Message
			smtpClient.Send(message);
		}

		void SendImplicitTls(IEmailMessage emailMessage)
		{
			using (var client = new TcpClient(this.SMTPConfiguration.Host, this.SMTPConfiguration.Port))
			using (var stream = new SslStream(client.GetStream()))
			{
				stream.AuthenticateAsClient(this.SMTPConfiguration.Host);
				using (var reader = new StreamReader(stream, Encoding.ASCII))
				using (var writer = new StreamWriter(stream, Encoding.ASCII) { NewLine = "\r\n", AutoFlush = true })
				{
					Expect(reader, 220);
					Command(writer, reader, "EHLO openbrew.local", 250);
					if (!string.IsNullOrWhiteSpace(this.SMTPConfiguration.Username) || !string.IsNullOrWhiteSpace(this.SMTPConfiguration.Password))
					{
						Command(writer, reader, "AUTH LOGIN", 334);
						Command(writer, reader, Convert.ToBase64String(Encoding.UTF8.GetBytes(this.SMTPConfiguration.Username ?? "")), 334);
						Command(writer, reader, Convert.ToBase64String(Encoding.UTF8.GetBytes(this.SMTPConfiguration.Password ?? "")), 235);
					}
					Command(writer, reader, "MAIL FROM:<" + emailMessage.SenderAddress + ">", 250);
					emailMessage.ToRecipients.ForEach(x => Command(writer, reader, "RCPT TO:<" + x + ">", 250, 251));
					Command(writer, reader, "DATA", 354);
					writer.WriteLine("From: " + emailMessage.SenderDisplayName + " <" + emailMessage.SenderAddress + ">");
					writer.WriteLine("To: " + string.Join(", ", emailMessage.ToRecipients));
					writer.WriteLine("Subject: " + emailMessage.Subject);
					writer.WriteLine("MIME-Version: 1.0");
					writer.WriteLine("Content-Type: " + (emailMessage.FormatAsHtml ? "text/html" : "text/plain") + "; charset=utf-8");
					writer.WriteLine("Content-Transfer-Encoding: 8bit");
					writer.WriteLine();
					writer.WriteLine((emailMessage.BuildMessageBody() ?? "").Replace("\n.", "\n.."));
					writer.WriteLine(".");
					Expect(reader, 250);
					Command(writer, reader, "QUIT", 221);
				}
			}
		}

		static void Command(StreamWriter writer, StreamReader reader, string command, params int[] expectedCodes)
		{
			writer.WriteLine(command);
			Expect(reader, expectedCodes);
		}

		static void Expect(StreamReader reader, params int[] expectedCodes)
		{
			string line, last = null;
			do { line = reader.ReadLine(); if (line == null) throw new IOException("SMTP server closed the connection."); last = line; } while (line.Length > 3 && line[3] == '-');
			int code;
			if (last == null || last.Length < 3 || !int.TryParse(last.Substring(0, 3), out code) || !expectedCodes.Contains(code)) throw new SmtpException(last ?? "No SMTP response.");
		}
	}
}
