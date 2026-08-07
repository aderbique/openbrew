using System;
using System.Web;
using System.Web.Mvc;
using Openbrew.Web.Core.Service;
using ctorx.Core.Ninject;
using Ninject;

namespace Openbrew.Web
{
	public static class HtmlHelperExtensions
	{
		/// <summary>
		/// Used to help with selectd values on drop down boxes
		/// </summary>
		public static string IsSelected(this HtmlHelper htmlHelper, string value, string comparison)
		{
			if(value.Trim() == comparison.Trim())
			{
				return "selected=\"selected\"";
			}

			return "";
		}

		/// <summary>
		/// Used to help with selectd values on drop down boxes
		/// </summary>
		public static string IsSelected(this HtmlHelper htmlHelper, int value, int comparison)
		{
			if (value == comparison)
			{
				return "selected=\"selected\"";
			}

			return "";
		}

		/// <summary>
		/// Used to help with selectd values on drop down boxes
		/// </summary>
		public static string IsSelected(this HtmlHelper htmlHelper, double value, double comparison)
		{
			if (value.Equals(comparison))
			{
				return "selected=\"selected\"";
			}

			return "";
		}


		/// <summary>
		/// Used to help with selectd values on drop down boxes
		/// </summary>
		public static string IsSelected(this HtmlHelper htmlHelper, bool value, bool comparison)
		{
			if (value.Equals(comparison))
			{
				return "selected=\"selected\"";
			}

			return "";
		}

		/// <summary>
		/// Gets db content by content Id
		/// </summary>
		public static IHtmlString DbContent(this HtmlHelper htmlHelper, int contentId)
		{
			if (contentId <= 0)
			{
				throw new ArgumentOutOfRangeException("contentId");
			}

			var contentService = GetContentService();
			return htmlHelper.Raw(contentService.GetContentTextById(contentId, cacheResult: true));
		}

		/// <summary>
		/// Gets db content by content short name
		/// </summary>
		public static IHtmlString DbContent(this HtmlHelper htmlHelper, string shortName)
		{
			if(string.IsNullOrWhiteSpace(shortName))
			{
				throw new ArgumentNullException("shortName");	
			}

			var contentService = GetContentService();
			return htmlHelper.Raw(contentService.GetContentTextByShortName(shortName, cacheResult: true));
		}

		/// <summary>
		/// Gets a Content Service
		/// </summary>
		static IContentService GetContentService()
		{
			return KernelPersister.Get().Get<IContentService>();
		}
	}
}