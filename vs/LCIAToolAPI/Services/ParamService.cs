using Data;
using Ninject;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ParamService : Service<Param>, IParamService
    {
         [Inject]
        private readonly IRepository<Param> _repository;


        public ParamService(IRepository<Param> repository)
            : base(repository)
        {
            _repository = repository;
        }
    }
}
