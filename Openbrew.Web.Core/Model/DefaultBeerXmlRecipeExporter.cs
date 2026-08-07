using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Openbrew.Web.Core.Configuration;
using Openbrew.Web.Core.Service;
using ctorx.Core.Formatting;

namespace Openbrew.Web.Core.Model
{
	/// <summary>
	/// Exports a recipe to the Beer Xml Format
	/// </summary>
	public class DefaultBeerXmlRecipeExporter : IBeerXmlRecipeExporter
	{
		readonly IWebSettings WebSettings;
		readonly IRecipeUnitConverter RecipeUnitConverter;
		readonly IBeerStyleService BeerStyleService;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public DefaultBeerXmlRecipeExporter(IWebSettings webSettings, IRecipeUnitConverter recipeUnitConverter, IBeerStyleService beerStyleService)
		{
			this.WebSettings = webSettings;
			this.RecipeUnitConverter = recipeUnitConverter;
			this.BeerStyleService = beerStyleService;
		}

		/// <summary>
		/// Exports a recipe to the Beer Xml Format
		/// </summary>
		public string Export(Recipe recipe)
		{
			if (recipe == null)
			{
				throw new ArgumentNullException("recipe");
			}

			// Build the Recipe Node
			var recipeXml =
				new XElement("RECIPE",
				new XElement("WATER"),
				new XElement("EQUIPMENT"),
				new XElement("NAME", recipe.RecipeName + " (exported from brewgr.com)"),
				new XElement("VERSION", "1"),
				new XElement("TYPE", HumanReadableFormatter.AddSpacesToPascalCaseString(((RecipeType)recipe.RecipeTypeId).ToString())),
				new XElement("BREWER", recipe.User.CalculatedUsername + string.Format(" ({0}/!/{1})", this.WebSettings.RootPath, recipe.User.CalculatedUsername)),
				new XElement("BATCH_SIZE", recipe.GetUnitType() == UnitType.Metric ? recipe.BatchSize : this.RecipeUnitConverter.ConvertGallonsToLiters(recipe.BatchSize)),
				new XElement("BOIL_SIZE", recipe.GetUnitType() == UnitType.Metric ? recipe.BoilSize : this.RecipeUnitConverter.ConvertGallonsToLiters(recipe.BoilSize)),
				new XElement("BOIL_TIME", recipe.BoilTime),
				new XElement("EFFICIENCY", recipe.Efficiency * 100),
				new XElement("NOTES", this.GetNotes(recipe)),
				new XElement("IBU_METHOD", this.GetIbuMethod(recipe)));

			// Hops
			recipeXml.Add(new XElement("HOPS",
				recipe.Hops.Select(x => new XElement("HOP",
					new XElement("NAME", x.Hop.Name),
					new XElement("VERSION", "1"),
					new XElement("ALPHA", x.AlphaAcidAmount),
					new XElement("AMOUNT", recipe.GetUnitType() == UnitType.Metric ? RecipeUnitConverter.ConvertGramsToKilograms(x.Amount) : RecipeUnitConverter.ConvertOuncesToKilograms(x.Amount)),
					new XElement("USE", this.GetHopUsageText(x.HopUsageTypeId)),
					new XElement("TIME", x.TimeInMinutes)
			))));

			// Fermentables
			recipeXml.Add(new XElement("FERMENTABLES",
				recipe.Fermentables.Select(x => new XElement("FERMENTABLE",
					new XElement("NAME", x.Fermentable.Name),
					new XElement("VERSION", "1"),
					new XElement("AMOUNT", recipe.GetUnitType() == UnitType.Metric ? x.Amount : RecipeUnitConverter.ConvertPoundsToKilograms(x.Amount)),
					new XElement("TYPE", this.GetFermentableUsageText(x.FermentableUsageTypeId, x.Fermentable.Name)),
					new XElement("YIELD", (x.Ppg / 46.214 / 0.01)),
					new XElement("COLOR", x.Lovibond)
			))));

			// Yeast
			recipeXml.Add(new XElement("YEASTS",
				recipe.Yeasts.Select(x => new XElement("YEAST",
					new XElement("NAME", x.Yeast.Name),
					new XElement("VERSION", "1"),
					new XElement("TYPE", x.Yeast.Name.ToLower().IndexOf("lager") > -1 ? "Lager" : "Ale"),
					new XElement("FORM", this.GetYeastFormFromName(x.Yeast.Name)),
					new XElement("ATTENUATION", x.Attenuation * 100)
			))));

			// Miscs
			recipeXml.Add(new XElement("MISCS",
				recipe.Adjuncts.Select(x => new XElement("MISC",
					new XElement("NAME", x.Adjunct.Name),
					new XElement("VERSION", "1"),
					this.GetAdjuctAmountElements(x),
					new XElement("TYPE", "other"),
					this.GetAdjunctUsageElement(x)
			))));

			// Style
			if (!string.IsNullOrWhiteSpace(recipe.BjcpStyleSubCategoryId))
			{
				var style = this.BeerStyleService.GetStyleBySubCategoryId(recipe.BjcpStyleSubCategoryId);

				recipeXml.Add(new XElement("STYLE",
					new XElement("NAME", style.SubCategoryName),
					new XElement("VERSION", "1"),
					new XElement("CATEGORY_NUMBER", style.CategoryId),
					new XElement("STYLE_LETTER", style.SubCategoryId.Replace(style.CategoryId.ToString(), "")),
					new XElement("STYLE_GUIDE", "BJCP"),
					new XElement("TYPE", this.GetStyleTypeText(style)),
					new XElement("OG_MIN", style.Og_Low),
					new XElement("OG_MAX", style.Og_High),
					new XElement("FG_MIN", style.Fg_Low),
					new XElement("FG_MAX", style.Fg_High),
					new XElement("IBU_MIN", style.Ibu_Low),
					new XElement("IBU_MAX", style.Ibu_High),
					new XElement("COLOR_MIN", style.Srm_Low),
					new XElement("COLOR_MAX", style.Srm_High)
				));
			}
			else
			{
				recipeXml.Add(new XElement("STYLE"));
			}

			// Build the Doc
			var xdoc = new XDocument(
				new XDeclaration("1.0", null, null),
					new XElement("RECIPES", recipeXml)
				);

			return xdoc.ToString();
		}

		/// <summary>
		/// Gets the Ibu Method Name
		/// </summary>
		string GetIbuMethod(Recipe recipe)
		{
			return recipe.IbuFormulaId == (int)IbuFormula.Tinseth
				? "Tinseth" : recipe.IbuFormulaId == (int)IbuFormula.Rager ? "Rager" : "Brewgr";
		}

		/// <summary>
		/// Gets the default notes
		/// </summary>
		string GetNotes(Recipe recipe)
		{
			var notes = new StringBuilder();

			notes.AppendLine("This recipe was exported from http://brewgr.com.  ");
			
			if (!string.IsNullOrWhiteSpace(recipe.Description))
			{
				notes.AppendLine();
				notes.AppendLine(recipe.Description);
			}

			return notes.ToString();
		}

		/// <summary>
		/// Gets an adjunct usage element
		/// </summary>
		XElement GetAdjunctUsageElement(RecipeAdjunct recipeAdjunct)
		{
			switch((AdjunctUsageType)recipeAdjunct.AdjunctUsageTypeId)
			{
				case AdjunctUsageType.Boil:
					return new XElement("use", "boil");
				case AdjunctUsageType.Mash:
					return new XElement("use", "mash");
				case AdjunctUsageType.Primary:
					return new XElement("use", "primary");
				case AdjunctUsageType.Secondary:
					return new XElement("use", "secondary");
				case AdjunctUsageType.Bottle:
					return new XElement("use", "bottling");
				default:
					return new XElement("use", "boil");
			}
		}

		/// <summary>
		/// Gets the style type name
		/// </summary>
		string GetStyleTypeText(BjcpStyle style)
		{
			// May be “Lager”, “Ale”, “Mead”, “Wheat”, “Mixed” or “Cider”.  Defines the type of beverage associated with this category.

			if (style.CategoryName.ToLower().IndexOf("lager") > -1 || style.CategoryName.ToLower().IndexOf("pilsner") > -1)
			{
				return "Lager";
			}

			if (style.CategoryName.ToLower().IndexOf("wheat") > -1)
			{
				return "Wheat";
			}

			if (style.CategoryName.ToLower().IndexOf("cider") > -1)
			{
				return "Cider";
			}

			if (style.CategoryName.ToLower().IndexOf("hybrid") > -1)
			{
				return "Mixed";
			}

			return "Ale";
		}

		/// <summary>
		/// Gets the Adjunct Amount Elements
		/// </summary>
		object GetAdjuctAmountElements(RecipeAdjunct recipeAdjunct)
		{
			// Weight use KG, Liquid use L

			switch (recipeAdjunct.Unit.ToLower())
			{				
				case "each":
					return new[] { new XElement("AMOUNT", recipeAdjunct.Amount), new XElement("AMOUNT_IS_WEIGHT", "false") };
				case "lb":
					return new[] { new XElement("AMOUNT", RecipeUnitConverter.ConvertPoundsToKilograms(recipeAdjunct.Amount)), new XElement("AMOUNT_IS_WEIGHT", "true") };
				case "oz":
					return new[] { new XElement("AMOUNT", RecipeUnitConverter.ConvertOuncesToKilograms(recipeAdjunct.Amount)), new XElement("AMOUNT_IS_WEIGHT", "true") };
				case "floz":
					return new[] { new XElement("AMOUNT", RecipeUnitConverter.ConvertFluidOuncesToLiters(recipeAdjunct.Amount)), new XElement("AMOUNT_IS_WEIGHT", "false") };
				case "gal":
					return new[] { new XElement("AMOUNT", RecipeUnitConverter.ConvertGallonsToLiters(recipeAdjunct.Amount)), new XElement("AMOUNT_IS_WEIGHT", "false") };
				case "pt":
					return new[] { new XElement("AMOUNT", RecipeUnitConverter.ConvertPintsToLiters(recipeAdjunct.Amount)), new XElement("AMOUNT_IS_WEIGHT", "false") };
				case "qt":
					return new[] { new XElement("AMOUNT", RecipeUnitConverter.ConvertQuartsToLiters(recipeAdjunct.Amount)), new XElement("AMOUNT_IS_WEIGHT", "false") };
				case "tbsp":
					return new[] { new XElement("AMOUNT", RecipeUnitConverter.ConvertTbspToLiters(recipeAdjunct.Amount)), new XElement("AMOUNT_IS_WEIGHT", "false") };
				case "tsp":
					return new[] { new XElement("AMOUNT", RecipeUnitConverter.ConvertTspToLiters(recipeAdjunct.Amount)), new XElement("AMOUNT_IS_WEIGHT", "false") };
				case "kg":
					return new[] { new XElement("AMOUNT", recipeAdjunct.Amount), new XElement("AMOUNT_IS_WEIGHT", "true") };
				case "l":
					return new[] { new XElement("AMOUNT", recipeAdjunct.Amount), new XElement("AMOUNT_IS_WEIGHT", "false") };
				case "ml":
					return new[] { new XElement("AMOUNT", recipeAdjunct.Amount * 1000), new XElement("AMOUNT_IS_WEIGHT", "false") };
				default:
					return null;
			}
		}

		/// <summary>
		/// Gets the Yeast Form from the name
		/// </summary>
		string GetYeastFormFromName(string name)
		{
			if (name.ToLower().IndexOf("brewferm") > -1 ||
				name.ToLower().IndexOf("coopers") > -1 ||
				name.ToLower().IndexOf("danstar") > -1 ||
				name.ToLower().IndexOf("fermentis") > -1 ||
				name.ToLower().IndexOf("muntons") > -1)
			{
				return "Dry";
			}

			return "Liquid";
		}

		/// <summary>
		/// Gets the Fermentable Usage Text
		/// </summary>
		string GetFermentableUsageText(int fermentableUsageTypeId, string ingredientName)
		{
			switch ((FermentableUsageType)fermentableUsageTypeId)
			{
				case FermentableUsageType.Mash:
				case FermentableUsageType.Steep:
					return "Grain";
				case FermentableUsageType.Extract:
					return ingredientName.ToLower().IndexOf("dry ") > -1 ? "Dry Extract" : "Extract";
				case FermentableUsageType.Late:
					return "Sugar";
				default:
					return "Grain";
			}
		}

		/// <summary>
		/// Gets the Hop Usage Text
		/// </summary>
		string GetHopUsageText(int hopUsageTypeId)
		{
			switch ((HopUsageType)hopUsageTypeId)
			{
				case HopUsageType.Mash:
					return "Mash";
				case HopUsageType.FirstWort:
					return "First Wort";
				case HopUsageType.Boil:
					return "Boil";			
				case HopUsageType.FlameOut:
					return "Flame Out";
				case HopUsageType.DryHop:
					return "Dry Hop";
				default:
					return "Boil";
			}
		}
	}
}