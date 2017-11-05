using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleBlogEngine.Models;
using Microsoft.Extensions.Options;
using SimpleBlogEngine.Utility;
using System.IO;
using NLog.Extensions.Logging;
using NLog.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Razor;

namespace SimpleBlogEngine
{
	public class Startup
	{
		public IConfiguration Configuration_from_BuildWebHost { get; }
		public IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration,IHostingEnvironment hostingEnvironment)
		{
			Configuration_from_BuildWebHost = configuration;
			hostingEnvironment.ConfigureNLog("nlog.config");
			var builder = new ConfigurationBuilder()
				.SetBasePath(hostingEnvironment.ContentRootPath)
				.AddJsonFile(GetPathToSettingsFile("blogPosts.json"), optional:true, reloadOnChange:true)
				.AddJsonFile(GetPathToSettingsFile("emailSettings.json"), optional:true, reloadOnChange:true)
				.AddJsonFile(GetPathToSettingsFile("googleRecaptchaSettings.json"), optional:true, reloadOnChange:true)
				.AddJsonFile(GetPathToSettingsFile("googleAnalyticsSettings.json"), optional:true, reloadOnChange:true)
				.AddJsonFile(GetPathToSettingsFile("addThisSettings.json"), optional:true, reloadOnChange:true)
				.AddJsonFile(GetPathToSettingsFile("siteSettings.json"), optional:true, reloadOnChange:true)
				.AddJsonFile(GetPathToSettingsFile("disqusSettings.json"), optional:true, reloadOnChange:true);
			Configuration = builder.Build();
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc();
			services.AddOptions();
			services.Configure<BlogPostsSettings>(Configuration);
			services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
			services.Configure<GoogleRecaptchaSettings>(Configuration.GetSection("GoogleRecaptchaSettings"));
			services.Configure<GoogleAnalyticsSettings>(Configuration.GetSection("GoogleAnalyticsSettings"));
			services.Configure<AddThisSettings>(Configuration.GetSection("AddThisSettings"));
			services.Configure<SiteSettings>(Configuration.GetSection("SiteSettings"));
			services.Configure<DisqusSettings>(Configuration.GetSection("DisqusSettings"));

			services.AddTransient<IEmailSender,Email>();
			services.AddTransient<IGoogleRecaptcha,GoogleRecaptcha>();

			services.Configure<RazorViewEngineOptions>(o => { o.ViewLocationExpanders.Add(new CustomViewLocationExpander()); });

			//call this in case you need aspnet-user-authtype/aspnet-user-identity
			services.AddSingleton<IHttpContextAccessor,HttpContextAccessor>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(
			IApplicationBuilder app,
			IHostingEnvironment env,
			IOptionsMonitor<BlogPostsSettings> blogPostConfigMonitor,
			IOptionsMonitor<EmailSettings> emailConfigMonitor,
			IOptionsMonitor<GoogleRecaptchaSettings> googleRecaptchaConfigMonitor,
			IOptionsMonitor<GoogleAnalyticsSettings> googleAnalyticsConfigMonitor,
			IOptionsMonitor<AddThisSettings> addThisConfigMonitor,
			IOptionsMonitor<SiteSettings> siteConfigMonitor,
			IOptionsMonitor<DisqusSettings> disqusConfigMonitor,
			ILoggerFactory loggerFactory)
		{
			if(env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseBrowserLink();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}
			app.UseStaticFiles();

			blogPostConfigMonitor.CurrentValue.CalculateMetaData();
			blogPostConfigMonitor.OnChange(
				values =>
				{
					values.CalculateMetaData();
				});
			siteConfigMonitor.OnChange(values =>
			{
				values.Theme.HeaderImage = values.Theme.HeaderImage.Replace("/{theme}","").Replace("{theme}",""); // {theme} is not needed, it's just a hint. Removing {theme} here is more optimal than replacing it every request.
			});
			emailConfigMonitor.OnChange(values => { });
			googleRecaptchaConfigMonitor.OnChange(values => { });
			googleAnalyticsConfigMonitor.OnChange(values => { });
			addThisConfigMonitor.OnChange(values => { });
			disqusConfigMonitor.OnChange(values => { });

			siteConfigMonitor.CurrentValue.Theme.HeaderImage = siteConfigMonitor.CurrentValue.Theme.HeaderImage.Replace("/{theme}","").Replace("{theme}","");

			app.UseMvc(routes =>
			{
				routes.MapRoute(
						 name: "page",
						 template: "page/{pageNumber:int}",
						 defaults: new { controller = "Home",action = "Index" });

				routes.MapRoute(
					  name: "blog",
					  template: "{year:int}/{month:int}/{slug}",
					  defaults: new { controller = "Blog",action = "ViewBlogPost" });

				routes.MapRoute(
					  name: "blog2",
					  template: "blog/{year:int}/{month:int}/{slug}",
					  defaults: new { controller = "Blog",action = "ViewBlogPost" });

				routes.MapRoute(
						 name: "category",
						 template: "category/{category}",
						 defaults: new { controller = "Category",action = "Index" });

				routes.MapRoute(
						 name: "default",
						 template: "{controller=Home}/{action=Index}/{id?}");
			});

			loggerFactory.AddNLog();
			app.AddNLogWeb();
		}

		private string GetPathToSettingsFile(string settingsFileName)
		{
			var customFile = "Settings" + Path.DirectorySeparatorChar + "Custom" + Path.DirectorySeparatorChar + settingsFileName;
			if(File.Exists(customFile))
			{
				return customFile;
			}
			return "Settings" + Path.DirectorySeparatorChar + "System" + Path.DirectorySeparatorChar + settingsFileName;
		}
	}
}
