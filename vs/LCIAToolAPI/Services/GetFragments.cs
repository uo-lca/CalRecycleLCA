using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Mappings;
using Repository;
using Entities.Models;

namespace Services
{
    public class GetFragments
    {




        public IEnumerable<Fragment> GetFragments1()
        {
            var unitOfWork = new UnitOfWork();
            var fragments =
       unitOfWork.Repository<Fragment>()
            .Query()
            //.Include(i => i.BackgroundFragments)
            .OrderBy(q => q
                .OrderBy(c => c.FragmentID)
                .ThenBy(c => c.FragmentID))
            .Filter(q => q.Name != null)
            .GetPage();

            return fragments;
        }


        //public static IEnumerable<BackgroundFragment> AddCustomerWithAddressValidation1(this IRepository<BackgroundFragment> backgroundFragmentRepository)
        //{
        //    //var unitOfWork = new UnitOfWork();
        //    Fragment fragment = new Fragment();
        //    int totalCustomerCount;
        //    BackgroundFragment a = new BackgroundFragment();
        //    a.BackgroundFragmentID = fragment.FragmentID;

        //    var fragments = backgroundFragmentRepository.Query()
        //        .Include(i => i.Fragment)
        //    .OrderBy(q => q
        //        .OrderBy(c => c.FragmentID)
        //        .ThenBy(c => c.FragmentID))
        //    .Filter(q => q.Fragment.Name != null)
        //    .GetPage(1, 10, out totalCustomerCount);
        //    return fragments;
        //}
    }
}
