using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBlogEngine.Models
{
    public class BlogPostsBaseViewModel
    {
		public BlogPostsBaseViewModel(BlogPostsSettings blogPostConfigMonitor)
		{
			BlogPostsConfig = blogPostConfigMonitor;
		}

		public BlogPostsSettings BlogPostsConfig {get;set;}
    }
}
