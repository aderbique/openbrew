using System;
using System.Linq;
using ctorx.Core.Validation;
using Openbrew.Web.Validators;

namespace Openbrew.Web.Models
{
	public class SignUpViewModel : ValidatesWith<SignUpViewModelValidator>
	{
		/// <summary>
		/// Gets or sets the Name
		/// </summary>
		public string NewUserFullName { get; set; }

		/// <summary>
		/// Gets or sets the EmailAddress
		/// </summary>
		public string NewUserEmailAddress { get; set; }

		/// <summary>
		/// Gets or sets the Password
		/// </summary>
		public string NewUserPassword { get; set; }

		/// <summary>
		/// Gets the First Name
		/// </summary>
		public string GetFirstName()
		{
			// TODO: Make this safer
			var parts = this.NewUserFullName.Trim().Replace("  ", " ").Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
			return parts[0];
		}

		/// <summary>
		/// Gets the Last Name
		/// </summary>
		public string GetLastName()
		{
			// TODO: Make this safer
			var parts = this.NewUserFullName.Trim().Replace("  ", " ").Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
			return parts[1];
		}
	}
}