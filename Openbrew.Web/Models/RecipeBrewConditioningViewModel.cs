using System;
using Openbrew.Web.Core.Model;
using ctorx.Core.Validation;

namespace Openbrew.Web.Models
{
	public class BrewSessionConditioningViewModel : ValidatesWith<BrewSessionConditioningViewModelValidator>
	{
		/// <summary>
		/// Gets or sets the BrewSessionId
		/// </summary>
		public int BrewSessionId { get; set; }

		/// <summary>
		/// Gets or sets the ConditionDate
		/// </summary>
		public DateTime ConditionDate { get; set; }

		/// <summary>
		/// Gets or sets the ConditionType
		/// </summary>
		public ConditionType ConditionType { get; set; }

		/// <summary>
		/// Gets or sets the ConditionLength
		/// </summary>
		public int ConditionLength { get; set; }

		/// <summary>
		/// Gets or sets the ConditionNotes
		/// </summary>
		public string ConditioningNotes { get; set; }

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