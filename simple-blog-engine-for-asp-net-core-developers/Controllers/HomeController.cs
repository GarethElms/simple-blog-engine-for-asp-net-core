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
	public class HomeController:BaseController
	{
		private BlogPostsSettings _blogPostsConfig;

		public HomeController(
			IHostingEnvironment hostingEnvironment,
			IOptionsMonitor<BlogPostsSettings> blogPostsConfig) : base(hostingEnvironment)
		{
			_blogPostsConfig = blogPostsConfig.CurrentValue;
		}

		public IActionResult Index(int pageNumber = 1)
		{
			var model = new BlogPostsViewModel(_blogPostsConfig) {
				PageOfBlogPosts = _blogPostsConfig.GetPage(pageNumber),
				CurrentPageNumber = pageNumber};
			return View(model);
		}
		
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
