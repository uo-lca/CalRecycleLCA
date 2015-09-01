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
        private static FlowResource ToResource(this IRepository<Flow> repository, Flow f)
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
            categoryName = classes.Where(c => c.Category.HierarchyLevel == maxHL).FirstOrDefault().Category.Name;

            return new FlowResource
            {
                FlowID = f.FlowID,
                Name = f.Name,
                FlowTypeID = f.FlowTypeID,
                ReferenceFlowPropertyID = f.ReferenceFlowProperty,
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

        public static IEnumerable<FlowResource> GetCompositionFlows(this IRepository<Flow> repository)
        {
            return repository.Query(f => f.CompostionModels.Count() > 0)
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

        public static string GetCanonicalName(this IRepository<Flow> repository, int flowId)
        {
            var f = repository.GetFlow(flowId);
            return f.Name + " [" + f.Category + "]";
        }

        public static FlowResource GetFlow(this IRepository<Flow> repository, int flowId)
        {
            return repository.Query(f => f.FlowID == flowId)
                .Include(f => f.ILCDEntity)
                .Select()
                .Select(f => repository.ToResource(f)).First();
        }

        public static IEnumerable<FlowResource> GetFlowsByFragment(this IRepositoryAsync<Flow> repository, int fragmentId)
        {
            List<int> flows = repository.Queryable()
                .Where(f => f.FragmentFlows.Any(ff => ff.FragmentID == fragmentId))
                .Select(f => f.FlowID).ToList();
            flows.Add(repository.GetRepository<FragmentFlow>().GetInFlow(fragmentId, Scenario.MODEL_BASE_CASE_ID).FlowID);
            return repository.Query(f => flows.Contains(f.FlowID))
                .Include(f => f.ILCDEntity)
                .Select()
                .Select(f => repository.ToResource(f));
        }

        public static IEnumerable<FlowResource> GetFlowsByLCIAMethod(this IRepository<Flow> repository, int lciaMethodId)
        {
            List<int> flows = repository.GetRepository<LCIA>().Queryable()
                .Where(fac => fac.LCIAMethodID == lciaMethodId)
                .Where(fac => fac.FlowID != null)
                .Select(fac => (int)fac.FlowID).ToList();
            return repository.Query(f => flows.Contains(f.FlowID))
                .Include(f => f.ILCDEntity)
                .Select()
                .Select(f => repository.ToResource(f));
        }
    }
}
