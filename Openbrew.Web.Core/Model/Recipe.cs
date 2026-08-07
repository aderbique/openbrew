using System;
using System.Collections.Generic;
using System.Linq;

namespace Openbrew.Web.Core.Model
{
	public class Recipe
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
        /// Gets or sets the RecipeName
        /// </summary>
        public string RecipeName { get; set; }

		/// <summary>
		/// Gets or sets the ImageUrlRoot
		/// </summary>
		public string ImageUrlRoot { get; set; }

		/// <summary>
		/// Gets or sets the Description
		/// </summary>
		public string Description { get; set; }

        /// <summary>
        /// Gets or sets the CreatedBy
        /// </summary>
        public int CreatedBy { get; set; }

		/// <summary>
		/// Gets or sets the User
		/// </summary>
		public virtual User User { get; set; }

        /// <summary>
		/// Gets or sets the BjcpStyleSubCategoryId
        /// </summary>
		public string BjcpStyleSubCategoryId { get; set; }

		/// <summary>Gets or sets the catalog that owns the selected style.</summary>
		public string StyleCatalog { get; set; }

		/// <summary>Gets or sets the style identifier within StyleCatalog.</summary>
		public string CatalogStyleCode { get; set; }

		/// <summary>Gets or sets the style display name captured from StyleCatalog.</summary>
		public string CatalogStyleName { get; set; }

		/// <summary>
		/// Gets or sets the BjcpStyle
		/// </summary>
		public BjcpStyle BjcpStyle { get; set; }

		/// <summary>
		/// Gets or sets the BjcpStyleSummary
		/// </summary>
		public BjcpStyleSummary BjcpStyleSummary { get; set; }

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
		/// Get or sets the Efficiency
        /// </summary>
        public double Efficiency { get; set; }

        /// <summary>
        /// Gets or sets the Fermentables
        /// </summary>
        public virtual IList<RecipeFermentable> Fermentables { get; set; }

        /// <summary>
        /// Gets or sets the Hops
        /// </summary>
		public virtual IList<RecipeHop> Hops { get; set; }

        /// <summary>
        /// Gets or sets the Yeast
        /// </summary>
		public virtual IList<RecipeYeast> Yeasts { get; set; }

        /// <summary>
        /// Gets or sets the Adjuncts
        /// </summary>
        public virtual IList<RecipeAdjunct> Adjuncts { get; set; }

        /// <summary>
        /// Gets or sets the MashSteps
        /// </summary>
        public virtual IList<RecipeMashStep> MashSteps { get; set; }

        /// <summary>
		/// Gets or sets the RecipeSteps
		/// </summary>
		public virtual IList<RecipeStep> Steps { get; set; }

		/// <summary>
		/// Gets or sets the RecipeComments
		/// </summary>
		public virtual IList<RecipeComment> RecipeComments { get; set; }

		/// <summary>
		/// Gets or sets the BrewSessions
		/// </summary>
		public virtual IList<BrewSession> BrewSessions { get; set; }

		/// <summary>
		/// Gets or sets the RecipeSummary
		/// </summary>
		public virtual RecipeSummary RecipeSummary { get; set; }

		/// <summary>
		/// Gets or sets the RecipeMetaData
		/// </summary>
		public RecipeMetaData RecipeMetaData { get; set; }

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
		/// Gets or sets the IsActive
		/// </summary>
		public bool IsActive { get; set; }

		/// <summary>
		/// Gets or sets the IsPublic
		/// </summary>
		public bool IsPublic { get; set; }

		/// <summary>
		/// Gets or sets the DateCreated
		/// </summary>
		public DateTime DateCreated { get; set; }

		/// <summary>
		/// Gets or sets the DateModified
		/// </summary>
		public DateTime? DateModified { get; set; }

		/// <summary>
		/// Gets or sets the UnitTypeId
		/// </summary>
		public int UnitTypeId { get; set; }

		/// <summary>
		/// Gets the Unit Type
		/// </summary>
		public UnitType GetUnitType()
		{
			return (UnitType)this.UnitTypeId;
		}

		/// <summary>
		/// Gets or sets the IbuFormulaId
		/// </summary>
		public int IbuFormulaId { get; set; }

		/// <summary>
		/// Gets the IbuFormula
		/// </summary>
		public IbuFormula GetIbuFormula()
		{
			return (IbuFormula)this.IbuFormulaId;
		}

		/// <summary>
		/// Determines if the recipe was created by a specific user
		/// </summary>
		public bool WasCreatedBy(int userId)
		{
			return this.CreatedBy.Equals(userId);
		}

		/// <summary>
		/// Determines if the recipe is a new recipe
		/// </summary>
		public bool IsNewRecipe()
		{
			return this.RecipeId == 0;
		}

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public Recipe()
		{
			this.Fermentables = new List<RecipeFermentable>();
			this.Hops = new List<RecipeHop>();
			this.Yeasts = new List<RecipeYeast>();
            this.Adjuncts = new List<RecipeAdjunct>();
            this.MashSteps = new List<RecipeMashStep>();
            this.Steps = new List<RecipeStep>();
		}
	}
}
