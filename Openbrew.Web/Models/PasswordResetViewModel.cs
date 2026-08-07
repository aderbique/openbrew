using System;
using System.Linq;
using ctorx.Core.Validation;
using Openbrew.Web.Validators;

namespace Openbrew.Web.Models
{
	public class PasswordResetViewModel : ValidatesWith<PasswordResetViewModelValidator>
	{
		/// <summary>
		/// Gets or sets the EmailAddress
		/// </summary>
		public string EmailAddress { get; set; }
	}
}