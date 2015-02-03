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
            return repository.Query(fp => fp.Flows.Any(f => f.ProcessFlows.Any(pf => pf.ProcessID == processId)))
                .Include(fp => fp.ILCDEntity)
                .Include(fp => fp.UnitGroup)
                .Select()
                .Select(fp => ToResource(fp)).ToList();

        }
        public static IEnumerable<FlowPropertyResource> GetFlowPropertiesByFragment(this IRepositoryAsync<FlowProperty> repository,
            int fragmentId)
        {
            return repository.Query(fp => fp.Flows.Any(f => f.FragmentFlows.Any(ff => ff.FragmentID == fragmentId)))
                .Include(fp => fp.ILCDEntity)
                .Include(fp => fp.UnitGroup)
                .Select()
                .Select(fp => ToResource(fp)).ToList();
        }
    }
}
