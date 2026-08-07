using System;
using System.Linq;

namespace Openbrew.Web.Models
{
	public class RecipeStepViewModel
	{
		/// <summary>
		/// Gets or sets the Id
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the Rank
		/// </summary>
		public string Rank { get; set; }

		/// <summary>
		/// Gets or sets the Description
		/// </summary>
		public string Text { get; set; }
	}
}