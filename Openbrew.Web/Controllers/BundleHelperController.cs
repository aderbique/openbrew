using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Openbrew.Web.Controllers
{
	public class BundleHelperController : BrewgrController
	{
		/// <summary>
		/// Gets the rendered script bunle
		/// </summary>
		public ActionResult GetRenderedScriptBundle()
		{
			return View();
		}

		/// <summary>
		/// Gets the rendered style bundle
		/// </summary>
		public ActionResult GetRenderedStyleBundle()
		{
			return View();
		}
	}
}