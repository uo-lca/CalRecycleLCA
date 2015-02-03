using LcaDataModel;
using Repository.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Pattern.Repositories;
using Entities.Models;

namespace CalRecycleLCA.Repositories
{
    public static class FlowRepository
    {
        /// <summary>
        /// This is bound to be super-slooooow
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        private static FlowResource ToResource(this IRepositoryAsync<Flow> repository, Flow f)
        {
            int? maxHL;
            string categoryName;

            //IEnumerable<Classification> classes = _ClassificationService.Query()
            //    .Include(c => c.Category)
            //    .Filter(c => c.ILCDEntityID == f.ILCDEntityID);

            IEnumerable<Classification> classes = repository.GetRepository<Classification>()
                                                .Query(c => c.ILCDEntityID == f.ILCDEntityID)
                                                .Include(c => c.Category)
                                                .Select();

            maxHL = classes.Max(c => c.Category.HierarchyLevel);
            categoryName = classes.Where(c => c.Category.HierarchyLevel == maxHL).Single().Category.Name;

            return new FlowResource
            {
                FlowID = f.FlowID,
                Name = f.Name,
                FlowTypeID = f.FlowTypeID,
                ReferenceFlowPropertyID = (int)f.ReferenceFlowProperty,
                CASNumber = f.CASNumber,
                Category = categoryName,
                UUID = f.ILCDEntity.UUID,
                Version = f.ILCDEntity.Version
            };
        }

        public static IEnumerable<FlowResource> GetFlows(this IRepositoryAsync<Flow> repository)
        {
            return repository.Query()
                .Include(f => f.ILCDEntity)
                .Select()
                .Select(f => repository.ToResource(f));
        }

        public static IEnumerable<FlowResource> GetFlows(this IRepositoryAsync<Flow> repository, int flowtypeId)
        {
            return repository.Query(f => f.FlowTypeID == flowtypeId)
                .Include(f => f.ILCDEntity)
                .Select()
                .Select(f => repository.ToResource(f));
        }

        public static IEnumerable<FlowResource> GetFlow(this IRepositoryAsync<Flow> repository, int flowId)
        {
            return repository.Query(f => f.FlowID == flowId)
                .Include(f => f.ILCDEntity)
                .Select()
                .Select(f => repository.ToResource(f));
        }

        public static IEnumerable<FlowResource> GetFlowsByFragment(this IRepositoryAsync<Flow> repository, int fragmentId)
        {
            return repository.Query(f => f.FragmentFlows.Any(ff => ff.FragmentID == fragmentId))
                .Include(f => f.ILCDEntity)
                .Select()
                .Select(f => repository.ToResource(f));
        }
    }
}
