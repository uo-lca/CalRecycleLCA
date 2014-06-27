using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Mappings;

namespace Services
{
    public interface IFragmentService : IService<Fragment>
    {
        IEnumerable<Fragment> GetFragments();
    }
}
