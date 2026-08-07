using System;
using Openbrew.Web.Core.Model;
using ctorx.Core.Validation;
using System.ComponentModel;

namespace Openbrew.Web.Models
{
	public class BrewSessionWaterInfusionViewModel : ValidatesWith<BrewSessionWaterInfusionViewModelValidator>
	{
		/// <summary>
		/// Gets or sets the RecipeId
		/// </summary>
		public int BrewSessionId { get; set; }

        /// <summary>
        /// Gets or sets the prop
        /// </summary>
        public double? Grain { get; set; }

        /// <summary>
        /// Gets or sets the prop
        /// </summary>
        public double? GrainTemperature { get; set; }

        /// <summary>
        /// Gets or sets the prop
        /// </summary>
        public TemperatureUnit? GrainTemperatureUnitId { get; set; }

        /// <summary>
        /// Gets or sets the prop
        /// </summary>
        public double? MashThickness { get; set; }

        /// <summary>
        /// Gets or sets the prop
        /// </summary>
        public int? TotalBoilTime { get; set; }

        /// <summary>
        /// Gets or sets the prop
        /// </summary>
        public double? WortIntoFermenter { get; set; }

        /// <summary>
        /// Gets or sets the prop
        /// </summary>
        public double? TargetMashTemperature { get; set; }

        /// <summary>
        /// Gets or sets the prop
        /// </summary>
        public TemperatureUnit? TargetMashTemperatureUnitId { get; set; }

        /// <summary>
        /// Gets or sets the prop
        /// </summary>
        public double? RunOffVolume { get; set; }

        /// <summary>
        /// Gets or sets the prop
        /// </summary>
        public double? TotalWaterNeeded { get; set; }

        /// <summary>
        /// Gets or sets the prop
        /// </summary>
        public double? MashStrikeVolume { get; set; }

        /// <summary>
        /// Gets or sets the prop
        /// </summary>
        public double? FirstRunnings { get; set; }

        /// <summary>
        /// Gets or sets the prop
        /// </summary>
        public double? SpargeVolume { get; set; }

        /// <summary>
        /// Gets or sets the prop
        /// </summary>
        public int? StrikeTemperature { get; set; }

        /// <summary>
        /// Gets or sets the prop
        /// </summary>
        public TemperatureUnit? StrikeTemperatureUnitId { get; set; }

        /// <summary>
        /// Gets or sets the BrewKettleTrubLoss
        /// </summary>
        public double? BrewKettleTrubLoss { get; set; }

        /// <summary>
        /// Gets or sets the WortShrinkage
        /// </summary>
        public double? WortShrinkage { get; set; }

        /// <summary>
        /// Gets or sets the MashTunEquipmentLoss
        /// </summary>
        public double? MashTunEquipmentLoss { get; set; }

        /// <summary>
        /// Gets or sets the BoilLoss
        /// </summary>
        public double? BoilLoss { get; set; }

        /// <summary>
        /// Gets or sets the GrainAbsorption
        /// </summary>
        public double? GrainAbsorption { get; set; }

        /// <summary>
        /// Gets or sets the SpargeGrainAbsorption
        /// </summary>
        public double? SpargeGrainAbsorption { get; set; }

        /// <summary>
        /// Gets or sets the DateCreated
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the DateModified
        /// </summary>
        public DateTime? DateModified { get; set; }

		/// <summary>
		/// Determines if there is water infusion
		/// </summary>
        public bool HasWaterInfusion()
        {
            return this.RunOffVolume != null ||
                this.TotalWaterNeeded != null ||
                this.MashStrikeVolume != null ||
                this.FirstRunnings != null ||
                this.SpargeVolume != null ||
                this.StrikeTemperature != null;
        }
	}
}