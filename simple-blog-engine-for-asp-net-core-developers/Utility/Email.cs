using SimpleBlogEngine.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBlogEngine.Utility
{
	public interface IEmailSender
	{
		bool SendEmail(string fromName, string fromEmail, string message, out string errorMessage);
	}

	public class Email:IEmailSender
	{
		public EmailSettings _emailSettings {get;}
		public SiteSettings _siteSettings {get;}
		public Email(IOptions<EmailSettings> emailSettings, IOptions<SiteSettings> siteSettings)
		{
			_emailSettings = emailSettings.Value;
			_siteSettings = siteSettings.Value;
		}

		public bool SendEmail(string fromName, string fromEmail, string message, out string errorMessage)
		{
			bool result = true;
			errorMessage = "";
			try
			{
				MailMessage mail = new MailMessage()
				{
					From = new MailAddress(_emailSettings.UsernameEmail, _siteSettings.Owner)
				};
				mail.To.Add(new MailAddress(_emailSettings.ToEmail));
				if(!string.IsNullOrEmpty(_emailSettings.CcEmail))
				{
					mail.CC.Add(new MailAddress(_emailSettings.CcEmail));
				}
				mail.Subject = "Contact from " + _siteSettings.SiteName + " from " + fromEmail;

				var body = new StringBuilder();
				body.Append("<h3 style='margin-bottom:0; padding-bottom:5px;'>Name</h3>");
				body.Append("<p style='margin-top:0; padding-top:0;'>");
				body.Append(fromName);
				body.Append("</p>");
				body.Append("<h3 style='margin-bottom:0; padding-bottom:5px;'>Email</h3>");
				body.Append("<p style='margin-top:0; padding-top:0;'>");
				body.Append(fromEmail);
				body.Append("</p>");
				body.Append("<h3 style='margin-bottom:0; padding-bottom:5px;'>Message</h3>");
				body.Append("<p style='margin-top:0; padding-top:0;'>");
				body.Append(message);
				body.Append("</p>");
				
				mail.Body = body.ToString();
				mail.IsBodyHtml = true;
				mail.Priority = MailPriority.High;
				using(SmtpClient smtp = new SmtpClient(_emailSettings.PrimaryDomain,_emailSettings.PrimaryPort))
				{
					smtp.Credentials = new NetworkCredential(_emailSettings.UsernameEmail,_emailSettings.UsernamePassword);
					smtp.EnableSsl = true;
					smtp.Send(mail);
				}
			}
			catch(Exception ex)
			{
				errorMessage = ex.Message;
				result = false;
			}

			return result;
		}
	}
}
