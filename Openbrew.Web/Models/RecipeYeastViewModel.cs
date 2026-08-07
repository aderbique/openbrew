using System;
using ctorx.Core.Conversion;

namespace Openbrew.Web.Models
{
	public class RecipeYeastViewModel : RecipeIngredientViewModel
	{
		/// <summary>
		/// Gets or sets the Attenuation
		/// </summary>
		public string Atten { get; set; }

		/// <summary>
		/// Gets the attenuation percent
		/// </summary>
		public double GetAttenuationPercent()
		{
			return Converter.Convert<double>(this.Atten) * 100.00;
		}
	}
}