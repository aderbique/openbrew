using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Openbrew.Web.Core.Model
{
    public static class CalculateFinalGravity
    {
        public static float Calculate(float totalGravityPoints, float attenuationPercent)
        {
            return 1 + ((totalGravityPoints * (1 - attenuationPercent)) / 1000);
        }
    }
}
