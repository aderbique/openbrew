using System;
using Openbrew.Web.Core.Model;
using ctorx.Core.Validation;

namespace Openbrew.Web.Models
{
	public class BrewSessionFermentationViewModel : ValidatesWith<BrewSessionFermentationViewModelValidator>
	{
		/// <summary>
		/// Gets or sets the BrewSessionId
		/// </summary>
		public int BrewSessionId { get; set; }

		/// <summary>
		/// Gets or sets the PitchingTemp
		/// </summary>
		public double PitchingTemp { get; set; }

		/// <summary>
		/// Gets or sets the PitchingTempUnit
		/// </summary>
		public TemperatureUnit PitchingTempUnit { get; set; }

		/// <summary>
		/// Gets or sets the YeastStarter
		/// </summary>
		public string YeastStarter { get; set; }

		/// <summary>
		/// Gets or sets the RackToSecondaryDate
		/// </summary>
		public DateTime? RackToSecondaryDate { get; set; }

		/// <summary>
		/// Gets or sets the AverageFermentationTemp
		/// </summary>
		public double? AverageFermentationTemp { get; set; }

		/// <summary>
		/// Gets or sets the AverageFermentationTempUnit
		/// </summary>
		public TemperatureUnit? AverageFermentationTempUnit { get; set; }

		/// <summary>
		/// Gets or sets the FinalGravity
		/// </summary>
		public double? FinalGravity { get; set; }

		/// <summary>
		/// Gets or sets the FinalGravityTemp
		/// </summary>
		public double? FinalGravityTemp { get; set; }

		/// <summary>
		/// Gets or sets the FinalGravityTempUnit
		/// </summary>
		public TemperatureUnit? FinalGravityTempUnit { get; set; }

		/// <summary>
		/// Gets or sets the FermentationNotes
		/// </summary>
		public string FermentationNotes { get; set; }

		/// <summary>
		/// Gets or sets the DateCreated
		/// </summary>
		public DateTime DateCreated { get; set; }

		/// <summary>
		/// Gets or sets the DateModified
		/// </summary>
		public DateTime? DateModified { get; set; }

	}
}