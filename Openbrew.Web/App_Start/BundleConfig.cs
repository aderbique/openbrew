using System.Web.Optimization;

namespace Openbrew.Web
{
	public class BundleConfig
	{
		// For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles(BundleCollection bundles)
		{
			// Ensure browser clients receive a versioned recipe-builder bundle after
			// deployment rather than retaining an old individual JavaScript file.
			BundleTable.EnableOptimizations = true;

			bundles.Add(new ScriptBundle("~/bundles/js-v3")
				.Include(
					"~/js/jquery.tmpl.min.js",
					"~/js/jquery.colorbox.js",
					"~/js/superfish.js",
					"~/js/jquery.tipsy.js",
					"~/js/jquery.validate.js",
					"~/js/jquery.validate.unobtrusive.js",
					"~/js/plugins.js",
					"~/js/jquery.custom.js",
					"~/js/jquery.raty.js",
					"~/js/jquery.filter_input.js",
					"~/js/jquery.timeago.js",
					"~/js/t.js",
					"~/js/jquery.chosen.js",
					"~/js/jquery.autosize.js",
					"~/js/utility.js",
					"~/js/layout.js",
					"~/js/recipe-workbench.js",
					"~/js/session.js",
					"~/js/style-chart.js"));

			bundles.Add(new StyleBundle("~/bundles/css")
				.Include(
					"~/css/smoothness/jquery-ui-1.10.3.custom.css",
					"~/css/style.css",
					"~/css/custom.css",
					"~/css/builder.css",
					"~/css/colorbox.css"));
		} 
	}
}
