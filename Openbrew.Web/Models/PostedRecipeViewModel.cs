using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;

namespace Openbrew.Web.Models
{
	public class PostedRecipeViewModel
	{
		/// <summary>
		/// Gets or sets the RecipeJson
		/// </summary>
		public string RecipeJson { get; set; }

		/// <summary>
		/// Gets or sets the RecipePhoto
		/// </summary>
		public HttpPostedFileBase PhotoForUpload { get; set; }

		/// <summary>
		/// Hydrates the Recipe Json
		/// </summary>
		public RecipeViewModel HydrateRecipeJson()
		{
			if(string.IsNullOrWhiteSpace(this.RecipeJson))
			{
				return null;
			}

			var serializer = new JavaScriptSerializer();
			return serializer.Deserialize<RecipeViewModel>(HttpUtility.UrlDecode(this.RecipeJson));
		}
	}
}