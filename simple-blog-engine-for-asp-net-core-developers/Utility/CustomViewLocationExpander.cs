using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBlogEngine
{
	public class CustomViewLocationExpander : IViewLocationExpander
	{
		public void PopulateValues(ViewLocationExpanderContext context)
		{
		}

		public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
		{
			// {1} is the controller and {0} is the name of the View
			var newViewLocations = new List<String>(viewLocations);
			newViewLocations.Insert(0, "/Views/Shared/EmptyCustomViews/{0}.cshtml"); // First. for a moment.
			newViewLocations.Insert(0, "/Views/Custom/{0}.cshtml"); // Not any more, this is first now.
			return newViewLocations;
		}
	}
}