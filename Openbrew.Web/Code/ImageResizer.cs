using System;
using System.IO;
using ImageResizer;

namespace Openbrew.Web
{
	public static class ImageResizer
	{
		/// <summary>
		/// Resizes an image for use as a Recipe Detail Image
		/// </summary>
		public static byte[] ResizeForRecipeDetailImage(Stream stream)
		{
			if(stream == null)
			{
				throw new ArgumentNullException("httpPostedFileBase");
			}

			// Image Upload
			return Resize(stream, "format=jpg&quality=80&width=300&scale=both&height=300&crop=auto");
		}

		/// <summary>
		/// Resizes an image for use as a Recipe thumbnail Image
		/// </summary>
		public static byte[] ResizeForRecipeThumbnailImage(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("httpPostedFileBase");
			}

			// Image Upload
			return Resize(stream, "format=jpg&quality=80&width=80&scale=both&height=80&crop=auto");
		}

		/// <summary>
		/// Resizes an image using the provided settings
		/// </summary>
		static byte[] Resize(Stream stream, string settings)
		{
			var memoryStream = new MemoryStream();
			ImageBuilder.Current.Build(stream, memoryStream, new ResizeSettings(settings));
			return memoryStream.ToArray();
		}
	}
}