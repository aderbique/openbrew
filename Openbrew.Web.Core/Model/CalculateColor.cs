using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Openbrew.Web.Core.Model
{
    public class CalculateColor
    {
        public static double Calculate(double amountInOunces, int Ppg, int lovibond, double BatchSize)
        {
            return 1.49d * (double)Math.Pow(((ConvertOuncesToPounds.Convert(amountInOunces) * lovibond) / BatchSize), 0.69d);
        }
    }
}
