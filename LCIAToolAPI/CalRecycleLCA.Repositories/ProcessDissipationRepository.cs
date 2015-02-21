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
    public static class ProcessDissipationRepository
    {
        public static IEnumerable<InventoryModel> GetDissipation(this IRepository<ProcessDissipation> repository,
            
            int processId, int scenarioId)
        {
            int compositionModel = repository.GetRepository<ProcessComposition>().Queryable()
                .Where(k => k.ProcessID == processId)
                .Select(k => k.CompositionModelID).First(); 
            // could apply composition substitutions here easily

            // FlowID = PD.FPE.FlowID
            // DirectionID = DirectionEnum.Output
            // Composition = CD.Where CM == compositionModel GroupJoin CompositionParam
            // Dissipation = PD.EmissionFactor GroupJoin ProcessDissipationParam
            // and hand it off
            return repository.Queryable()
                .Where(pd => pd.ProcessID == processId)
                .Join(repository.GetRepository<CompositionData>().Queryable()
                    .Where(cd => cd.CompositionModelID == compositionModel),
                    pd => pd.FlowPropertyEmission.FlowPropertyID,
                    cd => cd.FlowPropertyID,
                    (pd, cd) => new { diss = pd, comp = cd })
                .GroupJoin(repository.GetRepository<ProcessDissipationParam>().Queryable()
                    .Where(pdp => pdp.Param.ScenarioID == scenarioId),
                    pd => pd.diss.ProcessDissipationID,
                    pdp => pdp.ProcessDissipationID,
                    (pd, pdp) => new { dissp = pd, parameter = pdp })
                    .SelectMany(d => d.parameter.DefaultIfEmpty(), (d, parameter) => new { disspx = d, pdisp = parameter })
                .GroupJoin(repository.GetRepository<CompositionParam>().Queryable()
                    .Where(cp => cp.Param.ScenarioID == scenarioId),
                    d => d.disspx.dissp.comp.CompositionDataID,
                    cp => cp.CompositionDataID,
                    (d, cp) => new { disspxcompp = d, pcomp = cp })
                    .SelectMany(z => z.pcomp.DefaultIfEmpty(), (z, pcomp) => new InventoryModel()
                    {
                        FlowID = z.disspxcompp.disspx.dissp.diss.FlowPropertyEmission.FlowID,
                        DirectionID = (int)DirectionEnum.Output,
                        Composition = (pcomp == null
                            ? z.disspxcompp.disspx.dissp.comp.Value
                            : pcomp.Value) * z.disspxcompp.disspx.dissp.diss.FlowPropertyEmission.Scale,
                        Dissipation = z.disspxcompp.pdisp == null
                            ? z.disspxcompp.disspx.dissp.diss.EmissionFactor
                            : z.disspxcompp.pdisp.Value
                    });
            // I could not have written that without autocomplete
        }
    }
}
