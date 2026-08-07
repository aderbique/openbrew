using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using Openbrew.Web.Core.Service;

namespace Openbrew.Web.Core.Model
{
	public class DefaultBeerXmlRecipeImporter : IBeerXmlRecipeImporter
	{
		readonly IRecipeUnitConverter RecipeUnitConverter;
		readonly IRecipeDataService BrewDataService;
		readonly IBeerStyleService BeerStyleService;
		readonly int? UserId;

		public DefaultBeerXmlRecipeImporter(IRecipeUnitConverter recipeUnitConverter, IRecipeDataService brewDataService, IBeerStyleService beerStyleService,
			IUserResolver userResolver)
		{
			RecipeUnitConverter = recipeUnitConverter;
			BrewDataService = brewDataService;
			BeerStyleService = beerStyleService;
			var user = userResolver.Resolve();
			if (user != null) UserId = user.UserId;
		}

		public Recipe Import(string beerXml)
		{
			return ImportMany(beerXml).FirstOrDefault();
		}

		public IList<Recipe> ImportMany(string beerXml)
		{
			if (string.IsNullOrWhiteSpace(beerXml)) throw new ArgumentNullException("beerXml");

			var readerSettings = new XmlReaderSettings
			{
				DtdProcessing = DtdProcessing.Prohibit,
				XmlResolver = null,
				MaxCharactersInDocument = 5 * 1024 * 1024
			};
			XDocument document;
			using (var stringReader = new StringReader(beerXml))
			using (var xmlReader = XmlReader.Create(stringReader, readerSettings))
			{
				document = XDocument.Load(xmlReader, LoadOptions.None);
			}
			var entries = document.Descendants().Where(x => IsNamed(x, "RECIPE")).ToList();
			if (!entries.Any()) throw new FormatException("The file does not contain any BeerXML recipe records.");

			var hops = BrewDataService.GetUsableIngredients<Hop>(UserId).ToList();
			var fermentables = BrewDataService.GetUsableIngredients<Fermentable>(UserId).ToList();
			var yeasts = BrewDataService.GetUsableIngredients<Yeast>(UserId).ToList();
			var adjuncts = BrewDataService.GetUsableIngredients<Adjunct>(UserId).ToList();
			var recipes = new List<Recipe>();

			foreach (var entry in entries)
			{
				var recipe = NewRecipe();
				SetRecipeInfo(recipe, entry);
				SetHops(recipe, entry, hops);
				SetFermentables(recipe, entry, fermentables);
				SetYeasts(recipe, entry, yeasts);
				SetMiscs(recipe, entry, adjuncts);
				SetMashSteps(recipe, entry);
				recipes.Add(recipe);
			}

			return recipes;
		}

		Recipe NewRecipe()
		{
			return new Recipe
			{
				UnitTypeId = (int)UnitType.USStandard,
				IbuFormulaId = (int)IbuFormula.Tinseth,
				RecipeTypeId = (int)RecipeType.AllGrain,
				// Imported recipes follow the normal new-recipe default. The save
				// service will still keep incomplete recipes as unpublished drafts.
				IsPublic = true,
				BatchSize = 5,
				BoilSize = 6.5,
				BoilTime = 60,
				Efficiency = .75d
			};
		}

		void SetRecipeInfo(Recipe recipe, XElement entry)
		{
			recipe.RecipeName = Read(entry, "NAME").Replace("(exported from brewgr.com)", string.Empty).Trim();
			recipe.Description = Read(entry, "NOTES");
			recipe.RecipeTypeId = RecipeTypeFrom(Read(entry, "TYPE"));
			recipe.BatchSize = Gallons(ReadDouble(entry, "BATCH_SIZE"), recipe.BatchSize);
			recipe.BoilSize = Gallons(ReadDouble(entry, "BOIL_SIZE"), recipe.BoilSize);
			recipe.BoilTime = ReadInt(entry, "BOIL_TIME") ?? recipe.BoilTime;
			var efficiency = ReadDouble(entry, "EFFICIENCY");
			if (efficiency.HasValue) recipe.Efficiency = efficiency.Value / 100d;

			var ibuMethod = Read(entry, "IBU_METHOD");
			if (EqualsText(ibuMethod, "rager")) recipe.IbuFormulaId = (int)IbuFormula.Rager;
			else if (EqualsText(ibuMethod, "brewgr")) recipe.IbuFormulaId = (int)IbuFormula.Brewgr;

			var style = Child(entry, "STYLE");
			if (style != null)
			{
				var category = Read(style, "CATEGORY_NUMBER");
				var letter = Read(style, "STYLE_LETTER");
				if (!string.IsNullOrWhiteSpace(category) && !string.IsNullOrWhiteSpace(letter))
				{
					var match = BeerStyleService.GetStyleSummaries().FirstOrDefault(x => EqualsText(x.SubCategoryId, category + letter));
					if (match != null) { recipe.BjcpStyleSubCategoryId = match.SubCategoryId; recipe.BjcpStyleSummary = match; }
				}
			}
		}

		void SetHops(Recipe recipe, XElement entry, IList<Hop> known)
		{
			var container = Child(entry, "HOPS");
			if (container == null) return;
			var rank = 1;
			foreach (var element in Children(container, "HOP"))
			{
				var name = Read(element, "NAME");
				if (string.IsNullOrWhiteSpace(name)) continue;
				var match = known.FirstOrDefault(x => EqualsText(x.Name, name));
				var alpha = ReadDouble(element, "ALPHA") ?? 0;
				recipe.Hops.Add(new RecipeHop
				{
					Rank = rank++, IngredientId = match == null ? 0 : match.IngredientId,
					Hop = match ?? new Hop { Name = name, AA = alpha }, AlphaAcidAmount = alpha,
					Amount = Ounces(ReadDouble(element, "AMOUNT")), TimeInMinutes = ReadInt(element, "TIME") ?? 0,
					HopTypeId = HopTypeFrom(Read(element, "FORM")), HopUsageTypeId = HopUsageFrom(Read(element, "USE"))
				});
			}
		}

		void SetFermentables(Recipe recipe, XElement entry, IList<Fermentable> known)
		{
			var container = Child(entry, "FERMENTABLES");
			if (container == null) return;
			var rank = 1;
			foreach (var element in Children(container, "FERMENTABLE"))
			{
				var name = Read(element, "NAME");
				if (string.IsNullOrWhiteSpace(name)) continue;
				var match = known.FirstOrDefault(x => EqualsText(x.Name, name));
				var yield = ReadDouble(element, "YIELD") ?? 0;
				recipe.Fermentables.Add(new RecipeFermentable
				{
					Rank = rank++, IngredientId = match == null ? 0 : match.IngredientId,
					Fermentable = match ?? new Fermentable { Name = name }, Amount = Pounds(ReadDouble(element, "AMOUNT")),
					Ppg = Convert.ToInt32(Math.Round(yield * .46214d, MidpointRounding.AwayFromZero)),
					Lovibond = Convert.ToInt32(Math.Round(ReadDouble(element, "COLOR") ?? 0, MidpointRounding.AwayFromZero)),
					FermentableUsageTypeId = FermentableUsageFrom(Read(element, "TYPE"))
				});
			}
		}

		void SetYeasts(Recipe recipe, XElement entry, IList<Yeast> known)
		{
			var container = Child(entry, "YEASTS");
			if (container == null) return;
			var rank = 1;
			foreach (var element in Children(container, "YEAST"))
			{
				var name = Read(element, "NAME");
				if (string.IsNullOrWhiteSpace(name)) continue;
				var match = known.FirstOrDefault(x => EqualsText(x.Name, name));
				recipe.Yeasts.Add(new RecipeYeast
				{
					Rank = rank++, IngredientId = match == null ? 0 : match.IngredientId,
					Yeast = match ?? new Yeast { Name = name }, Attenuation = (ReadDouble(element, "ATTENUATION") ?? 75d) / 100d
				});
			}
		}

		void SetMiscs(Recipe recipe, XElement entry, IList<Adjunct> known)
		{
			var container = Child(entry, "MISCS");
			if (container == null) return;
			var rank = 1;
			foreach (var element in Children(container, "MISC"))
			{
				var name = Read(element, "NAME");
				if (string.IsNullOrWhiteSpace(name)) continue;
				var match = known.FirstOrDefault(x => EqualsText(x.Name, name));
				var isWeight = ReadBool(element, "AMOUNT_IS_WEIGHT");
				recipe.Adjuncts.Add(new RecipeAdjunct
				{
					Rank = rank++, IngredientId = match == null ? 0 : match.IngredientId, Adjunct = match ?? new Adjunct { Name = name },
					Unit = isWeight ? "oz" : "floz", Amount = isWeight ? Ounces(ReadDouble(element, "AMOUNT")) : FluidOunces(ReadDouble(element, "AMOUNT")),
					AdjunctUsageTypeId = AdjunctUsageFrom(Read(element, "USE"))
				});
			}
		}

		void SetMashSteps(Recipe recipe, XElement entry)
		{
			var mash = Child(entry, "MASH");
			var steps = mash == null ? null : Child(mash, "MASH_STEPS");
			if (steps == null) return;
			var rank = 1;
			foreach (var element in Children(steps, "MASH_STEP"))
			{
				var name = Read(element, "NAME");
				if (string.IsNullOrWhiteSpace(name)) name = "Mash Step " + rank;
				recipe.MashSteps.Add(new RecipeMashStep
				{
					Rank = rank++, IngredientId = 0, MashStep = new MashStep { Name = name }, Heat = Read(element, "TYPE"),
					Temp = Fahrenheit(ReadDouble(element, "STEP_TEMP")), Time = ReadInt(element, "STEP_TIME") ?? 0
				});
			}
		}

		static bool IsNamed(XElement element, string name) { return string.Equals(element.Name.LocalName, name, StringComparison.OrdinalIgnoreCase); }
		static XElement Child(XElement parent, string name) { return parent == null ? null : parent.Elements().FirstOrDefault(x => IsNamed(x, name)); }
		static IEnumerable<XElement> Children(XElement parent, string name) { return parent == null ? Enumerable.Empty<XElement>() : parent.Elements().Where(x => IsNamed(x, name)); }
		static string Read(XElement parent, string name) { var element = Child(parent, name); return element == null ? string.Empty : element.Value.Trim(); }
		static double? ReadDouble(XElement parent, string name) { double value; return double.TryParse(Read(parent, name), NumberStyles.Float, CultureInfo.InvariantCulture, out value) ? value : (double?)null; }
		static int? ReadInt(XElement parent, string name) { double? value = ReadDouble(parent, name); return value.HasValue ? (int?)Math.Round(value.Value, MidpointRounding.AwayFromZero) : null; }
		static bool ReadBool(XElement parent, string name) { var value = Read(parent, name); return EqualsText(value, "true") || value == "1"; }
		static bool EqualsText(string left, string right) { return string.Equals((left ?? string.Empty).Trim(), (right ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase); }
		double Gallons(double? liters, double fallback) { return liters.HasValue ? Round(RecipeUnitConverter.ConvertLitersToGallons(liters.Value)) : fallback; }
		double Pounds(double? kilograms) { return kilograms.HasValue ? Round(RecipeUnitConverter.ConvertKilogramsToPounds(kilograms.Value)) : 0; }
		double Ounces(double? kilograms) { return kilograms.HasValue ? Round(RecipeUnitConverter.ConvertKilogramsToOunces(kilograms.Value)) : 0; }
		double FluidOunces(double? liters) { return liters.HasValue ? Round(RecipeUnitConverter.ConvertLitersToFluidOunces(liters.Value)) : 0; }
		double Fahrenheit(double? celsius) { return celsius.HasValue ? Round(celsius.Value * 9d / 5d + 32d) : 0; }
		static double Round(double value) { return Math.Round(value, 4, MidpointRounding.AwayFromZero); }
		static int RecipeTypeFrom(string type) { return type.IndexOf("extract", StringComparison.OrdinalIgnoreCase) >= 0 && type.IndexOf("grain", StringComparison.OrdinalIgnoreCase) >= 0 ? (int)RecipeType.AllGrainPlusExtract : type.IndexOf("extract", StringComparison.OrdinalIgnoreCase) >= 0 ? (int)RecipeType.Extract : (int)RecipeType.AllGrain; }
		static int HopTypeFrom(string form) { return EqualsText(form, "leaf") ? (int)HopType.Leaf : EqualsText(form, "plug") ? (int)HopType.Plug : (int)HopType.Pellet; }
		static int HopUsageFrom(string use) { return EqualsText(use, "mash") ? (int)HopUsageType.Mash : use.IndexOf("dry", StringComparison.OrdinalIgnoreCase) >= 0 ? (int)HopUsageType.DryHop : use.IndexOf("flame", StringComparison.OrdinalIgnoreCase) >= 0 || use.IndexOf("aroma", StringComparison.OrdinalIgnoreCase) >= 0 ? (int)HopUsageType.FlameOut : use.IndexOf("first", StringComparison.OrdinalIgnoreCase) >= 0 ? (int)HopUsageType.FirstWort : (int)HopUsageType.Boil; }
		static int FermentableUsageFrom(string type) { return type.IndexOf("extract", StringComparison.OrdinalIgnoreCase) >= 0 ? (int)FermentableUsageType.Extract : type.IndexOf("sugar", StringComparison.OrdinalIgnoreCase) >= 0 ? (int)FermentableUsageType.Late : type.IndexOf("steep", StringComparison.OrdinalIgnoreCase) >= 0 ? (int)FermentableUsageType.Steep : (int)FermentableUsageType.Mash; }
		static int AdjunctUsageFrom(string use) { return EqualsText(use, "mash") ? (int)AdjunctUsageType.Mash : use.IndexOf("flame", StringComparison.OrdinalIgnoreCase) >= 0 ? (int)AdjunctUsageType.FlameOut : use.IndexOf("primary", StringComparison.OrdinalIgnoreCase) >= 0 ? (int)AdjunctUsageType.Primary : use.IndexOf("secondary", StringComparison.OrdinalIgnoreCase) >= 0 ? (int)AdjunctUsageType.Secondary : use.IndexOf("bottle", StringComparison.OrdinalIgnoreCase) >= 0 ? (int)AdjunctUsageType.Bottle : (int)AdjunctUsageType.Boil; }
	}
}
