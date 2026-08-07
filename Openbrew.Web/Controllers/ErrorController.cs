using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Openbrew.Web.Controllers
{
	public class ErrorController : BrewgrController
	{
		[ActionName("404")]
		public ViewResult NotFound()
		{
			return View();
		}

		[ActionName("500")]
		public ViewResult Error()
		{
			return View();
		}

		/// <summary>
		/// Executes the View for ErrorTest
		/// </summary>
		[ActionName("500-test")]
		public ViewResult ErrorTest()
		{
			throw new Exception("Error-Test");
		}

		/// <summary>
		/// Executes the View for NotFoundTest
		/// </summary>
		[ActionName("404-test")]
		public ActionResult NotFoundTest()
		{
			return this.Issue404();
		}

		[ActionName("403-test")]
		public ViewResult ForbiddenText()
		{
			throw new HttpException(403, "Forbidden Test");
		}
	}
}