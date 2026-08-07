using System;
using System.Collections.Generic;
using System.Linq;

namespace Openbrew.Web.Core.Model
{
    public class Equipment
    {
        /// <summary>
        /// Gets or sets the EquipmentId
        /// </summary>
        public int EquipmentId { get; set; }

        /// <summary>
        /// Gets or sets the BrewSessionId
        /// </summary>
        public int BrewSessionId { get; set; }

        /// <summary>
        /// Gets or sets the BrewSession
        /// </summary>
        public BrewSession BrewSession { get; set; }

        /// <summary>
        /// Gets or sets the BrewKettleTrubLoss
        /// </summary>
        public double? BrewKettleTrubLoss { get; set; }

        /// <summary>
        /// Gets or sets the WortShrinkage
        /// </summary>
        public double? WortShrinkage { get; set; }

        /// <summary>
        /// Gets or sets the MashTunEquipmentLoss
        /// </summary>
        public double? MashTunEquipmentLoss { get; set; }

        /// <summary>
        /// Gets or sets the BoilLoss
        /// </summary>
        public double? BoilLoss { get; set; }

        /// <summary>
        /// Gets or sets the GrainAbsorption
        /// </summary>
        public double? GrainAbsorption { get; set; }

        /// <summary>
        /// Gets or sets the SpargeGrainAbsorption
        /// </summary>
        public double? SpargeGrainAbsorption { get; set; }

        /// <summary>
        /// Gets or sets the User
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Gets or sets the DateCreated
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the DateModified
        /// </summary>
        public DateTime? DateModified { get; set; }

    }
}