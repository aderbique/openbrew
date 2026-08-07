using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Openbrew.Web.Core.Model;
using ctorx.Core.Formatting;

namespace Openbrew.Web.Models
{
    public class RecipeDetailViewModel
    {

        /// <summary>
        /// Gets and sets RecipeViewModel
        /// </summary>
        public RecipeViewModel RecipeViewModel { get; set; }

        /// <summary>
        /// Gets or sets user summary
        /// </summary>
        public UserSummary UserSummary { get; set; }

	    /// <summary>
	    /// Gets or sets the TastingNotes
	    /// </summary>
	    public IList<TastingNote> TastingNotes { get; set; }

		/// <summary>
		/// Gets the Detail Description
		/// </summary>
		/// <returns></returns>
    	public string GetDescription()
    	{
			// Has Description
			if(!string.IsNullOrWhiteSpace(this.RecipeViewModel.Description))
			{
				return StringShortener.Shorten(this.RecipeViewModel.Description, 155);
			}

			var builder = new StringBuilder();

			// Has Style
			if(!string.IsNullOrWhiteSpace(this.RecipeViewModel.StyleName))
			{
				builder.Append(this.RecipeViewModel.StyleName.Trim());
			}

			builder.Append(" " + HumanReadableFormatter.AddSpacesToPascalCaseString(this.RecipeViewModel.GetRecipeType().ToString()));
			builder.Append(" homebrew recipe. ");

			// Add First Fermentable Name
			if(this.RecipeViewModel.Fermentables.Any())
			{
				builder.Append(this.RecipeViewModel.Fermentables.First().Name.Trim() + (this.RecipeViewModel.Fermentables.First().Name.Trim().ToLower().EndsWith("malt") ? ". " : " malt. "));
			}

			// Add First Hop Name
			if(this.RecipeViewModel.Hops.Any())
			{
				builder.Append(this.RecipeViewModel.Hops.First().Name.Trim() + " hops. ");
			}

			// Add First Yeast Name
			if(this.RecipeViewModel.Yeasts.Any())
			{
				builder.Append(this.RecipeViewModel.Yeasts.First().Name.Trim() + " homebrew yeast. ");
			}

			// Add First Adjunct Name
			if (this.RecipeViewModel.Others.Any())
			{
				builder.Append(this.RecipeViewModel.Others.First().Name.Trim() + " homebrew ingredient. ");
			}

			return builder.ToString();
    	}

		/// <summary>
		/// Gets an inferred description
		/// </summary>
		public string GetInferredDescription()
		{
			var builder = new StringBuilder();

			builder.Append(HumanReadableFormatter.AddSpacesToPascalCaseString(this.RecipeViewModel.GetRecipeType().ToString()));

			// Has Style
			if (!string.IsNullOrWhiteSpace(this.RecipeViewModel.StyleName))
			{
				builder.Append(" " + this.RecipeViewModel.StyleName.Trim());
			}

			builder.Append(" homebrew recipe. ");

			// Ingredients
			if (this.RecipeViewModel.Fermentables.Any() || this.RecipeViewModel.Hops.Any() || this.RecipeViewModel.Yeasts.Any() ||
			    this.RecipeViewModel.Others.Any())
			{
				builder.Append("This homebrew recipe uses the following ingredients: ");

				var ingredientNames =
					this.RecipeViewModel.Fermentables.Select(x => x.Name)
						.Union(this.RecipeViewModel.Hops.Select(x => x.Name + " Hops")
						.Union(this.RecipeViewModel.Yeasts.Select(x => x.Name + " Homebrew Yeast")
						.Union(this.RecipeViewModel.Others.Select(x => x.Name))));

				builder.Append(string.Join(", ", ingredientNames) + ".");
			}

			return builder.ToString();
		}

		/// <summary>
		/// Gets the Title
		/// </summary>
    	public string GetTitle()
    	{
			if(!string.IsNullOrWhiteSpace(this.RecipeViewModel.StyleName))
			{
				return this.RecipeViewModel.StyleName + " Recipe - " + this.RecipeViewModel.Name;
			}

			return this.RecipeViewModel.Name + " Recipe";
    	}
    }
}