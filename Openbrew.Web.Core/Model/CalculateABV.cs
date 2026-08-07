using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Openbrew.Web.Core.Model
{
    public static class CalculateABV
    {
        public static float Calculate(float totalGravityPoints, float attenuationPercent)
        {
            return (totalGravityPoints - (totalGravityPoints * (1 - attenuationPercent))) * 0.129f;
        }
    }
}
