using LcaDataModel;
using Repository.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Pattern.Repositories;
using Entities.Models;
using Repository.Pattern.Infrastructure;

namespace CalRecycleLCA.Repositories
{
    public static class ParamRepository
    {
        public static bool IsDissipation(this IRepository<Param> repository, int processId, int flowId)
        {
            return (repository.GetRepository<ProcessDissipation>().Queryable()
                .Where(pd => pd.ProcessID == processId)
                .Where(pd => pd.FlowPropertyEmission.FlowID == flowId)
                .Count() > 0);
        }

        public static IEnumerable<ParamResource> GetParamResource(this IRepository<Param> repository,
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
                            ParamTypeID = p.ParamTypeID,
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
                            ParamTypeID = p.ParamTypeID,
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
                            ParamTypeID = p.ParamTypeID,
                            ScenarioID = p.ScenarioID,
                            Name = p.Name,
                            CompositionDataID = k.CompositionDataID,
                            Value = k.Value
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
                            ParamTypeID = p.ParamTypeID,
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
                            ParamTypeID = p.ParamTypeID,
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
                            ParamTypeID = p.ParamTypeID,
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

        public static IEnumerable<Param> PostParam(this IRepository<Param> repository,
            int scenarioId, ParamResource post, ref CacheTracker cacheTracker)
        {
            // determine if a param exists
            int pid = 0; // param id being updated
            switch (post.ParamTypeID)
            {
                case 1:
                case 2:
                    {
                        pid = repository.GetRepository<DependencyParam>().Queryable()
                            .Where(k => k.Param.ScenarioID == scenarioId)
                            .Where(k => k.FragmentFlowID == post.FragmentFlowID)
                            .Select(k => k.ParamID).FirstOrDefault();
                        break;
                    }
                case 4:
                    {
                        pid = repository.GetRepository<FlowPropertyParam>().Queryable()
                            .Where(k => k.Param.ScenarioID == scenarioId)
                            .Where(k => k.FlowFlowProperty.FlowID == post.FlowID)
                            .Where(k => k.FlowFlowProperty.FlowPropertyID == post.FlowPropertyID)
                            .Select(k => k.ParamID)
                            .FirstOrDefault();
                        break;
                    }
                case 5:
                    {
                        pid = repository.GetRepository<CompositionParam>().Queryable()
                            .Where(k => k.Param.ScenarioID == scenarioId)
                            .Where(k => k.CompositionDataID == post.CompositionDataID)
                            .Select(k => k.ParamID).FirstOrDefault();
                        break;
                    }
                case 6:
                    {
                        pid = repository.GetRepository<ProcessDissipationParam>().Queryable()
                            .Where(k => k.Param.ScenarioID == scenarioId)
                            .Where(k => k.ProcessDissipation.FlowPropertyEmission.FlowID == post.FlowID)
                            .Where(k => k.ProcessDissipation.ProcessID == post.ProcessID)
                            .Select(k => k.ParamID)
                            .FirstOrDefault();
                        break;
                    }
                case 8:
                    {
                        pid = repository.GetRepository<ProcessEmissionParam>().Queryable()
                            .Where(k => k.Param.ScenarioID == scenarioId)
                            .Where(k => k.ProcessFlow.FlowID == post.FlowID)
                            .Where(k => k.ProcessFlow.ProcessID == post.ProcessID)
                            .Select(k => k.ParamID)
                            .FirstOrDefault();
                        break;
                    }
                case 10:
                    {
                        pid = repository.GetRepository<CharacterizationParam>().Queryable()
                            .Where(k => k.Param.ScenarioID == scenarioId)
                            .Where(k => k.LCIA.FlowID == post.FlowID)
                            .Where(k => k.LCIA.LCIAMethodID == post.LCIAMethodID)
                            .Select(k => k.ParamID)
                            .FirstOrDefault();
                        break;
                    }
            }
            if (pid == 0) // no match- create a new one
                return new List<Param> { repository.NewParam(scenarioId, post, ref cacheTracker) };
            else
                return repository.UpdateParam((int)pid, post, ref cacheTracker);
        }

        private static Param NewParam(this IRepository<Param> repository,
            int scenarioId, ParamResource post, ref CacheTracker cacheTracker)
        {
            // this creates a new param, only after confirming that a matching one does not exist
            Param P = new Param()
            {
                ScenarioID = scenarioId,
                ParamTypeID = post.ParamTypeID,
                Name = post.Name,
            };
            switch (post.ParamTypeID)
            {
                case 1:
                case 2:
                    {
                        DependencyParam DP = new DependencyParam()
                        {
                            FragmentFlowID = (int)post.FragmentFlowID,
                            Value = post.Value,
                            ObjectState = ObjectState.Added
                        };
                        if (post.ParamTypeID == 2)
                        {
                            FragmentFlow c = repository.GetRepository<FragmentFlow>()
                                .Query(k => k.FragmentFlowID == post.FragmentFlowID)
                                .Include(k => k.Flow).Select().First();
                            DP.ConservationParam = new ConservationParam()
                            {
                                FragmentFlowID = (int)c.ParentFragmentFlowID,
                                FlowPropertyID = c.Flow.ReferenceFlowProperty,
                                DirectionID = c.DirectionID,
                                ObjectState = ObjectState.Added
                            };
                        }
                        P.DependencyParams.Add(DP);
                        cacheTracker.NodeCacheStale = true;
                        break;
                    }
                case 4:
                    {
                        int ffp = repository.GetRepository<FlowFlowProperty>().Queryable()
                            .Where(k => k.FlowID == post.FlowID)
                            .Where(k => k.FlowPropertyID == post.FlowPropertyID)
                            .Select(k => k.FlowFlowPropertyID)
                            .First();
                        FlowPropertyParam fp = new FlowPropertyParam()
                        {
                            FlowFlowPropertyID = ffp,
                            Value = post.Value,
                            ObjectState = ObjectState.Added
                        };
                        P.FlowPropertyParams.Add(fp);
                        cacheTracker.NodeCacheStale = true;
                        break;
                    }
                case 5:
                    {
                        P.CompositionParams.Add(new CompositionParam()
                        {
                            CompositionDataID = (int)post.CompositionDataID,
                            Value = post.Value,
                            ObjectState = ObjectState.Added
                        });
                        cacheTracker.ScoreCacheStale = true;
                        break;
                    }
                case 6:
                    {
                        int pdp = repository.GetRepository<ProcessDissipation>().Queryable()
                            .Where(k => k.FlowPropertyEmission.FlowID == post.FlowID)
                            .Where(k => k.ProcessID == post.ProcessID)
                            .Select(k => k.ProcessDissipationID)
                            .First();
                        ProcessDissipationParam pd = new ProcessDissipationParam()
                        {
                            ProcessDissipationID = pdp,
                            Value = post.Value,
                            ObjectState = ObjectState.Added
                        };
                        P.ProcessDissipationParams.Add(pd);
                        cacheTracker.ScoreCacheStale = true;
                        break;
                    }
                case 8:
                    {
                        int pfp = repository.GetRepository<ProcessFlow>().Queryable()
                            .Where(k => k.FlowID == post.FlowID)
                            .Where(k => k.ProcessID == post.ProcessID)
                            .Select(k => k.ProcessFlowID)
                            .First();
                        ProcessEmissionParam pe = new ProcessEmissionParam()
                        {
                            ProcessFlowID = pfp,
                            Value = post.Value,
                            ObjectState = ObjectState.Added
                        };
                        P.ProcessEmissionParams.Add(pe);
                        cacheTracker.ScoreCacheStale = true;
                        break;
                    }
                case 10:
                    {
                        int cfp = repository.GetRepository<LCIA>().Queryable()
                            .Where(k => k.FlowID == post.FlowID)
                            .Where(k => k.LCIAMethodID == post.LCIAMethodID)
                            .Select(k => k.LCIAID)
                            .First();
                        CharacterizationParam cp = new CharacterizationParam()
                            {
                                LCIAID = cfp,
                                Value = post.Value,
                                ObjectState = ObjectState.Added
                            };
                        P.CharacterizationParams.Add(cp);
                        cacheTracker.LCIAMethodsStale.Add((int)post.LCIAMethodID);
                        break;
                    }
            }
            P.ObjectState = ObjectState.Added;
            repository.InsertOrUpdateGraph(P);
            return P;
        }

        private static DependencyParam Conserve(this IRepository<Param> repository, int dpid, double delta)
        {
            DependencyParam dp = repository.GetRepository<DependencyParam>().Query(k => k.DependencyParamID == dpid)
                .Select().First();
            dp.Value -= delta;
            dp.ObjectState = ObjectState.Modified;
            repository.GetRepository<DependencyParam>().Update(dp);
            return dp;
        }

        public static IEnumerable<Param> UpdateParam(this IRepository<Param> repository,
            int paramId, ParamResource put, ref CacheTracker cacheTracker)
        {
            List<Param> Ps = new List<Param>();
            Param P = repository.Query(k => k.ParamID == paramId)
                .Include(k => k.DependencyParams)
                .Include(k => k.FlowPropertyParams)
                .Include(k => k.CompositionParams)
                .Include(k => k.ProcessDissipationParams)
                .Include(k => k.ProcessEmissionParams)
                .Include(k => k.CharacterizationParams)
                .Select().First();
            if (put.Name != null)
                P.Name = put.Name;
            if (put.Value != null)
            { 
                switch (P.ParamTypeID)
                {
                    case 1:
                        {
                            double oldval = P.DependencyParams.Where(k => k.FragmentFlowID == put.FragmentFlowID).First().Value;
                            FragmentFlow ff = repository.GetRepository<FragmentFlow>().Query(k => k.FragmentFlowID == put.FragmentFlowID)
                                .Include(k => k.Flow)
                                .Select().First();
                            ConservationParam cp = repository.GetRepository<ConservationParam>().Queryable()
                                .Where(k => k.DependencyParam.Param.ScenarioID == P.ScenarioID)
                                .Where(k => k.FragmentFlowID == ff.ParentFragmentFlowID)
                                .Where(k => k.FlowPropertyID == ff.Flow.ReferenceFlowProperty)
                                .FirstOrDefault();
                            if (cp != null)
                            {
                                double delta = put.Value - oldval;
                                if (ff.DirectionID != cp.DirectionID)
                                    delta = -1 * delta;

                                var cdp = repository.Conserve(cp.DependencyParamID, delta);
                                Ps.Add(cdp.Param);

                            }
                            P.DependencyParams.Where(k => k.FragmentFlowID == put.FragmentFlowID).First().Value = put.Value;
                            P.DependencyParams.Where(k => k.FragmentFlowID == put.FragmentFlowID).First().ObjectState = ObjectState.Modified;
                            cacheTracker.NodeCacheStale = true;
                            break;
                        }
                    case 2:
                        {
                            P.DependencyParams.Where(k => k.FragmentFlowID == put.FragmentFlowID).First().Value = put.Value;
                            P.DependencyParams.Where(k => k.FragmentFlowID == put.FragmentFlowID).First().ObjectState = ObjectState.Modified;
                            cacheTracker.NodeCacheStale = true;
                            break;
                        }
                    case 4:
                        {
                            P.FlowPropertyParam.Value = put.Value;
                            P.FlowPropertyParam.ObjectState = ObjectState.Modified;
                            cacheTracker.NodeCacheStale = true;
                            break;
                        }
                    case 5:
                        {
                            P.CompositionParams.Where(k => k.CompositionDataID == put.CompositionDataID).First().Value = put.Value;
                            P.CompositionParams.Where(k => k.CompositionDataID == put.CompositionDataID).First().ObjectState = ObjectState.Modified;
                            cacheTracker.ScoreCacheStale = true;
                            break;
                        }
                    case 6:
                        {
                            P.ProcessDissipationParam.Value = put.Value;
                            P.ProcessDissipationParam.ObjectState = ObjectState.Modified;
                            cacheTracker.ScoreCacheStale = true;
                            break;
                        }
                    case 8:
                        {
                            P.ProcessEmissionParam.Value = put.Value;
                            P.ProcessEmissionParam.ObjectState = ObjectState.Modified;
                            cacheTracker.ScoreCacheStale = true;
                            break;
                        }
                    case 10:
                        {
                            int lmid = repository.GetRepository<LCIA>().Queryable()
                                                .Where(k => k.LCIAID == P.CharacterizationParam.LCIAID)
                                                .Select(k => k.LCIAMethodID)
                                                .First();
                            P.CharacterizationParam.Value = put.Value;
                            P.CharacterizationParam.ObjectState = ObjectState.Modified;
                            cacheTracker.LCIAMethodsStale.Add(lmid);
                            break;
                        }
                }
            }
            P.ObjectState = ObjectState.Modified;
            repository.Update(P);
            Ps.Add(P);
            return Ps;
        }

        public static void DeleteParam(this IRepository<Param> repository, int paramId, ref CacheTracker cacheTracker)
        {
            Param P = repository.Query(k => k.ParamID == paramId)
                .Select().First();
            switch (P.ParamTypeID)
            {
                case 1:
                case 2:
                case 4:
                    {
                        cacheTracker.NodeCacheStale = true;
                        break;
                    }
                case 5:
                case 6:
                case 8:
                    {
                        cacheTracker.ScoreCacheStale = true;
                        break;
                    }
                case 10:
                    {
                        int lmid = repository.GetRepository<CharacterizationParam>().Query(k => k.ParamID == paramId)
                            .Select(k => k.LCIA.LCIAMethodID).First();
                        cacheTracker.LCIAMethodsStale.Add(lmid);
                        break;
                    }
            }
            repository.Delete(P);
        }
    }
}

