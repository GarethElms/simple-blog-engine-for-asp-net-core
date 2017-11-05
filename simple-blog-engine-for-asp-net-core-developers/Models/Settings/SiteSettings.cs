using SimpleBlogEngine.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBlogEngine.Models
{
	public class SiteSettings
	{
		public string Owner {get;set;}
		public string SiteName {get;set;}
		public string SiteURL {get;set;}
		public string Description {get;set;}
		public bool ShowViewHooks {get;set;}
		public Theme Theme {get;set;}
		public SiteSettings_Meta MetaData {get;set;}
		public DebugMode DebugMode {get;set;}
	}

	public class Theme
	{
		public string ThemeName {get;set;}
		public string HeaderImage {get;set;}
		public string ShareImage {get;set;}
	}

	public class SiteSettings_Meta
	{
		public string Locale {get;set;}
		public string TwitterUserName {get;set;}
		public string PageTitlePrefix {get;set;}
		public string PageTitleSuffix {get;set;}
	}

	public class DebugMode
	{
		public bool ShowViewHooks {get;set;}
	}
} 
