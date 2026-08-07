using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Openbrew.Web.Models
{
    public class BJCPStyleJSONModel
    {
        public string SubCategoryID { get; set; }
        public string SubCategoryName { get; set; }
        public double? og_low { get; set; }
        public double? og_high { get; set; }
        public double? fg_low { get; set; }
        public double? fg_high { get; set; }
        public int? ibu_low { get; set; }
        public int? ibu_high { get; set; }
        public double? srm_low { get; set; }
        public double? srm_high { get; set; }
        public double? abv_low { get; set; }
        public double? abv_high { get; set; }
    }
}