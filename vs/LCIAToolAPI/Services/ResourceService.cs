using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LcaDataModel;
using Entities.Models;
using Ninject;

namespace Services {
    /// <summary>
    /// Service providing access to resource data
    /// Transforms LcaDataModel entities to Resource entities
    /// </summary>
    public class ResourceService : IResourceService {
        [Inject]
        private readonly IService<LCIAMethod> _LciaMethodService;

        /// <summary>
        /// Constructor for use with Ninject dependency injection
        /// </summary>
        /// <param name="lciaMethodService"></param>
        public ResourceService(IService<LCIAMethod> lciaMethodService) {
            if (lciaMethodService == null) {
                throw new ArgumentNullException("lciaMethodService");
            }
            _LciaMethodService = lciaMethodService;
        }

        /// <summary>
        /// Transform an LCIAMethod object to a LCIAMethodResource object
        /// </summary>
        /// <param name="lm"></param>
        /// <returns></returns>
        public LCIAMethodResource Transform(LCIAMethod lm) {
            return new LCIAMethodResource {
                LCIAMethodID = lm.LCIAMethodID,
                Name = lm.Name,
                Methodology = lm.Methodology,
                ImpactCategoryID = Convert.ToInt32(lm.ImpactCategoryID),
                ImpactIndicator = lm.ImpactIndicator,
                ReferenceYear = lm.ReferenceYear,
                Duration = lm.Duration,
                ImpactLocation = lm.ImpactLocation,
                IndicatorTypeID = Convert.ToInt32(lm.IndicatorTypeID),
                Normalization = Convert.ToBoolean(lm.Normalization),
                Weighting = Convert.ToBoolean(lm.Weighting),
                UseAdvice = lm.UseAdvice,
                FlowPropertyID = Convert.ToInt32(lm.ReferenceQuantity)
            };
        }

        public IEnumerable<LCIAMethodResource> GetLCIAMethodResources() {
            IEnumerable<LCIAMethod> lciaMethods = _LciaMethodService.Query().Get();
            return lciaMethods.Select(lm => Transform(lm)).ToList();
        }

    }
}
