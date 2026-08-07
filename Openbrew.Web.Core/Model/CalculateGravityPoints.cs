using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Openbrew.Web.Core.Model
{
    public static class CalculateGravityPoints
    {
        public static double Calculate(double amountInOunces, int Ppg, double mashLauterEfficiency, bool addedToMash, double BatchSize)
        {
            if (!addedToMash)
                mashLauterEfficiency = 1.00f;

            return (ConvertOuncesToPounds.Convert(amountInOunces) * Ppg * mashLauterEfficiency / BatchSize);
        }
    }
}
