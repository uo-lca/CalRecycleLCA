using Entities.Models;
using LcaDataModel;
using Ninject;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services;

namespace CalRecycleLCA.Services
{
    public class TestGenericService : ITestGenericService//<T> : ITestGenericService<T> where T : class
    {
        //[Inject]
        //private readonly IService<T> _service;

        //public TestGenericService(IService<T> service) { _service = service; }
        [Inject]
        private readonly IService<LCIAMethod> _lciaMethodService;



        public TestGenericService(IService<LCIAMethod> lciaMethodService)
        {
            _lciaMethodService = lciaMethodService;
        }

        public IEnumerable<LCIAMethod> GetLCIAMethods()
        {
            var lciaMethods = _lciaMethodService.Query().Get().ToList();
            return lciaMethods;
        }
    }
}
