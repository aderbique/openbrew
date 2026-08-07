using System;
using System.Configuration;
using System.Linq;
using System.Security;
using System.Web.Mvc;
using Openbrew.Web.Email;
using ctorx.Core.Crypto;
using ctorx.Core.Data;
using ctorx.Core.Email;
using ctorx.Core.Messaging;
using ctorx.Core.Security;
using Openbrew.Web.Core.Configuration;
using Openbrew.Web.Core.Data;
using Openbrew.Web.Core.Model;
using Openbrew.Web.Core.Service;
using Openbrew.Web.Models;
using ctorx.Core.Web;
using System.Collections.Generic;

namespace Openbrew.Web.Controllers
{
    public class ContentController : BrewgrController
	{
		/// <summary>
        /// Executes the View for Calculations
		/// </summary>
		public ViewResult Calculations()
		{
			return View();
		}

        /// <summary>
        /// Executes the View for Calculations/OriginalGravity
        /// </summary>
        public ViewResult CalculationsOriginalGravity()
        {
            return View();
        }

        /// <summary>
        /// Executes the View for Calculations/FinalGravity
        /// </summary>
        public ViewResult CalculationsFinalGravity()
        {
            return View();
        }

        /// <summary>
        /// Executes the View for Calculations/SRM
        /// </summary>
        public ViewResult CalculationsSRM()
        {
            return View();
        }

        /// <summary>
        /// Executes the View for Calculations/IBU
        /// </summary>
        public ViewResult CalculationsIBU()
        {
            return View();
        }

        /// <summary>
        /// Executes the View for Calculations/Alcohol
        /// </summary>
        public ViewResult CalculationsAlcohol()
        {
            return View();
        }

        /// <summary>
        /// Executes the View for Calculations/Calories
        /// </summary>
        public ViewResult CalculationsCalories()
        {
            return View();
        }

        /// <summary>
        /// Executes the View for Calculators
        /// </summary>
        public ViewResult Calculators()
        {
            return View();
        }

        /// <summary>
        /// Executes the View for CalculatorsHydrometerTemp
        /// </summary>
        public ViewResult CalculatorsHydrometerTemp()
        {
            // Temperature Units
            ViewBag.TempUnitOptions = new List<OptionViewModel>
			{
				new OptionViewModel { Value = "Fahrenheit", DisplayText = "F" },
				new OptionViewModel { Value = "Celcius", DisplayText = "C" }
			};

            // Mash Time
            ViewBag.MashMinutesOptions = new List<OptionViewModel> { new OptionViewModel { Value = null, DisplayText = null } };
            for (var i = 20; i <= 120; i += 5)
            {
                ViewBag.MashMinutesOptions.Add(new OptionViewModel { Value = i.ToString(), DisplayText = i.ToString() + " Minute(s)" });
            }

            // Gravity Options
            ViewBag.GravityOptions = new List<OptionViewModel> { new OptionViewModel { Value = null, DisplayText = null } };
            for (var i = 1.000; i <= 1.150; i += .001)
            {
                ViewBag.GravityOptions.Add(new OptionViewModel { Value = i.ToString(), DisplayText = i.ToString("0.000") });
            }

            return View(new CalculatorHydrometerTempViewModel { SpecificGravity = 1.04999999999999, TargetSpecificGravityTemp = 60 });
        }

        /// <summary>
        /// Executes the View for CalculatorsMashSpargeWater
        /// </summary>
        public ViewResult CalculatorsMashSpargeWater()
        {
	        return this.View(new BrewSessionViewModel());
        }

    }
}
