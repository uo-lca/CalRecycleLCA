using Entities.Models;
using LcaDataModel;
using Ninject;
//using Repository;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalRecycleLCA.Services
{
    public class TestGenericService : ITestGenericService//<T> : ITestGenericService<T> where T : class
    {
        //[Inject]
        //private readonly IService<T> _service;

        //public TestGenericService(IService<T> service) { _service = service; }
        [Inject]
        private readonly ILCIAMethodService _lciaMethodService;
        [Inject]
        private readonly IFragmentFlowService _fragmentFlowService;
        [Inject]
        private readonly IFragmentNodeProcessService _fragmentNodeProcessService;
        [Inject]
        private readonly IProcessFlowService _processFlowService;
        [Inject]
        private readonly IFragmentNodeFragmentService _fragmentNodeFragmentService;


        public TestGenericService(
            ILCIAMethodService lciaMethodService,
            IFragmentFlowService fragmentFlowService,
            IProcessFlowService processFlowService,
            IFragmentNodeProcessService fragmentNodeProcessService,
            IFragmentNodeFragmentService fragmentNodeFragmentService)
        {
            _lciaMethodService = lciaMethodService;
            _fragmentFlowService = fragmentFlowService;
            _processFlowService = processFlowService;
            _fragmentNodeProcessService = fragmentNodeProcessService;
            _fragmentNodeFragmentService = fragmentNodeFragmentService;
        }

        public IEnumerable<LCIAMethod> GetLCIAMethods()
        {
            var lciaMethods = _lciaMethodService.Queryable().ToList();
            return lciaMethods;
        }

        public FragmentNodeResource FindTerminus(int fragmentFlowID, int scenarioID)
        {
            var ff = _fragmentFlowService.GetFragmentFlow(fragmentFlowID);
            var fnr = _fragmentFlowService.Terminate(ff, scenarioID, true);
            return fnr;
        }

        public IEnumerable<InventoryModel> GetDependencies(int fragmentFlowId, int scenarioId)
        {
            var inv = new List<InventoryModel>();
            var ff = _fragmentFlowService.GetFragmentFlow(fragmentFlowId);
            var fnr = _fragmentFlowService.Terminate(ff, scenarioId, true);
            switch (fnr.NodeTypeID)
            {
                case 1:
                    {
                        inv = _processFlowService.GetDependencies((int)fnr.ProcessID,fnr.TermFlowID,ff.DirectionID)
                            .ToList();
                        break;
                    }
                case 2:
                    {
                        double foo = 1.0;
                        inv = _fragmentFlowService.GetDependencies((int)fnr.SubFragmentID, fnr.TermFlowID, ff.DirectionID,
                            out foo, fnr.ScenarioID).ToList();
                        break;
                    }
            }
            return inv;
        }
    }
}
