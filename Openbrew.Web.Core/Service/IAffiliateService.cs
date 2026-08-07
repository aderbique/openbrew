using System.IO;
using Openbrew.Web.Core.Model;

namespace Openbrew.Web.Core.Service
{
	public interface IAffiliateService
	{
		/// <summary>
		/// Imports products for an affiliate
		/// </summary>
		void ImportProducts(int affiliateId, Stream stream);

		/// <summary>
		/// Gets the best match product for a fermentable
		/// </summary>
		AffiliateProduct GetBestMatchProduct<TIngredientType>(int ingredientId) where TIngredientType : class, IIngredient;
	}
}