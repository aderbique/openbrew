using System;
using Openbrew.Web.Core.Model;

namespace Openbrew.Web.Models
{
    public class RecipeMashStepViewModel : RecipeIngredientViewModel
    {
        /// <summary>
        /// Gets or sets the Heat
        /// </summary>
        public string Heat { get; set; }

        /// <summary>
        /// Gets or sets the Temp
        /// </summary>
        public string Temp { get; set; }

        /// <summary>
        /// Gets or sets the Time
        /// </summary>
        public string Time { get; set; }
    }
}