using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SimpleBlogEngine.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using SimpleBlogEngine.Utility;

namespace SimpleBlogEngine.Controllers
{
	public class ContactController : BaseController
	{
		private EmailSettings _emailSettings;
		private GoogleRecaptchaSettings _googleRecaptchaSettings;
		private BlogPostsSettings _blogPostsConfig;
		private IHostingEnvironment _hostingEnvironment;
		private IGoogleRecaptcha _googleRecaptcha;
		private IEmailSender _emailSender;

		public ContactController(
			IHostingEnvironment hostingEnvironment,
			IOptionsMonitor<BlogPostsSettings> blogPostsConfig,
			IOptions<EmailSettings>emailSettings,
			IOptions<GoogleRecaptchaSettings> googleRecaptchaSettings,
			IGoogleRecaptcha googleRecaptcha,
			IEmailSender emailSender) : base(hostingEnvironment)
		{
			_hostingEnvironment = hostingEnvironment;
			_blogPostsConfig = blogPostsConfig.CurrentValue;
			_emailSettings = emailSettings.Value;
			_emailSender = emailSender;
			_googleRecaptchaSettings = googleRecaptchaSettings.Value;
			_googleRecaptcha = googleRecaptcha;
		}

		public IActionResult Index(string success)
		{
			ViewBag.Success = success;
			return View(_blogPostsConfig);
		}

		[HttpPost]
		public IActionResult SendEmail(string fromName, string fromEmail, string message, [ModelBinder(Name="g-recaptcha-response")]string recaptchaResponse, bool isAJAX = false)
      {
			var sendEmail = true;
			var success = false;
			var errorMessage = "";
			if(_googleRecaptchaSettings.IsEnabled)
			{
				var recaptchaResult = _googleRecaptcha.IsNotARobot(recaptchaResponse, Request.HttpContext.Connection.RemoteIpAddress.ToString());
				sendEmail = recaptchaResult.Success;
				if(!recaptchaResult.Success)
				{
					errorMessage = "Recaptcha error : ";
					if(recaptchaResult.Errors != null && recaptchaResult.Errors.Count > 0)
					{
						foreach(var error in recaptchaResult.Errors)
						{
							errorMessage += "[" + error + "] ";
						}
					}
				}
			}
			if(sendEmail)
			{
				if(_emailSender.SendEmail(fromName, fromEmail, message, out errorMessage))
				{
					success = true;
				}
			}
			if(isAJAX)
			{
				if(success)
				{
					return new JsonResult(success);
				}
				else
				{
					// Log the error
					return StatusCode(500, new {error="There was an error sending your message.", detail=errorMessage});
				}
			}
			else
			{
				return Redirect("/Contact?success=" + success.ToString().ToLower());
			}
      }
	}
}
