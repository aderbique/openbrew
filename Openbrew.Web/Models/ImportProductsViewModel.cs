using System;
using System.Linq;
using System.Web;

namespace Openbrew.Web.Models
{
	public class ImportProductsViewModel
	{
		/// <summary>
		/// Gets or sets the ProductFeedFile
		/// </summary>
		public HttpPostedFileBase ProductFeedFile { get; set; }
	}
}