using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Repository;

namespace LcaDataModel {
    [Table("CompositionSubstitution")]
    public partial class CompositionSubstitution : Entity
    {
        [Key]
        [Column(Order = 1)]
        public int ProcessCompositionID { get; set; }

        [Key]
        [Column(Order = 2)]
        public int ScenarioID { get; set; }

        public int CompositionModelID { get; set; }

        // Navigation Properties
        public virtual ProcessComposition ProcessComposition { get; set; }
        public virtual Scenario Scenario { get; set; }
        public virtual CompositionModel CompositionModel { get; set; }
    }
}
