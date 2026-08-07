using System;

namespace Openbrew.Web.Core.Model
{
	public class DefaultRecipeUnitConverter : IRecipeUnitConverter
	{
		/// <summary>
		/// Converts pounds to kilograms
		/// </summary>
		public double ConvertPoundsToKilograms(double pounds)
		{
			return pounds * 0.453592;
		}

		/// <summary>
		/// Converts gallons to liters
		/// </summary>
		public double ConvertGallonsToLiters(double gallons)
		{
			return gallons * 3.785411784;
		}

		/// <summary>
		/// Converts Liters to Gallons
		/// </summary>
		public double ConvertLitersToGallons(double liters)
		{
			return liters * 0.264172;
		}

		/// <summary>
		/// Converts ounces to kilograms
		/// </summary>
		public double ConvertOuncesToKilograms(double ounces)
		{
			return ounces * 0.0283495;
		}

		/// <summary>
		/// Converts kilograms to ounces
		/// </summary>
		public double ConvertKilogramsToOunces(double kilograms)
		{
			return kilograms * 35.274;
		}

		/// <summary>
		/// Converts Tbsps to Kilograms
		/// </summary>
		public double ConvertTbspToKilograms(double tbsps)
		{
			return tbsps * 0.0141748;
		}

		/// <summary>
		/// Converts Tsp to Kilograms
		/// </summary>
		public double ConvertTspToKilograms(double tsps)
		{
			return tsps * 0.00472493;
		}

		/// <summary>
		/// Converts fluid ounces to millileters
		/// </summary>
		public double ConvertFluidOuncesToLiters(double amount)
		{
			return amount * 0.0295735;
		}

		/// <summary>
		/// Converts pints to liters
		/// </summary>
		public double ConvertPintsToLiters(double amount)
		{
			return amount * 0.473176;
		}

		/// <summary>
		/// Converts quarts to liters
		/// </summary>
		public double ConvertQuartsToLiters(double amount)
		{
			return amount * 0.946353;
		}

		/// <summary>
		/// Comverts Tbsps to millileters
		/// </summary>
		public double ConvertTbspToLiters(double amount)
		{
			return amount * 0.0147868;
		}

		/// <summary>
		/// Converts tsps to millileters
		/// </summary>
		public double ConvertTspToLiters(double amount)
		{
			return amount * 0.00492892;
		}

		/// <summary>
		/// Converts liters to fluid ounces
		/// </summary>
		public double ConvertLitersToFluidOunces(double amount)
		{
			return amount * 33.814;
		}

		/// <summary>
		/// Converts kilograms to pounds
		/// </summary>
		public double ConvertKilogramsToPounds(double amount)
		{
			return amount * 2.20462;
		}

		/// <summary>
		/// Converts grams to kilograms
		/// </summary>
		public object ConvertGramsToKilograms(double amount)
		{
			return amount / 1000;
		}
	}
}