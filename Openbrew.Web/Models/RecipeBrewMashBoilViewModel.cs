using System;
using Openbrew.Web.Core.Model;
using ctorx.Core.Validation;

namespace Openbrew.Web.Models
{
	public class BrewSessionMashBoilViewModel
	{
		/// <summary>
		/// Gets or sets the RecipeId
		/// </summary>
		public int BrewSessionId { get; set; }

		/// <summary>
		/// Gets or sets the DoughInTemp
		/// </summary>
		public double? DoughInTemp { get; set; }

		/// <summary>
		/// Gets or sets the DoughInTempUnit
		/// </summary>
		public TemperatureUnit? DoughInTempUnit { get; set; }

		/// <summary>
		/// Gets or sets the DoughInMinutes
		/// </summary>
		public int? DoughInMinutes { get; set; }

		/// <summary>
		/// Gets or sets the AcidRestTemp
		/// </summary>
		public double? AcidRestTemp { get; set; }

		/// <summary>
		/// Gets or sets the AcidRestTempUnit
		/// </summary>
		public TemperatureUnit? AcidRestTempUnit { get; set; }

		/// <summary>
		/// Gets or sets the AcidRestMinutes
		/// </summary>
		public int? AcidRestMinutes { get; set; }

		/// <summary>
		/// Gets or sets the ProteinRestTemp
		/// </summary>
		public double? ProteinRestTemp { get; set; }

		/// <summary>
		/// Gets or sets the ProteinRestTempUnitId
		/// </summary>
		public TemperatureUnit? ProteinRestTempUnit { get; set; }

		/// <summary>
		/// Gets or sets the ProteinRestMinutes
		/// </summary>
		public int? ProteinRestMinutes { get; set; }

		/// <summary>
		/// Gets or sets the IntRestTemp
		/// </summary>
		public double? IntRestTemp { get; set; }

		/// <summary>
		/// Gets or sets the IntRestTempUnitId
		/// </summary>
		public TemperatureUnit? IntRestTempUnit { get; set; }

		/// <summary>
		/// Gets or sets the IntRestMinutes
		/// </summary>
		public int? IntRestMinutes { get; set; }

		/// <summary>
		/// Gets or sets the SacchRestTemp
		/// </summary>
		public double? SacchRestTemp { get; set; }

		/// <summary>
		/// Gets or sets the SacchRestTempUnit
		/// </summary>
		public TemperatureUnit? SacchRestTempUnit { get; set; }

		/// <summary>
		/// Gets or sets the SacchRestMinutes
		/// </summary>
		public int? SacchRestMinutes { get; set; }

		/// <summary>
		/// Gets or sets the MashOutTemp
		/// </summary>
		public double? MashOutTemp { get; set; }

		/// <summary>
		/// Gets or sets the MashOutTempUnit
		/// </summary>
		public TemperatureUnit? MashOutTempUnit { get; set; }

		/// <summary>
		/// Gets or sets the MashOutMinutes
		/// </summary>
		public int? MashOutMinutes { get; set; }

		/// <summary>
		/// Gets or sets the SpargeTemp
		/// </summary>
		public double? SpargeTemp { get; set; }

		/// <summary>
		/// Gets or sets the SpargeTempUnit
		/// </summary>
		public TemperatureUnit? SpargeTempUnit { get; set; }

		/// <summary>
		/// Gets or sets the SpargeMinutes
		/// </summary>
		public int? SpargeMinutes { get; set; }

		/// <summary>
		/// Gets or sets the PreBoilGravity
		/// </summary>
		public double? PreBoilGravity { get; set; }

		/// <summary>
		/// Gets or sets the PreBoilGravityTemp
		/// </summary>
		public double? PreBoilGravityTemp { get; set; }

		/// <summary>
		/// Gets or sets the PreBoilGravityTempUnit
		/// </summary>
		public TemperatureUnit? PreBoilGravityTempUnit { get; set; }

		/// <summary>
		/// Gets or sets the PostBoilGravity
		/// </summary>
		public double? PostBoilGravity { get; set; }

		/// <summary>
		/// Gets or sets the PostBoilGravityTemp
		/// </summary>
		public double? PostBoilGravityTemp { get; set; }

		/// <summary>
		/// Gets or sets the PostBoilGravityTempUnit
		/// </summary>
		public TemperatureUnit? PostBoilGravityTempUnit { get; set; }

		/// <summary>
		/// Gets or sets the WortCoolingMethod
		/// </summary>
		public WortCoolingMethod? WortCoolingMethod { get; set; }

		/// <summary>
		/// Gets or sets the MashBoilNotes
		/// </summary>
		public string MashBoilNotes { get; set; }

		/// <summary>
		/// Gets or sets the DateCreated
		/// </summary>
		public DateTime DateCreated { get; set; }

		/// <summary>
		/// Gets or sets the DateModified
		/// </summary>
		public DateTime? DateModified { get; set; }

		/// <summary>
		/// Determines if there is a Mash Schedule
		/// </summary>
		public bool HasMashSchedule()
		{
			return this.DoughInMinutes != null ||
				this.AcidRestMinutes != null ||
				this.ProteinRestMinutes != null ||
				this.IntRestMinutes != null ||
				this.SacchRestMinutes != null ||
				this.MashOutMinutes != null ||
				this.SpargeMinutes != null;
		}
	}
}