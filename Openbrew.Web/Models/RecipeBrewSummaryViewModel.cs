using Openbrew.Web.Core.Model;
using System;

namespace Openbrew.Web.Models
{
    public class BrewSessionSummaryViewModel
    {
        /// <summary>
        /// Gets or sets the ShowAddedBy
        /// </summary>
        public bool ShowAddedBy { get; set; }

        /// <summary>
        /// Gets or sets the BrewSessionId
        /// </summary>
        public int BrewSessionId { get; set; }

        /// <summary>
        /// Gets or sets the RecipeId
        /// </summary>
        public int RecipeId { get; set; }

        /// <summary>
        /// Gets or sets the RecipeTypeId
        /// </summary>
        public int RecipeTypeId { get; set; }

        /// <summary>
        /// Gets or sets the RecipeType
        /// </summary>
        public RecipeType GetRecipeType()
        {
            return (RecipeType)this.RecipeTypeId;
        }

        /// <summary>
        /// Gets or sets the RecipeName
        /// </summary>
        public string RecipeName { get; set; }

        /// <summary>
        /// Gets or sets the RecipeBjcpStyleSubCategoryId
        /// </summary>
        public string RecipeBjcpStyleSubCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the RecipeBjcpStyleName
        /// </summary>
        public string RecipeBjcpStyleName { get; set; }

        /// <summary>
        /// Gets or sets the BrewedBy
        /// </summary>
        public int BrewedBy { get; set; }

        /// <summary>
        /// Gets or sets the BrewedByUsername
        /// </summary>
        public string BrewedByUsername { get; set; }

        /// <summary>
        /// Gets or sets the BrewedByUserEmail
        /// </summary>
        public string BrewedByUserEmail { get; set; }

        /// <summary>
        /// Gets or sets the BrewDate
        /// </summary>
        public DateTime BrewDate { get; set; }

        /// <summary>
        /// Gets or sets the PostalCode
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the Summary
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets the RecipeImageUrlRoot
        /// </summary>
        public string RecipeImageUrlRoot { get; set; }

        /// <summary>
        /// Gets or sets the IsActive
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the IsPublic
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        /// Gets or sets the HasMashBoil
        /// </summary>
        public bool HasMashBoil { get; set; }

        /// <summary>
        /// Gets or sets the HasWaterInfusion
        /// </summary>
        public bool HasWaterInfusion { get; set; }

        /// <summary>
        /// Gets or sets the HasFermentation
        /// </summary>
        public bool HasFermentation { get; set; }

        /// <summary>
        /// Gets or sets the HasConditioning
        /// </summary>
        public bool HasConditioning { get; set; }

        /// <summary>
        /// Gets or sets the HasTastingNotes
        /// </summary>
        public bool HasTastingNotes { get; set; }

        /// <summary>
        /// Gets or sets the DateCreated
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the DateModified
        /// </summary>
        public DateTime? DateModified { get; set; }

        /// <summary>
        /// Gets or sets the RecipeSrm
        /// </summary>
        public double? RecipeSrm { get; set; }

        /// <summary>
        /// Gets the RecipeCommentSummary
        /// </summary>
        public CommentWrapperViewModel CommentWrapperViewModel { get; set; }

        /// <summary>
        /// Determines if the brew session was brewed by a specific user
        /// </summary>
        public bool WasBrewedBy(int userId)
        {
            return this.BrewedBy == userId;
        }

        /// <summary>
        /// Determines if the BrewSession has an image
        /// </summary>
        public bool HasImage()
        {
            return !string.IsNullOrWhiteSpace(this.RecipeImageUrlRoot);
        }

        /// <summary>
        /// Gets the Avatar
        /// </summary>
        public string GetAvatar(int size)
        {
            return UserAvatar.GetAvatar(size, this.BrewedByUserEmail);
        }
    }
}