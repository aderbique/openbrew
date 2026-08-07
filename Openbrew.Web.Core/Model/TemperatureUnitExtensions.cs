using System;

namespace Openbrew.Web.Core.Model
{
	public static class TemperatureUnitExtensions
	{
		/// <summary>
		/// Gets the abbreviated string version of the temperature unit
		/// </summary>
		public static string GetAbbreviation(this TemperatureUnit temperatureUnit)
		{
			return temperatureUnit == TemperatureUnit.Fahrenheit ? "F" : "C";
		}

	}
}