using System;
using System.Collections;
using System.Collections.Generic;

namespace Openbrew.Web.Core.Model
{
	public class BrewSession
	{
		/// <summary>
		/// Gets or sets the BrewSessionId
		/// </summary>
		public int BrewSessionId { get; set; }

		/// <summary>
		/// Gets or sets the RecipeId
		/// </summary>
		public int RecipeId { get; set; }

		/// <summary>
		/// Gets or sets the RecipeSummary
		/// </summary>
		public virtual RecipeSummary RecipeSummary { get; set; }

		/// <summary>
		/// Gets or sets the UserId
		/// </summary>
		public int UserId { get; set; }

		/// <summary>
		/// Gets or sets the BrewedByUser
		/// </summary>
		public virtual User BrewedByUser { get; set; }

		/// <summary>
		/// Gets or sets the UnitTypeId
		/// </summary>
		public int UnitTypeId { get; set; }

		/// <summary>
		/// Gets or sets the BrewDate
		/// </summary>
		public DateTime BrewDate { get; set; }

		/// <summary>
		/// Gets or sets the Notes
		/// </summary>
		public string Notes { get; set; }

		/// <summary>
		/// Gets or sets the GrainWeight
		/// </summary>
		public double? GrainWeight { get; set; }

		/// <summary>
		/// Gets or sets the GrainTemp
		/// </summary>
		public double? GrainTemp { get; set; }

		/// <summary>
		/// Gets or sets the BoilTime
		/// </summary>
		public double? BoilTime { get; set; }

		/// <summary>
		/// Gets or sets the BoilVolumeEst
		/// </summary>
		public double? BoilVolumeEst { get; set; }

		/// <summary>
		/// Gets or sets the FermentVolumeEst
		/// </summary>
		public double? FermentVolumeEst { get; set; }

		/// <summary>
		/// Gets or sets the TargetMashTemp
		/// </summary>
		public double? TargetMashTemp { get; set; }

		/// <summary>
		/// Gets or sets the MashThickness
		/// </summary>
		public double? MashThickness { get; set; }

		/// <summary>
		/// Gets or sets the TotalWaterNeeded
		/// </summary>
		public double? TotalWaterNeeded { get; set; }

		/// <summary>
		/// Gets or sets the StrikeWaterTemp
		/// </summary>
		public double? StrikeWaterTemp { get; set; }

		/// <summary>
		/// Gets or sets the StrikeWaterVolume
		/// </summary>
		public double? StrikeWaterVolume { get; set; }

		/// <summary>
		/// Gets or sets the FirstRunningsVolume
		/// </summary>
		public double? FirstRunningsVolume { get; set; }

		/// <summary>
		/// Gets or sets the SpargeWaterVolume
		/// </summary>
		public double? SpargeWaterVolume { get; set; }

		/// <summary>
		/// Gets or sets the BrewKettleLoss
		/// </summary>
		public double? BrewKettleLoss { get; set; }

		/// <summary>
		/// Gets or sets the WortShrinkage
		/// </summary>
		public double? WortShrinkage { get; set; }

		/// <summary>
		/// Gets or sets the MashTunLoss
		/// </summary>
		public double? MashTunLoss { get; set; }

		/// <summary>
		/// Gets or sets the BoilLoss
		/// </summary>
		public double? BoilLoss { get; set; }

		/// <summary>
		/// Gets or sets the MashGrainAbsorption
		/// </summary>
		public double? MashGrainAbsorption { get; set; }

		/// <summary>
		/// Gets or sets the SpargeGrainAbsorption
		/// </summary>
		public double? SpargeGrainAbsorption { get; set; }

		/// <summary>
		/// Gets or sets the MashPH
		/// </summary>
		public double? MashPH { get; set; }

		/// <summary>
		/// Gets or sets the MashStartTemp
		/// </summary>
		public double? MashStartTemp { get; set; }

		/// <summary>
		/// Gets or sets the MashEndTemp
		/// </summary>
		public double? MashEndTemp { get; set; }

		/// <summary>
		/// Gets or sets the MashTime
		/// </summary>
		public int? MashTime { get; set; }

		/// <summary>
		/// Gets or sets the BoilVolumeActual
		/// </summary>
		public double? BoilVolumeActual { get; set; }

		/// <summary>
		/// Gets or sets the PreBoilGravity
		/// </summary>
		public double? PreBoilGravity { get; set; }

		/// <summary>
		/// Gets or sets the BoilTimeActual
		/// </summary>
		public int? BoilTimeActual { get; set; }

		/// <summary>
		/// Gets or sets the PostBoilVolume
		/// </summary>
		public double? PostBoilVolume { get; set; }

		/// <summary>
		/// Gets or sets the FermentVolumeActual
		/// </summary>
		public double? FermentVolumeActual { get; set; }

		/// <summary>
		/// Gets or sets the OriginalGravity
		/// </summary>
		public double? OriginalGravity { get; set; }

		/// <summary>
		/// Gets or sets the FinalGravity
		/// </summary>
		public double? FinalGravity { get; set; }

		/// <summary>
		/// Gets or sets the ConditionDate
		/// </summary>
		public DateTime? ConditionDate { get; set; }

		/// <summary>
		/// Gets or sets the ConditionTypeId
		/// </summary>
		public int? ConditionTypeId { get; set; }

		/// <summary>
		/// Gets or sets the PrimingSugarType
		/// </summary>
		public string PrimingSugarType { get; set; }

		/// <summary>
		/// Gets or sets the PrimingSugarAmount
		/// </summary>
		public double? PrimingSugarAmount { get; set; }

		/// <summary>
		/// Gets or sets the KegPSI
		/// </summary>
		public int? KegPSI { get; set; }

		/// <summary>
		/// Gets or sets the IsPublic
		/// </summary>
		public bool IsPublic { get; set; }

		/// <summary>
		/// Gets or sets the IsActive
		/// </summary>
		public bool IsActive { get; set; }

		/// <summary>
		/// Gets or sets the DateCreated
		/// </summary>
		public DateTime DateCreated { get; set; }

		/// <summary>
		/// Gets or sets the DateModified
		/// </summary>
		public DateTime? DateModified { get; set; }

		/// <summary>
		/// Gets or sets the TastingNotes
		/// </summary>
		public IList<TastingNote> TastingNotes { get; set; }

		/// <summary>
		/// Determines if a recipe was brewed by a specific user 
		/// </summary>
		public bool WasBrewedBy(int userId)
		{
			return this.UserId == userId;
		}
	}
}