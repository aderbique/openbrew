using System;

using System.IO;

namespace Openbrew.Web.Core.Service
{
	public interface IStaticContentService
	{
		/// <summary>
		/// Writes an image to disk, returning the root Url
		/// </summary>
		string SaveRecipeImage(Stream fileStream, string physicalRoot);

		/// <summary>
		/// Deletes a Recipe Image
		/// </summary>
		void DeleteRecipeImage(string mediaPhysicalRoot, string oldImageUrlRoot);
	}
}