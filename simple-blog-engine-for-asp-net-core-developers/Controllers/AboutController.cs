using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SimpleBlogEngine.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace SimpleBlogEngine.Controllers
{
	public class AboutController:BaseController
	{
		private BlogPostsSettings _blogPostsConfig;

		public AboutController(
			IHostingEnvironment hostingEnvironment,
			IOptionsMonitor<BlogPostsSettings> blogPostsConfig) : base(hostingEnvironment)
		{
			_blogPostsConfig = blogPostsConfig.CurrentValue;
		}

		public IActionResult Index()
		{
			return View(_blogPostsConfig);
		}
	}
}
