using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Openbrew.Web.Controllers;
using ctorx.Core.Web;

namespace Openbrew.Web
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapMvcAttributeRoutes();

			// App Entry Point
			routes.MapEntryRoute("Root", "Index");

			// Search Route
			routes.MapRoute("SearchRoute", "search/{searchTerm}", new { controller = "Search", Action = "Search", searchTerm = UrlParameter.Optional });
			routes.MapRoute("NewsletterConfirmRoute", "newsletter/confirm/{token}", new { controller = "Marketing", action = "NewsletterConfirm" });
			routes.MapRoute("NewsletterUnsubscribeRoute", "newsletter/unsubscribe", new { controller = "Marketing", action = "NewsletterUnsubscribe" });

			// User Routes
			routes.MapRoute("UserRoute", "!/{username}", new { controller = "User", action = "UserProfile" });

			// Recipe Routes
			routes.MapRoute("RecipeEditRoute", "recipe/{recipeid}/edit", new { controller = "Recipe", action = "RecipeEdit", recipename = UrlParameter.Optional }, new { recipeid = @"\d+" });
			routes.MapRoute("RecipeCloneRoute", "recipe/{recipeid}/clone", new { controller = "Recipe", action = "RecipeClone", recipename = UrlParameter.Optional }, new { recipeid = @"\d+" });
			routes.MapRoute("RecipeDeleteRoute", "recipe/{recipeid}/delete", new { controller = "Recipe", action = "RecipeDelete" }, new { recipeid = @"\d+" });
			
			routes.MapRoute("RecipeBrewSessionsRoute", "recipe/{recipeid}/{recipename}/brew-sessions", new { controller = "Recipe", action = "RecipeBrewSessions", recipename = UrlParameter.Optional }, new { recipeid = @"\d+" });

			routes.MapRoute("RecipeBeerXmlRoute", "recipe/{recipeid}/beerxml", new { controller = "Recipe", action = "BeerXml" }, new { recipeid = @"\d+" });
			
			routes.MapRoute("RecipeDetailRoute", "recipe/{recipeid}/{recipename}", new { controller = "Recipe", action = "RecipeDetail", recipename = UrlParameter.Optional }, new { recipeid = @"\d+" });
			routes.MapRoute("RecipeBuilderRoute", "homebrew-recipe-calculator", new { controller = "Recipe", action = "homebrew-recipe-calculator" });
			routes.MapRoute("RecipeBuilderLegacyRoute", "homebrew-recipe-builder", new { controller = "Recipe", action = "homebrew-recipe-builder" });
			routes.MapRoute("RecipeBuilderTemplatesRoute", "buildertemplates-v2", new { controller = "Recipe", action = "buildertemplates-v2" });
			routes.MapRoute("StyleDetailRoute", "style/{urlfriendlyname}/{page}", new { controller = "Recipe", action = "StyleDetail", page = UrlParameter.Optional });
			routes.MapRoute("Ba2026StyleDetailRoute", "styles/2026/{styleId}/{slug}", new { controller = "Recipe", action = "Ba2026StyleDetail", slug = UrlParameter.Optional }, new { styleId = @"\d+" });
			routes.MapRoute("Ba2026StyleRangeRoute", "ba-2026-style-range", new { controller = "Recipe", action = "ba-2026-style-range" });
			routes.MapRoute("Ba2026StyleComparisonsRoute", "ba-2026-style-comparisons", new { controller = "Recipe", action = "ba-2026-style-comparisons" });
			routes.MapRoute("UnCategorizedRecipesRoute", "recipes/other-homebrew-recipes/{page}", new { controller = "Recipe", action = "other-homebrew-recipes", page = UrlParameter.Optional });

			// Content Routes
			routes.MapRoute("HowItWorksLegacyRoute", "howitworks", new { controller = "Root", action = "HowItWorksLegacy" });
			routes.MapRoute("HowItWorksRoute", "how-it-works", new { controller = "Root", action = "HowItWorks" });
			routes.MapRoute("BlogRoute", "blog", new { controller = "Root", action = "Blog" });
			routes.MapRoute("ContentCalculations", "calculations/", new { controller = "Content", action = "Calculations" });
			routes.MapRoute("ContentCalculationsOG", "calculations/original-gravity", new { controller = "Content", action = "CalculationsOriginalGravity" });
			routes.MapRoute("ContentCalculationsFG", "calculations/final-gravity", new { controller = "Content", action = "CalculationsFinalGravity" });
			routes.MapRoute("ContentCalculationsSRM", "calculations/srm-beer-color", new { controller = "Content", action = "CalculationsSRM" });
			routes.MapRoute("ContentCalculationsIBU", "calculations/ibu-hop-bitterness", new { controller = "Content", action = "CalculationsIBU" });
			routes.MapRoute("ContentCalculationsABV", "calculations/alcohol-content", new { controller = "Content", action = "CalculationsAlcohol" });
			routes.MapRoute("ContentCalculationsCal", "calculations/calories", new { controller = "Content", action = "CalculationsCalories" });
			routes.MapRoute("CalculatorsHydrometerCorrection", "calculators/hydrometer-correction", new { controller = "Content", action = "CalculatorsHydrometerTemp" });
			routes.MapRoute("CalculatorsMashSpargeWater", "calculators/mash-sparge-water-insusion", new { controller = "Content", action = "CalculatorsMashSpargeWater" });



			// Ingredient Routes
			routes.MapRoute("FermentableRoute", "fermentables/{ingredientId}/{name}/{page}", new { controller = "Ingredient", action = "FermentableDetail", name = UrlParameter.Optional, page = UrlParameter.Optional }, new { ingredientId = @"\d+" });
			routes.MapRoute("HopRoute", "hops/{ingredientId}/{name}", new { controller = "Ingredient", action = "HopDetail", name = UrlParameter.Optional }, new { ingredientId = @"\d+" });
			routes.MapRoute("YeastRoute", "yeasts/{ingredientId}/{name}", new { controller = "Ingredient", action = "YeastDetail", name = UrlParameter.Optional }, new { ingredientId = @"\d+" });
			routes.MapRoute("AdjunctRoute", "adjuncts/{ingredientId}/{name}", new { controller = "Ingredient", action = "AdjunctDetail", name = UrlParameter.Optional }, new { ingredientId = @"\d+" });

			// Dashboard Routes
			routes.MapRoute("DashboardItems", "AllMyDashboardItems", new { controller = "Dashboard", action = "AllMyDashboardItems" });

			// Admin Routes
			routes.MapRoute("ExceptionRoute", "admin/exceptions/{resource}/{subResource}", new { controller = "Admin", action = "Exceptions", resource = UrlParameter.Optional, subResource = UrlParameter.Optional });

			// Route all Inferred Routes
			routes.MapInferredRoutes(
				controllerSearchType: typeof(BrewgrController),
				controllerNameMappings: new List<ControllerNameMap>()
				{
				    new ControllerNameMap { SourceName = "Root", TargetName = "" },
					new ControllerNameMap { SourceName = "Error", TargetName = "" },
					new ControllerNameMap { SourceName = "Auth", TargetName = "" },
					new ControllerNameMap { SourceName = "User", TargetName = "" },
					new ControllerNameMap { SourceName = "Recipe", TargetName = "" },
					new ControllerNameMap { SourceName = "BrewSession", TargetName = "" },
					new ControllerNameMap { SourceName = "Search", TargetName = "" },
					new ControllerNameMap { SourceName = "Ingredient", TargetName = "" },
                    new ControllerNameMap { SourceName = "Dashboard", TargetName = "" }
				});

			// Catch-All for 404 Route
			routes.MapCatchAllFor404("Error", "404");
		}	 
	}
}
