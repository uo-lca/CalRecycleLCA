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
    public static class FlowPropertyRepository
    {
        private static FlowPropertyResource ToResource(FlowProperty fp)
        {
            // this is solely to account for FlowPropertyID 186, c79145ca-cc90-49b1-905a-51aaedc0e36b, whose unitgroup is missing
            string unit;
            if (fp.UnitGroup == null)
                unit = "null";
            else
                unit = fp.UnitGroup.ReferenceUnit;
            return new FlowPropertyResource
            {
                FlowPropertyID = fp.FlowPropertyID,
                Name = fp.Name,
                ReferenceUnit = unit,
                UUID = fp.ILCDEntity.UUID,
                Version = fp.ILCDEntity.Version
            };
        } 

        public static IEnumerable<FlowPropertyResource> GetResources(this IRepositoryAsync<FlowProperty> repository) 
        {
            //return repository.Queryable().ToResource();
            return repository.Query()
                .Include(fp => fp.ILCDEntity)
                .Include(fp => fp.UnitGroup)
                .Select()
                .Select( fp => ToResource(fp)).ToList();
        }
        public static IEnumerable<FlowPropertyResource> GetFlowPropertiesByProcess(this IRepositoryAsync<FlowProperty> repository,
            int processId)
        {
            var FPs = repository.GetRepository<ProcessFlow>().Queryable()
                .Where(k => k.ProcessID == processId)
                .Join(repository.GetRepository<FlowFlowProperty>().Queryable(), pf => pf.FlowID, k => k.FlowID,
                    (pf, k) => new { pf = pf, ffp = k }).Select(j => j.ffp.FlowPropertyID).Distinct().ToList();

            return repository.Query(fp => FPs.Contains(fp.FlowPropertyID))
                .Include(fp => fp.ILCDEntity)
                .Include(fp => fp.UnitGroup)
                .Select()
                .Select(fp => ToResource(fp)).ToList();

        }
        public static IEnumerable<FlowPropertyResource> GetFlowPropertiesByFragment(this IRepositoryAsync<FlowProperty> repository,
            int fragmentId)
        {
            // FOUR queries AND a node termination? seriously?
            var FPs = repository.GetRepository<FragmentFlow>().Queryable()
                .Where(k => k.FragmentID == fragmentId)
                .Join(repository.GetRepository<FlowFlowProperty>().Queryable(), ff => ff.FlowID, k => k.FlowID,
                    (ff, k) => new { ff = ff, ffp = k }).Select(j => j.ffp.FlowPropertyID)
                    .Distinct().ToList();

            var termFlow = repository.GetRepository<FragmentFlow>().GetInFlow(fragmentId, Scenario.MODEL_BASE_CASE_ID).FlowID;
            FPs.AddRange(repository.GetRepository<FlowFlowProperty>().Queryable().Where(f => f.FlowID == termFlow).Select(f => f.FlowPropertyID));
            FPs = FPs.Distinct().ToList();

            return repository.Query(fp => FPs.Contains(fp.FlowPropertyID))
                .Include(fp => fp.ILCDEntity)
                .Include(fp => fp.UnitGroup)
                .Select()
                .Select(fp => ToResource(fp)).ToList();
        }
    }
}
