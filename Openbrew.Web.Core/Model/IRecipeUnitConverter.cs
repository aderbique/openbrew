namespace Openbrew.Web.Core.Model
{
	public interface IRecipeUnitConverter
	{
		/// <summary>
		/// Converts pounds to kilograms
		/// </summary>
		double ConvertPoundsToKilograms(double pounds);

		/// <summary>
		/// Converts Gallons to Liters
		/// </summary>
		double ConvertGallonsToLiters(double gallons);

		/// <summary>
		/// Converts Liters to Gallons
		/// </summary>
		double ConvertLitersToGallons(double liters);

		/// <summary>
		/// Converts Ounces to Kilograms
		/// </summary>
		double ConvertOuncesToKilograms(double ounces);

		/// <summary>
		/// Converts kilograms to ounces
		/// </summary>
		double ConvertKilogramsToOunces(double kilograms);

		/// <summary>
		/// Converts Tbsps to Kilograms
		/// </summary>
		double ConvertTbspToKilograms(double tbsps);

		/// <summary>
		/// Converts Tsp to Kilograms
		/// </summary>
		double ConvertTspToKilograms(double tsps);

		/// <summary>
		/// Converts fluid ounces to millileters
		/// </summary>
		double ConvertFluidOuncesToLiters(double amount);

		/// <summary>
		/// Converts pints to liters
		/// </summary>
		double ConvertPintsToLiters(double amount);

		/// <summary>
		/// Converts quarts to liters
		/// </summary>
		double ConvertQuartsToLiters(double amount);

		/// <summary>
		/// Comverts Tbsps to millileters
		/// </summary>
		double ConvertTbspToLiters(double amount);

		/// <summary>
		/// Converts tsps to millileters
		/// </summary>
		double ConvertTspToLiters(double amount);

		/// <summary>
		/// Converts liters to fluid ounces
		/// </summary>
		double ConvertLitersToFluidOunces(double amount);

		/// <summary>
		/// Converts kilograms to pounds
		/// </summary>
		double ConvertKilogramsToPounds(double amount);

		/// <summary>
		/// Converts grams to kilograms
		/// </summary>
		object ConvertGramsToKilograms(double amount);
	}
}