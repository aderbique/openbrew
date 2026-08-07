using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.WebPages.Html;
using Openbrew.Web.Core.Model;
using System.Web.Script.Serialization;

namespace Openbrew.Web.Models
{
	public class BrewSessionViewModel : BrewSession
	{
		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public BrewSessionViewModel()
		{
			this.SetDefaultEquipmentProfile();	
		}

		/// <summary>
		/// Gets or sets the BrewedByUsername
		/// </summary>
		public string BrewedByUsername { get; set; }

		/// <summary>
		/// Gets or sets the RecipeSummary
		/// </summary>
		public new RecipeSummaryViewModel RecipeSummary { get; set; }

        /// <summary>
        /// Gets or sets the Comments
        /// </summary>
        [ScriptIgnore]
        public CommentWrapperViewModel CommentWrapperViewModel { get; set; }

		/// <summary>
		/// Determines if the brew session is a new session
		/// </summary>
		public bool IsNewBrewSession()
		{
			return this.BrewSessionId == 0;
		}

		/// <summary>
		/// Gets Notes by Line Break
		/// </summary>
		public IList<string> NotesByLineBreak()
		{
			return this.Notes.Replace(Environment.NewLine, "|<br />|").Split('|').ToList();
		}

		/// <summary>
		/// Gets the appropriate unit label
		/// </summary>
		public string UnitLabel(string standard, string metric)
		{
			return this.UnitTypeId == (int)UnitType.USStandard ? standard : metric;
		}

		/// <summary>
		/// Shows a value or "--" if the value is not available
		/// </summary>
		public string ShowValue(object value, string output)
		{
			return (value == null || (value is string && (string.IsNullOrWhiteSpace((string)value)) )) ? "not specified" : output;
		}

		/// <summary>
		/// Sets the default equipment profile
		/// </summary>
		public void SetDefaultEquipmentProfile()
		{
			this.UnitTypeId = (int)UnitType.USStandard;
			this.BrewKettleLoss = 0.50;
			this.WortShrinkage = 0.20;
			this.MashTunLoss = 0.25;
			this.BoilLoss = 1.00;
			this.MashGrainAbsorption = 0.15;
			this.SpargeGrainAbsorption = 0.01;
		}
	}
}
