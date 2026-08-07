using System;
using System.Linq;
using Openbrew.Web.Validators;
using ctorx.Core.Validation;

namespace Openbrew.Web.Models
{
	public class ContactViewModel : ValidatesWith<ContactViewModelValidator>
	{
		/// <summary>
		/// Gets or sets the Name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the EmailAddress
		/// </summary>
		public string EmailAddress { get; set; }

		/// <summary>
		/// Gets or sets the MessageContent
		/// </summary>
		public string MessageContent { get; set; }
	}
}