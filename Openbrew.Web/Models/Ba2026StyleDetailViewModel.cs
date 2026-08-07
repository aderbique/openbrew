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
        public IList<BaStyleGaugeMetric> GaugeMetrics { get; set; }
        public IList<Recipe> Recipes { get; set; }
    }

    public class BaStyleGaugeMetric
    {
        public string Label { get; set; }
        public string Unit { get; set; }
        public string LowLabel { get; set; }
        public string HighLabel { get; set; }
        public string RangeLabel { get; set; }
        public double Low { get; set; }
        public double High { get; set; }
        public double StartPercent { get; set; }
        public double WidthPercent { get; set; }
    }
}
