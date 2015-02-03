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
    public static class LCIARepository
    {
        public static IEnumerable<LCIAModel> ComputeLCIA(this IRepositoryAsync<LCIA> repository,
            IEnumerable<InventoryModel> inventory, int lciaMethodId, int scenarioId)
        {
            return repository.Queryable()
                .Where(x => x.FlowID != null
                        && x.Geography == null
                        && x.LCIAMethodID == lciaMethodId)
                .Join(inventory,
                    l => l.FlowID,
                    i => i.FlowID,
                    (l, i) => new { l, i })
                .GroupJoin(repository.GetRepository<CharacterizationParam>().Queryable()
                        .Where(cp => cp.Param.ScenarioID == scenarioId), // Target table
                    l => l.l.LCIAID,
                    cp => cp.LCIAID,
                    (l, cp) => new { lcias = l, parameter = cp })
                .SelectMany(s => s.parameter.DefaultIfEmpty(),
                    (s, parameter) => new LCIAModel
                    {
                        FlowID = (int)s.lcias.l.FlowID,
                        DirectionID = s.lcias.l.DirectionID,
                        Quantity = (double)s.lcias.i.Result, // inventory table
                        Factor = parameter == null ? s.lcias.l.Factor : parameter.Value,
                        Geography = s.lcias.l.Geography,
                        CharacterizationParam = parameter == null ? null : new ParamInstance
                            {
                                ParamID = parameter.ParamID,
                                Value = parameter.Value
                            }
                    });
        }

        public static IEnumerable<LCIAModel> OldComputeLCIA(this IRepositoryAsync<LCIA> repository,
            IEnumerable<InventoryModel> inventory, int lciaMethodId, int scenarioId)
        {
            return inventory
           .Join(repository.Queryable().Where(x => x.FlowID != null && x.Geography == null), i => i.FlowID, l => l.FlowID, (i, l) => new { i, l })
           .Join(repository.GetRepository<LCIAMethod>().Queryable().Where(x => x.LCIAMethodID == lciaMethodId), l => l.l.LCIAMethodID, lm => lm.LCIAMethodID, (l, lm) => new { l, lm })
           .GroupJoin(repository.GetRepository<CharacterizationParam>().Queryable() // Target table
           , l => l.l.l.LCIAID
           , cp => cp.LCIAID
           , (l, cp) => new { lcias = l, characterizationParams = cp })
           .SelectMany(s => s.characterizationParams.DefaultIfEmpty()
           , (s, characterizationParams) => new
           {
               FlowID = s.lcias.l.i.FlowID,
               DirectionID = s.lcias.l.i.DirectionID,
               Quantity = s.lcias.l.i.Result,
               LCIAID = characterizationParams == null ? 0 : characterizationParams.LCIAID,
               Value = characterizationParams == null ? 0 : characterizationParams.Value,
               ParamID = characterizationParams == null ? 0 : characterizationParams.ParamID,
               LCIAMethodID = s.lcias.lm.LCIAMethodID,
               Geography = s.lcias.l.l.Geography,
               Factor = s.lcias.l.l.Factor
           })
           .GroupJoin(repository.GetRepository<Param>().Queryable() // Target table
           , cp => cp.ParamID
           , p => p.ParamID
           , (cp, p) => new { characterizationParams = cp, parameters = p })
           .SelectMany(s => s.parameters.DefaultIfEmpty()
           , (s, parameters) => new LCIAModel
           {
               FlowID = s.characterizationParams.FlowID,
               DirectionID = s.characterizationParams.DirectionID,
               Quantity = (double)s.characterizationParams.Quantity,
               Geography = s.characterizationParams.Geography,
               CharacterizationParam = parameters == null ? null : new ParamInstance
               {
                   ParamID = parameters.ParamID,
                   Value = (double)s.characterizationParams.Value
               },
               Factor = s.characterizationParams.Factor
           }).ToList();
            //leave this where clause out for now as there are no records in CharacterizationParam table with which to join on the Param table
            //so this where clause will result in no records being returned
            //.Where(x => x.ScenarioID == scenarioId)
            //.Where(x => x.DirectionID == inventory.Select(i => i.DirectionID).FirstOrDefault());
            //.Where(x => x.Geography == null);
        }

        public static IEnumerable<LCIAFactorResource> QueryFactors(this IRepositoryAsync<LCIA> repository, 
            int lciaMethodId)
        {
            return repository.Queryable()
                .Where(k => k.LCIAMethodID == lciaMethodId)
                .Where(k => k.FlowID != null)
                .Where(k => String.IsNullOrEmpty(k.Geography) )
                .Select(k => new LCIAFactorResource() {
                    LCIAMethodID = k.LCIAMethodID,
                    FlowID = (int)k.FlowID,
                    //Geography = k.Geography,
                    Direction = k.Direction.Name,
                    Factor = k.Factor
                });
        }
    }
}

