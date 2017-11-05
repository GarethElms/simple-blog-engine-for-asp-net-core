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
	public class BlogController:BaseController
	{
		private BlogPostsSettings _blogPostsConfig;
		public BlogController(
			IHostingEnvironment hostingEnvironment,
			IOptionsMonitor<BlogPostsSettings> blogPostsConfig) : base(hostingEnvironment)
		{
			_blogPostsConfig = blogPostsConfig.CurrentValue;
		}

		public IActionResult ViewBlogPost(int year, int month, string slug)
		{
			var blogPost = _blogPostsConfig.Blogs.SingleOrDefault(b => b.Slug == slug);
			if(blogPost == null)
			{
				return this.NotFound();
			}
			return View(blogPost);
		}

		public IActionResult About()
		{
			ViewData["Message"] = "Your application description page.";

			return View();
		}

		public IActionResult Contact()
		{
			ViewData["Message"] = "Your contact page.";

			return View();
		}

		public IActionResult All()
		{
			return View();
		}

		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
