using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using ctorx.Core.Formatting;
using ctorx.Core.Validation;
using System.Linq;
using Openbrew.Web.Core.Model;

namespace Openbrew.Web.Models
{
	public class RecipeViewModel : IRecipeFactsViewModel
	{
		/// <summary>
		/// Gets or sets the RecipeId
		/// </summary>
		public int RecipeId { get; set; }

        /// <summary>
        /// Gets or sets the RecipeTypeId
        /// </summary>
        public int RecipeTypeId { get; set; }

		/// <summary>
		/// Gets or sets the OriginalRecipeId
		/// </summary>
		public int? OriginalRecipeId { get; set; }

		/// <summary>
		/// Gets the OriginalRecipe
		/// </summary>
		[ScriptIgnore]
		public RecipeViewModel OriginalRecipe { get; set; }

		/// <summary>
		/// Gets or sets the UnitType
		/// </summary>
		public string UnitType { get; set; }

		/// <summary>
		/// Gets or sets the IbuFormula
		/// </summary>
		public string IbuFormula { get; set; }

		/// <summary>
		/// Gets or sets the CreatedBy
		/// </summary>
		public int CreatedBy { get; set; }

		/// <summary>
		/// Gets or sets the CreatedByUser
		/// </summary>
		[ScriptIgnore]
		public UserSummary CreatedByUser { get; set; }

		/// <summary>
		/// Gets or sets the RecipeName
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the Description
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the ImageUrlRoot
		/// </summary>
		public string ImageUrlRoot { get; set; }

		/// <summary>
		/// Gets or sets the HasImage
		/// </summary>
		[ScriptIgnore]
		public bool HasImage 
		{
			get { return !string.IsNullOrWhiteSpace(this.ImageUrlRoot); }
		}

		/// <summary>
		/// Gets or sets the PhotoForUpload
		/// </summary>
		[ScriptIgnore]
		public HttpPostedFileBase PhotoForUpload { get; set; }

		/// <summary>
		/// Gets or sets the BjcpSubCategoryId
		/// </summary>
		public string StyleId { get; set; }

		/// <summary>Gets or sets the catalog that owns StyleId.</summary>
		public string StyleCatalog { get; set; }

		/// <summary>
		/// Gets or sets the BjcpSubCategoryName
		/// </summary>
		public string StyleName { get; set; }

		/// <summary>
		/// Gets or sets the BatchSize
		/// </summary>
		public double BatchSize { get; set; }

		/// <summary>
		/// Gets or sets the BoilSize
		/// </summary>
		public double BoilSize { get; set; }

		/// <summary>
		/// Gets or sets the BoilTimeInMinutes
		/// </summary>
		public int BoilTime { get; set; }

		/// <summary>
		/// Gets or sets the Efficiency
		/// </summary>
		public double Efficiency { get; set; }

		/// <summary>
		/// Gets or sets the ShowAddedBy
		/// </summary>
		[ScriptIgnore]
		public bool ShowAddedBy { get; set; }

		/// <summary>
		/// Gets or sets the IsPublic
		/// </summary>
		[ScriptIgnore]
		public bool IsPublic { get; set; }

		/// <summary>
		/// Gets or sets the requested sharing mode. Incomplete recipes remain drafts
		/// regardless of this value until they contain fermentables and yeast.
		/// </summary>
		public string Visibility { get; set; }

		/// <summary>
		/// Gets or sets the DateCreated
		/// </summary>
		public DateTime DateCreated { get; set; }

		/// <summary>
		/// Gets or sets the BrewSessionCount
		/// </summary>
		public int BrewSessionCount { get; set; }

		/// <summary>
		/// Gets or sets the MostRecentBrewSession
		/// </summary>
		public int? MostRecentBrewSession { get; set; }

		/// <summary>
		/// Gets or sets the OriginalGravity
		/// </summary>
		public double Og { get; set; }

		/// <summary>
		/// Gets or sets the FinalGravity
		/// </summary>
		public double Fg { get; set; }

		/// <summary>
		/// Gets or sets the SRM
		/// </summary>
		public double Srm { get; set; }

		/// <summary>
		/// Gets or sets the IBU
		/// </summary>
		public double Ibu { get; set; }

		/// <summary>
		/// Gets or sets the BGGU
		/// </summary>
		public double BgGu { get; set; }

		/// <summary>
		/// Gets or sets the ABV
		/// </summary>
		public double Abv { get; set; }

		/// <summary>
		/// Gets or sets the Calories
		/// </summary>
		public int Calories { get; set; }

		/// <summary>
		/// Gets or sets the AverageRating
		/// </summary>
		public double AverageRating { get; set; }

		/// <summary>
		/// Gets or sets the TastingNoteCount
		/// </summary>
		public int TastingNoteCount { get; set; }

		/// <summary>
		/// Gets or sets the Fermentables
		/// </summary>
		public IList<RecipeFermentableViewModel> Fermentables { get; set; }

		/// <summary>
		/// Gets or sets the Hops
		/// </summary>
		public IList<RecipeHopViewModel> Hops { get; set; }

		/// <summary>
		/// Gets or sets the Yeast
		/// </summary>
		public IList<RecipeYeastViewModel> Yeasts { get; set; }

        /// <summary>
        /// Gets or sets the Adjuncts
        /// </summary>
        public IList<RecipeOtherViewModel> Others { get; set; }

        /// <summary>
        /// Gets or sets the MashSteps
        /// </summary>
        public IList<RecipeMashStepViewModel> MashSteps { get; set; }

        /// <summary>
		/// Gets or sets the RecipeStep
		/// </summary>
		public IList<RecipeStepViewModel> Steps { get; set; }

		/// <summary>
		/// Gets or sets the Comments
		/// </summary>
		[ScriptIgnore]
		public CommentWrapperViewModel CommentWrapperViewModel { get; set; }

		/// <summary>
		/// Gets or sets the TastingNotes
		/// </summary>
		[ScriptIgnore]
		public IList<TastingNote> TastingNotes { get; set; }

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public RecipeViewModel()
		{
			this.Fermentables = new List<RecipeFermentableViewModel>();
			this.Hops = new List<RecipeHopViewModel>();
			this.Yeasts = new List<RecipeYeastViewModel>();
			this.Others = new List<RecipeOtherViewModel>();
            this.MashSteps = new List<RecipeMashStepViewModel>();
			this.Steps = new List<RecipeStepViewModel>();
		}

		/// <summary>
		/// Determines if the Recipe is new (unsaved)
		/// </summary>
		public bool IsNewRecipe()
		{
			return this.RecipeId == 0;
		}

        /// <summary>
        /// Gets the Recipe Type
        /// </summary>
        public RecipeType GetRecipeType()
        {
            return (RecipeType)this.RecipeTypeId;
        }

		/// <summary>
		/// Gets the IbuFormula Name
		/// </summary>
		public string GetIbuFormulaName()
		{
			switch(this.IbuFormula)
			{
				case "t":
					return "Tinseth";
				case "r":
					return "Rager";
				case "b":
					return "Brewgr";
				default:
					throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets the Recipe Type Name
		/// </summary>
		public string GetRecipeTypeName()
		{
			return HumanReadableFormatter.AddSpacesToPascalCaseString(this.GetRecipeType().ToString()).Replace(" Plus ", " + ");
		}

		/// <summary>
		/// Gets a list of steps
		/// </summary>
		public IList<RecipeStepViewModel> GetSteps()
		{
			return this.Steps.Where(x => !string.IsNullOrWhiteSpace(x.Text)).ToList();
		}

		/// <summary>
		/// Serializes the recipe to Json
		/// </summary>
		public string GetJson()
		{
			var serializer = new JavaScriptSerializer();
			return serializer.Serialize(this);
		}
	}
}
