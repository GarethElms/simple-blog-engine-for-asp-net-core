using SimpleBlogEngine.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlogEngine.Models
{
	public class GoogleRecaptchaSettings
	{
		public bool IsEnabled {get;set;}
		public string SiteKey {get;set;}
		public string SecretKey {get;set;}
	}
} 
