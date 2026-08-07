using System.Collections.Generic;
using Openbrew.Web.Core.Model;

namespace Openbrew.Web.Models
{
    public class Ba2026StyleDetailViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IList<string> TastingNotes { get; set; }
        public string Bitterness { get; set; }
        public string Color { get; set; }
        public string OriginalGravity { get; set; }
        public string FinalGravity { get; set; }
        public string Alcohol { get; set; }
        public string VisualCue { get; set; }
        public string BalanceCue { get; set; }
        public string BodyCue { get; set; }
        public string MaltCue { get; set; }
        public string HopCue { get; set; }
        public string FermentationCue { get; set; }
        public IList<Recipe> Recipes { get; set; }
    }
}
