using SimpleBlogEngine.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlogEngine.Models
{
	public class BlogPostTag
	{
		public string Tag {get;set;}
		public string Tag_URLFriendly {get;set;}
		public int Count {get;set;}
	}
	public class BlogPostsSettings_MetaData
	{
		public int TotalBlogPosts {get;set;}
		public int TotalPages {get;set;}
		public List<BlogPostTag> Tags {get;set;}
	}

	public class BlogPostsSettings
	{
		public int BlogsPerPage {get;set;}
		public int NumberOfRecentBlogPostsToShowInSidebar {get;set;}
		public BlogPostsSettings_MetaData Meta {get;set;}
		public List<BlogPost> Blogs {get;set;}

		public BlogPostsSettings CalculateMetaData()
		{
			Blogs = Blogs.Where(b => b.Published).OrderByDescending(b => new DateTime(b.Date.Year, b.Date.Month, b.Date.Day)).ToList(); // Do I need Blogs object lock() here or does asp.net core do that for me?

			if(Meta == null) {
				Meta = new BlogPostsSettings_MetaData();
			}
			Meta.TotalBlogPosts = Blogs.Count;
			Meta.TotalPages = (Meta.TotalBlogPosts + BlogsPerPage - 1) / BlogsPerPage;
			Meta.Tags = new List<BlogPostTag>();
			foreach(var blogPost in Blogs)
			{
				foreach(var tag in blogPost.Tags)
				{
					var existingTag = Meta.Tags.SingleOrDefault(t => t.Tag.ToLower() == tag.ToLower());
					if(existingTag == null)
					{
						Meta.Tags.Add(new BlogPostTag() {Tag=tag, Tag_URLFriendly=tag.MakeURLFriendly(), Count =1});
					}
					else
					{
						existingTag.Count ++;
					}
				}
			}
			return this;
		}

		public List<BlogPost>GetPage(int pageNumber)
		{
			var pages = Blogs.Where(b => b.Published).Skip(BlogsPerPage * (pageNumber-1)).Take(BlogsPerPage);
			if(pages != null) {
				return pages.ToList();
			}
			return null;
		}

		public List<BlogPost>GetBlogPostsWithTag(string tag)
		{
			var trueTag = Meta.Tags.SingleOrDefault(t => t.Tag_URLFriendly == tag.ToLower());
			if(trueTag != null)
			{
				var pages = Blogs.Where(b => b.Published && b.Tags.Contains(trueTag.Tag));
				if(pages != null) {
					return pages.ToList();
				}
			}
			return null;
		}
	}
} 
