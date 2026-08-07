using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Openbrew.Web.Core.Data;
using Openbrew.Web.Core.Model;
using ctorx.Core.Collections;

namespace Openbrew.Web.Core.Service
{
	public class DefaultAffiliateService : IAffiliateService
	{
		readonly IBrewgrRepository Repository;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public DefaultAffiliateService(IBrewgrRepository repository)
		{
			this.Repository = repository;
		}

		/// <summary>
		/// Imports products for an affiliate
		/// </summary>
		public void ImportProducts(int affiliateId, Stream stream)
		{
			// NOTE: This is hardcoded for Midwest for now
			// NOTE: If we get more affiliartes (and we should)
			// NOTE: This should use a factory and parser classes/methods

			// Func to Get Node Value
			Func<XElement, string, string> getValueFunc = ((XElement node, string name) =>
			{
				var match = node.Elements().FirstOrDefault(x => x.Name.LocalName == name);
				if (match == null)
				{
					return null;
				}

				return match.Value;
			});

			// Get Existing Products
			var products = this.Repository.GetSet<AffiliateProduct>();

			// Get Xml from Stream and Parse as XML
			var xmlContent = new StreamReader(stream).ReadToEnd();
			var xDoc = XDocument.Parse(xmlContent);

			var productNodes = xDoc.Elements().FirstOrDefault(x => x.Name.LocalName == "catalog")
				.Elements().Where(x => x.Name.LocalName == "product");

			// Add and Update based on feed content
			foreach (var productNode in productNodes)
			{
				var sku = getValueFunc(productNode, "sku");
				var dateModified = DateTime.Parse(getValueFunc(productNode, "lastupdated"));

				var product = new AffiliateProduct();

				// Check Existing Products
				product = products.FirstOrDefault(x => x.Sku == sku);

				if (product != null && dateModified <= (product.DateModified ?? DateTime.MinValue))
				{
					continue;
				}

				// Doesn't Exist, new it up (otherwise set modify date)
				if (product == null)
				{
					product = new AffiliateProduct();
					product.DateCreated = DateTime.Now;
					product.AffiliateId = affiliateId;

					this.Repository.Add(product);
				}
				else
				{
					product.DateModified = DateTime.Now;
				}

				// Set Values
				product.DateModified = dateModified;
				product.Name = getValueFunc(productNode, "name");

				// Only set Description if it is a new product
				if (product.AffiliateProductId <= 0)
				{
					product.Description = getValueFunc(productNode, "description");
				}

				product.Sku = getValueFunc(productNode, "sku");
				product.Price = Decimal.Parse(getValueFunc(productNode, "price"));
				product.Url = getValueFunc(productNode, "buyurl");
				product.ImageUrl = getValueFunc(productNode, "imageurl");
				product.Category = getValueFunc(productNode, "advertisercategory");
				product.InStock = getValueFunc(productNode, "instock") == "YES";
			}

			// Deactivate Anything Not in the Feed Anymore
			foreach (var product in products)
			{
				if (productNodes.Select(x => x.Elements().FirstOrDefault(y => y.Name.LocalName == "sku").Value)
					.FirstOrDefault(x => x == product.Sku) == null)
				{
					product.IsActive = false;
					product.DateModified = DateTime.Now;
				}
			}
		}

		/// <summary>
		/// Gets the best match product for a fermentable
		/// </summary>
		public AffiliateProduct GetBestMatchProduct<TIngredientType>(int ingredientId) where TIngredientType : class, IIngredient
		{
			IQueryable<IngredientAffiliateProduct<IIngredient>> productSet;

			// We have to do this because EF won't figure it out for us
			switch (typeof(TIngredientType).Name)
			{
				case "Fermentable":
					productSet = this.Repository.GetSet<FermentableAffiliateProduct>() as IQueryable<IngredientAffiliateProduct<IIngredient>>;
					break;
				case "Hop":
					productSet = this.Repository.GetSet<HopAffiliateProduct>() as IQueryable<IngredientAffiliateProduct<IIngredient>>;
					break;
				case "Yeast":
					productSet = this.Repository.GetSet<YeastAffiliateProduct>() as IQueryable<IngredientAffiliateProduct<IIngredient>>;
					break;
				case "Adjunct":
					productSet = this.Repository.GetSet<AdjunctAffiliateProduct>() as IQueryable<IngredientAffiliateProduct<IIngredient>>;
					break;
				default:
					throw new InvalidOperationException("A valid type was not specified");
			}

			return productSet
				.Where(x => x.IngredientId == ingredientId)
				.Where(x => x.IsActive)
				.OrderBy(x => x.Rank)
				.Select(x => x.AffiliateProduct)
				.Take(1)
				.FirstOrDefault();


			//return this.Repository.GetSet<TIngredientAffiliateProductType>()
			//	.Where(x => x.IngredientId == ingredientId)
			//	.Where(x => x.IsActive)
			//	.OrderBy(x => x.Rank)
			//	.Select(x => x.AffiliateProduct)
			//	.Take(1)
			//	.FirstOrDefault();
		}
	}
}