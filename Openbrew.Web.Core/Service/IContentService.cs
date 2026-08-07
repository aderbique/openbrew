using Openbrew.Web.Core.Model;

namespace Openbrew.Web.Core.Service
{
	public interface IContentService
	{
		/// <summary>
		/// Gets a content entry by Id
		/// </summary>
		Content GetContentById(int contentId, bool forceActive = false, bool forcePublic = false, bool cacheResult = false);

		/// <summary>
		/// Gets a Content entry by short name
		/// </summary>
		Content GetContentByShortName(string shortName, bool forceActive = false, bool forcePublic = false, bool cacheResult = false);

		/// <summary>
		/// Gets content text by id
		/// </summary>
		string GetContentTextById(int contentId, bool forceActive = false, bool forcePublic = false, bool cacheResult = false);

		/// <summary>
		/// Gets content text by short name
		/// </summary>
		string GetContentTextByShortName(string shortName, bool forceActive = false, bool forcePublic = false, bool cacheResult = false);
	}
}