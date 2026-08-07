using System;
using ctorx.Core.Collections;

namespace Openbrew.Web.Models
{
	public class PageableViewModel
	{
		/// <summary>
		/// Gets or sets the Pager
		/// </summary>
		public Pager Pager { get; set; }

		/// <summary>
		/// Gets or sets the BaseUrl
		/// </summary>
		public string BaseUrl { get; set; }

		/// <summary>
		/// Gets the previous page Url
		/// </summary>
		public string GetPreviousPageUrl()
		{
			if(this.Pager.CurrentPage - 1 == 1)
			{
				return this.BaseUrl;
			}

			return this.GetPageUrl(this.Pager.CurrentPage - 1);
		}

		/// <summary>
		/// Gets the next page Url
		/// </summary>
		public string GetNextPageUrl()
		{
			return this.GetPageUrl(this.Pager.CurrentPage + 1);			
		}

		/// <summary>
		/// Gets a Page Url
		/// </summary>
		public string GetPageUrl(int page)
		{
			return string.Format("{0}/{1}", BaseUrl, page);
		}

	}
}