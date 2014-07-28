using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LcaDataModel;
using Entities.Models;

namespace Services
{
    public interface IFragmentService : IService<Fragment>
    {
        IEnumerable<FragmentModel> GetFragments();

        FragmentModel GetFragment(int id);
    }
}
