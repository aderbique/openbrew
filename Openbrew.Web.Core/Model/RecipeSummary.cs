using System;
using System.Collections.Generic;
using System.Linq;
using ctorx.Core.Formatting;

namespace Openbrew.Web.Core.Model
{
	public class RecipeSummary
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
		/// Gets or sets the OriginalRecipeName
		/// </summary>
		public string OriginalRecipeName { get; set; }

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
		/// Gets or sets the CreatedByUsername
		/// </summary>
		public string CreatedByUserName { get; set; }

        /// <summary>
        /// Gets or sets the CreatedByUserEmail
        /// </summary>
        public string CreatedByUserEmail { get; set; }

		/// <summary>
		/// Gets or sets the BjcpStyleSubCategoryId
		/// </summary>
		public string BjcpStyleSubCategoryId { get; set; }

		/// <summary>
		/// Gets or sets the BJCPStyleName
		/// </summary>
		public string BJCPStyleName { get; set; }

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
		/// Gets or sets the UserIsAdmin
		/// </summary>
		public bool UserIsAdmin { get; set; }

		/// <summary>
		/// Gets or sets the BrewSessionCount
		/// </summary>
		public int BrewSessionCount { get; set; }

		/// <summary>
        /// Gets or sets the RecipeComments
        /// </summary>
        public IList<RecipeComment> RecipeComments { get; set; }

		/// <summary>
		/// Determines if the Recipe has an Image
		/// </summary>
		public bool HasImage()
		{
			return !string.IsNullOrWhiteSpace(this.ImageUrlRoot);	
		}

		/// <summary>
		/// Determines if the recipe was created by a specific user
		/// </summary>
		public bool WasCreatedBy(int userId)
		{
			return this.CreatedBy.Equals(userId);
		}

		/// <summary>
		/// Gets the Recipe Type
		/// </summary>
		public RecipeType GetRecipeType()
		{
			return (RecipeType)this.RecipeTypeId;
		}

		/// <summary>
		/// Gets the Recipe Type Name
		/// </summary>
		public string GetRecipeTypeName()
		{
			return HumanReadableFormatter.AddSpacesToPascalCaseString(this.GetRecipeType().ToString()).Replace(" Plus ", " + ");
		}
	}
}