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
	public class CategoryController:BaseController
	{
		private BlogPostsSettings _blogPostsConfig;
		public CategoryController(
			IHostingEnvironment hostingEnvironment,
			IOptionsMonitor<BlogPostsSettings> blogPostsConfig) : base(hostingEnvironment)
		{
			_blogPostsConfig = blogPostsConfig.CurrentValue;
		}

		public IActionResult Index(string category)
		{
			var model = new BlogPostsViewModel(_blogPostsConfig) {
				PageOfBlogPosts = _blogPostsConfig.GetBlogPostsWithTag(category),
				Category = category
			};
			return View(model);
		}
	}
}
