using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Openbrew.Web.Core.Model
{
    public static class CalculateOriginalGravity
    {
        public static float Calculate(float totalGravityPoints, float mashLauterEfficiency, float BoilSize)
        {
            //return 1 + (fermentables.Sum(x => CalculateGravityPoints.Calculate(x.AmountInOunces, x.Ppg, mashLauterEfficiency, x.AddedToMash, BoilSize) / 1000));
            return 1 + (totalGravityPoints / 1000);
        }
    }
}
