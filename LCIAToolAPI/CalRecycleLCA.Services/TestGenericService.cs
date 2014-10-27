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
        //[Inject]
        //private readonly IProcessFlowService _processFlowService;
        [Inject]
        private readonly IFragmentNodeFragmentService _fragmentNodeFragmentService;


        public TestGenericService(
            ILCIAMethodService lciaMethodService,
            IFragmentFlowService fragmentFlowService,
            IFragmentNodeProcessService fragmentNodeProcessService,
            IFragmentNodeFragmentService fragmentNodeFragmentService)
        {
            _lciaMethodService = lciaMethodService;
            _fragmentFlowService = fragmentFlowService;
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
            var ff = _fragmentFlowService.Query(x => x.FragmentFlowID == fragmentFlowID).Select().FirstOrDefault();
            FragmentNodeResource fnr;
            switch (ff.NodeTypeID)
            {
                case 1:
                    {
                        fnr = _fragmentNodeProcessService.GetFragmentNodeProcessId(fragmentFlowID, scenarioID);
                        break;
                    }
                case 2:
                    {
                        fnr = _fragmentNodeFragmentService.GetFragmentNodeSubFragmentId(fragmentFlowID);
                        break;
                    }
                default:
                    {
                        fnr = new FragmentNodeResource { };
                        break;
                    }
            }
            return fnr;

        }
    }
}
