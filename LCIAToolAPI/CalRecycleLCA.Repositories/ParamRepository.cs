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
    public static class ParamRepository
    {
        public static IEnumerable<ParamResource> GetParamResource(this IRepositoryAsync<Param> repository,
            Param p)
        {
            var PR = new List<ParamResource>();
            switch (p.ParamTypeID)
            {
                case 1:
                case 2:
                    {   // DependencyParam: TODO- mark conservation flow ?
                        PR = repository.GetRepository<DependencyParam>()
                            .Query(k => k.ParamID == p.ParamID)
                            .Select(k => new ParamResource()
                        {
                            ParamID = p.ParamID,
                            ParamTypeID = (int)p.ParamTypeID,
                            ScenarioID = p.ScenarioID,
                            Name = p.Name,
                            FragmentFlowID = k.FragmentFlowID,
                            Value = k.Value
                        }).ToList();
                        break;
                    }
                case 4:
                    {   // FlowPropertyParam
                        PR = repository.GetRepository<FlowPropertyParam>()
                            .Query(k => k.ParamID == p.ParamID)
                            .Select(k => new ParamResource()
                        {
                            ParamID = p.ParamID,
                            ParamTypeID = (int)p.ParamTypeID,
                            ScenarioID = p.ScenarioID,
                            Name = p.Name,
                            FlowID = k.FlowFlowProperty.FlowID,
                            FlowPropertyID = k.FlowFlowProperty.FlowPropertyID,
                            Value = k.Value
                        }).ToList();
                        break;
                    }
                case 5:
                    {   // CompositionParam
                        PR = repository.GetRepository<CompositionParam>()
                            .Query(k => k.ParamID == p.ParamID)
                            .Select(k => new ParamResource()
                        {
                            ParamID = p.ParamID,
                            ParamTypeID = (int)p.ParamTypeID,
                            ScenarioID = p.ScenarioID,
                            Name = p.Name,
                            CompositionDataID = k.CompositionDataID,
                            Value = (double)k.Value
                        }).ToList();
                        break;
                    }
                case 6:
                    {   // ProcessDissipationParam
                        PR = repository.GetRepository<ProcessDissipationParam>()
                            .Query(k => k.ParamID == p.ParamID)
                            .Select(k => new ParamResource()
                        {
                            ParamID = p.ParamID,
                            ParamTypeID = (int)p.ParamTypeID,
                            ScenarioID = p.ScenarioID,
                            Name = p.Name,
                            FlowID = k.ProcessDissipation.FlowPropertyEmission.FlowID,
                            ProcessID = k.ProcessDissipation.ProcessID,
                            Value = k.Value
                        }).ToList();
                        break;
                    }
                case 8:
                    {   // ProcessEmissionParam
                        PR = repository.GetRepository<ProcessEmissionParam>()
                            .Query(k => k.ParamID == p.ParamID)
                            .Select(k => new ParamResource()
                        {
                            ParamID = p.ParamID,
                            ParamTypeID = (int)p.ParamTypeID,
                            ScenarioID = p.ScenarioID,
                            Name = p.Name,
                            FlowID = k.ProcessFlow.FlowID,
                            ProcessID = k.ProcessFlow.ProcessID,
                            Value = k.Value
                        }).ToList();
                        break;
                    }
                case 10:
                    {   // CharacterizationParam
                        PR = repository.GetRepository<CharacterizationParam>()
                            .Query(k => k.ParamID == p.ParamID)
                            .Select(k => new ParamResource()
                        {
                            ParamID = p.ParamID,
                            ParamTypeID = (int)p.ParamTypeID,
                            ScenarioID = p.ScenarioID,
                            Name = p.Name,
                            FlowID = k.LCIA.FlowID,
                            LCIAMethodID = k.LCIA.LCIAMethodID,
                            Value = k.Value
                        }).ToList();
                        break;
                    }
            }
            return PR;
        }
    }
}

