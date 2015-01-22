using LcaDataModel;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalRecycleLCA.Services
{
    public interface ILCIAMethodService : IService<LCIAMethod>
    {
        List<int> QueryActiveMethods();
        IEnumerable<LCIAMethod> FetchActiveMethods();
    }

    public class LCIAMethodService : Service<LCIAMethod>, ILCIAMethodService
    {
        private IRepositoryAsync<LCIAMethod> _repository;

        private List<int> activeMethods = new List<int> { 
            2,  //	ILCD2011; Climate change; midpoint; GWP100; IPCC2007
            28, //	ILCD2011; Acidification terrestrial and freshwater; midpoint; Accumulated Exceedance; Seppälä et al. 2006, Posch et al. 2008
            20, //	ILCD2011; Ecotoxicity freshwater; midpoint; CTUe; USEtox
            5, //	ILCD2011; Eutrophication marine; midpoint; N equivalents; ReCiPe
            16, //	ILCD2011; Eutrophication freshwater; midpoint; P equivalents; ReCiPe
            3, //	ILCD2011; Eutrophication terrestrial; midpoint; Accumulated Exceedance; Seppala et al 2006, Posch et al 2008
            23, //	ILCD2011; Cancer human health effects; midpoint; CTUh; USEtox
            1, //	ILCD2011; Non-cancer human health effects; midpoint; CTUh; USEtox
            8, //	ILCD2011; Respiratory inorganics; midpoint; PM2.5eq; Rabl and Spadaro 2004-Greco et al 2007
            12, //	ILCD2011; Ozone depletion; midpoint; ODP; WMO1999
            25, //	ILCD2011; Photochemical ozone formation; midpoint - human health; POCP; Van Zelm et al. (2008)
            21 //	ILCD2011; Resource depletion- mineral, fossils and renewables; midpoint;abiotic resource depletion; Van Oers et al. 2002
        };

        public LCIAMethodService(IRepositoryAsync<LCIAMethod> repository)
            : base(repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Return only the LCIA methods selected here for inclusion-- others will still be available 
        /// by direct request.
        /// </summary>
        /// <returns></returns>
        public List<int> QueryActiveMethods()
        {
            return _repository.Queryable().Where(k => activeMethods.Contains(k.LCIAMethodID))
                .Select(k => k.LCIAMethodID).ToList();
        }
        /// <summary>
        /// Just like QueryActiveMethods except it pre-fetches the needed data for the 
        /// API resource. 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<LCIAMethod> FetchActiveMethods()
        {
            return _repository.Query(k => activeMethods.Contains(k.LCIAMethodID))
                                                .Include(x => x.IndicatorType)
                                                .Include(x => x.ILCDEntity)
                                                .Select();
        }
    }
}

