using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LcaDataModel {
    [Table("ProcessComposition")]
    public partial class ProcessComposition : Entity
    {

        // public ProcessComposition()
        // {
        //     CompositionSubstitutions = new HashSet<CompositionSubstitution>();
        // }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProcessCompositionID { get; set; }

        public int CompositionModelID { get; set; }

        public int ProcessID { get; set; }

        public virtual CompositionModel CompositionModel { get; set; }

        public virtual Process Process { get; set; }

        // public virtual ICollection<CompositionSubstitution> CompositionSubstitutions { get; set; }

    }
}
