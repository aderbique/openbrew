using System;
using System.Linq;
using ctorx.Core.Validation;
using Openbrew.Web.Validators;

namespace Openbrew.Web.Models
{
	public class ChangePasswordViewModel : ValidatesWith<ChangePasswordViewModelValidator>
	{
		/// <summary>
		/// Gets or sets the CurrentPassword
		/// </summary>
		public string CurrentPassword { get; set; }

		/// <summary>
		/// Gets or sets the NewPassword
		/// </summary>
		public string NewPassword { get; set; }

		/// <summary>
		/// Gets or sets the ConfirmNewPassword
		/// </summary>
		public string ConfirmNewPassword { get; set; }
	}
}