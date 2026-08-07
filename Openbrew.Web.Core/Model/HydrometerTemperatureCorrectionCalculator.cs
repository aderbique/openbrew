using System;
using System.Linq;

namespace Openbrew.Web.Core.Model
{
	public class HydrometerTemperatureCorrectionCalculator
	{
		/// <summary>
		/// Calculates the temperature adjusted hydrometer reasing 
		/// </summary>
		public double Calculate(double measuredGravity, double measuredTemperature, double targetTemperature = 60.00)
		{
			// Taken from: http://homebrew.stackexchange.com/questions/4137/temperature-correction-for-specific-gravity

			return measuredGravity * ((1.00130346 - 0.000134722124 * measuredTemperature + 0.00000204052596 * Math.Pow(measuredTemperature, 2) - 0.00000000232820948 * Math.Pow(measuredTemperature, 3))
				/ (1.00130346 - 0.000134722124 * targetTemperature + 0.00000204052596 * Math.Pow(targetTemperature, 2) - 0.00000000232820948 * Math.Pow(targetTemperature, 3)));
		}
	}
}