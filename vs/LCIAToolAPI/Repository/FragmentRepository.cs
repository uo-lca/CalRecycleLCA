using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;

namespace Repository
{
    public static class FragmentRepository
    {

        public static IEnumerable<Fragment> GetFragments(this IRepository<Fragment> fragmentRepository)
        {
            var fragments =
       fragmentRepository
            .Query()
            .Include(i => i.FragmentFlow)
            .OrderBy(q => q
                .OrderBy(c => c.FragmentFlow.FragmentStageID)
                .ThenBy(c => c.FragmentID))
            .Filter(q => q.Name != null)
            .GetPage();

            return fragments;
        }

       
    }
}
