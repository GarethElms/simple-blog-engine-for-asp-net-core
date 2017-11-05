using SimpleBlogEngine.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace SimpleBlogEngine.Utility
{
	public class GoogleRecaptchaResult
	{
		public bool Success {get;set;}

		[JsonProperty("error-codes")]
		public List<string> Errors {get;set;}
	}

	public interface IGoogleRecaptcha
	{
		GoogleRecaptchaResult IsNotARobot(string recaptchaResponse, string remoteIP);
	}

	/// <summary>
	/// https://www.google.com/recaptcha/admin#site/338811890?setup
	/// </summary>
	public class GoogleRecaptcha : IGoogleRecaptcha
	{
		public GoogleRecaptchaSettings _settings {get;}
		private ILogger<GoogleRecaptcha> _logger {get;set;}
		public GoogleRecaptcha(IOptions<GoogleRecaptchaSettings> settings, ILogger<GoogleRecaptcha> logger)
		{
			_settings = settings.Value;
			_logger = logger;
		}

		public GoogleRecaptchaResult IsNotARobot(string recaptchaResponse, string remoteIP)
		{
			using(HttpClient client = new HttpClient())
			{
				client.BaseAddress = new Uri("https://www.google.com");
				var contentText = "secret=" + _settings.SecretKey + "&response=" + recaptchaResponse + "&remoteip=" + remoteIP;

				 var content = new FormUrlEncodedContent(new[] {
               new KeyValuePair<string, string>("secret", _settings.SecretKey),
               new KeyValuePair<string, string>("response", recaptchaResponse),
					new KeyValuePair<string, string>("remoteip", remoteIP)
            });
				HttpResponseMessage response = client.PostAsync("/recaptcha/api/siteverify", content).Result;
				_logger.LogInformation("Google Recaptcha : " + contentText);
				var result = JsonConvert.DeserializeObject<GoogleRecaptchaResult>(response.Content.ReadAsStringAsync().Result); // {"success": false,"error-codes": ["missing-input-response",  "missing-input-secret"]}
				return result;
			}
		}
	}
}
