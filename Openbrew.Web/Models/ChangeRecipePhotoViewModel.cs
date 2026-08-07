using System.Web;

namespace Openbrew.Web.Models
{
	public class ChangeRecipePhotoViewModel
	{
		/// <summary>
		/// Gets or sets the RecipeId
		/// </summary>
		public int RecipeId { get; set; }

		/// <summary>
		/// Gets or sets the HttpPostedFileBase
		/// </summary>
		public HttpPostedFileBase PhotoForUpload { get; set; }
	}
}