using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LcaDataModel;
using Entities.Models;
using Repository;
using Ninject;

namespace Services
{
    public class FragmentService : Service<Fragment>, IFragmentService
    {
        [Inject]
        private readonly IRepository<Fragment> _repository;


        public FragmentService(IRepository<Fragment> repository)
            : base(repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Get Fragment data and transform to API model
        /// </summary>
        /// <returns>List of FragmentModel objects</returns>
        public IEnumerable<FragmentModel> GetFragments()
        {
            IEnumerable<Fragment> fragments = _repository.GetFragments();
            return fragments.Select(f => new FragmentModel {
                FragmentID = f.FragmentID,
                Name = f.Name,
                ReferenceFragmentFlowID = f.ReferenceFragmentFlowID
            });
        }

        /// <summary>
        /// Get a Fragment  and transform to API model
        /// </summary>
        /// <param name="fragmentID">FragmentID</param>
        /// <returns>FragmentModel</returns>
        public FragmentModel GetFragment(int id) {
            Fragment fragment = _repository.GetFragment(id);
            if (fragment == null) {
                // TODO : error handling for ID not found
                return null;
            }
            else {
                return new FragmentModel {
                    FragmentID = fragment.FragmentID,
                    Name = fragment.Name,
                    ReferenceFragmentFlowID = fragment.ReferenceFragmentFlowID
                };
            }
        }
    }
}
