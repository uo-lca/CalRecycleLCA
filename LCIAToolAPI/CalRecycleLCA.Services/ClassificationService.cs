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
    public interface IClassificationService : IService<Classification>
    {
    }

    public class ClassificationService : Service<Classification>, IClassificationService
    {
        public ClassificationService(IRepositoryAsync<Classification> repository)
            : base(repository)
        {

        }
    }
}

