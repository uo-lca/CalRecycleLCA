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
                        foreach (var PRi in PR)
                            PRi.DefaultValue = repository.GetRepository<FragmentFlow>()
                                .GetDefaultValue((int)PRi.FragmentFlowID, p.ScenarioID);

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
                            Value = k.Value,
                            DefaultValue = k.FlowFlowProperty.MeanValue
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
                            Value = k.Value,
                            DefaultValue = k.CompositionData.Value
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
                            Value = k.Value,
                            DefaultValue = k.ProcessDissipation.EmissionFactor
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
                            Value = k.Value,
                            DefaultValue = k.ProcessFlow.Result
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
                            Value = k.Value,
                            DefaultValue = k.LCIA.Factor
                        }).ToList();
                        break;
                    }
            }
            return PR;
        }

        private static List<int> FragmentFlowsAffected(this IRepository<Param> repository, int scenarioId, int processId)
        {
            // neglects background
            var ffs = repository.GetRepository<FragmentNodeProcess>().Queryable()
                .Where(fnp => fnp.ProcessID == processId)
                .Select(fnp => fnp.FragmentFlowID)
                .ToList();
            ffs.AddRange(repository.GetRepository<ProcessSubstitution>().Queryable()
                .Where(ps => ps.ScenarioID == scenarioId)
                .Where(ps => ps.ProcessID == processId)
                .Select(ps => ps.FragmentNodeProcess.FragmentFlowID)
                .ToList());
            return ffs;
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
            {
                cacheTracker.ParamsToPost.Add(post);
                return new List<Param>();// { repository.NewParam(scenarioId, post, ref cacheTracker) };
            }
            else
                return repository.UpdateParam((int)pid, post, ref cacheTracker);
        }


        private static bool Validate(this IRepository<Param> repository, ParamResource post)
        {
            switch (post.ParamTypeID)
            {
                case 1:
                case 2:
                    // need to confirm that the FragmentFlow being parameterized has a Process-type parent
                    return (repository.GetRepository<FragmentFlow>().Queryable()
                            .Where(k => k.FragmentFlowID == post.FragmentFlowID)
                            .Where(k => k.ParentFragmentFlow != null)
                            .Select(k => k.ParentFragmentFlow.NodeTypeID).FirstOrDefault() == 1);
                case 4:
                    // not implemented
                    return (repository.GetRepository<FlowFlowProperty>().Queryable()
                        .Where(k => k.FlowID == post.FlowID)
                        .Where(k => k.FlowPropertyID == post.FlowPropertyID)
                        .Where(k => k.Flow.ReferenceFlowProperty != post.FlowPropertyID)
                        .Count() == 1);
                case 5:
                    return (repository.GetRepository<CompositionData>().Queryable()
                        .Where(k => k.CompositionDataID == post.CompositionDataID).Count() == 1);
                case 6:
                    return (repository.GetRepository<ProcessDissipation>().Queryable()
                        .Where(k => k.ProcessID == post.ProcessID)
                        .Where(k => k.FlowPropertyEmission.FlowID == post.FlowID)
                        .Count() == 1);
                case 8:
                    return (repository.GetRepository<ProcessFlow>().Queryable()
                        .Where(k => k.ProcessID == post.ProcessID)
                        .Where(k => k.FlowID == post.FlowID)
                        .Count() > 0);
                case 10:
                    return (repository.GetRepository<LCIA>().Queryable()
                        .Where(k => k.FlowID == post.FlowID)
                        .Where(k => k.LCIAMethodID == post.LCIAMethodID)
                        .Count() > 0);

                default:
                    return false;
            }
        }

        public static List<Param> PostNewParams(this IRepository<Param> repository,
            int scenarioId, ref CacheTracker cacheTracker)
        {
            List<Param> Ps = new List<Param>();
            foreach (var post in cacheTracker.ParamsToPost)
            {
                if (repository.Validate(post))
                    Ps.Add(repository.NewParam(scenarioId, post, ref cacheTracker));
            }
            repository.InsertGraphRange(Ps);
            cacheTracker.ParamsToPost.Clear();
            return Ps;
        }


        private static string GetCanonicalName(this IRepository<Param> repository,
            ParamResource post)
        {
            string c;
            switch (post.ParamTypeID)
            {
                case 1:
                case 2:
                    {
                        var n = repository.GetRepository<FragmentFlow>()
                            .Query(k => k.FragmentFlowID == post.FragmentFlowID)
                            .Include(k => k.ParentFragmentFlow)
                            .Select().First();
                        c = n.ParentFragmentFlow.ShortName + " -> " + n.ShortName;
                        break;
                    }
                case 4:
                    {
                        var n = repository.GetRepository<FlowFlowProperty>()
                            .Query(k => k.FlowID == post.FlowID && k.FlowPropertyID == post.FlowPropertyID)
                            .Include(k => k.Flow)
                            .Include(k => k.FlowProperty)
                            .Select().First();
                        c = n.Flow.Name + " | " + n.FlowProperty.Name;
                        break;
                    }
                case 5:
                    {
                        var n = repository.GetRepository<CompositionData>()
                            .Query(k => k.CompositionDataID == post.CompositionDataID)
                            .Include(k => k.CompositionModel.Flow)
                            .Include(k => k.FlowProperty)
                            .Select().First();
                        c = n.CompositionModel.Flow.Name + " | " + n.FlowProperty.Name;
                        break;
                    }
                case 6:
                    {
                        var n = repository.GetRepository<ProcessDissipation>()
                            .Query(k => k.ProcessID == post.ProcessID && k.FlowPropertyEmission.FlowID == post.FlowID)
                            .Include(k => k.FlowPropertyEmission.FlowProperty)
                            .Include(k => k.Process)
                            .Select().First();
                        c = n.Process.Name + " | Dissipation of " + n.FlowPropertyEmission.FlowProperty.Name;
                        break;
                    }
                case 8:
                    {
                        var n = repository.GetRepository<ProcessFlow>().Queryable()
                            .Where(k => k.ProcessID == post.ProcessID)
                            .Where(k => k.FlowID == post.FlowID)
                            .Select(k => k.Process.Name).First();
                        c = n + " | " + repository.GetRepository<Flow>().GetCanonicalName((int)post.FlowID);
                        break;
                    }
                case 10:
                    {
                        var n = repository.GetRepository<LCIA>().Queryable()
                            .Where(k => k.FlowID == post.FlowID)
                            .Where(k => k.LCIAMethodID == post.LCIAMethodID)
                            .Select(k => k.LCIAMethod.Name).First();
                        c = n + " | " + repository.GetRepository<Flow>().GetCanonicalName((int)post.FlowID);
                        break;
                    }
                default:
                    c = "";
                    break;
            }
            return c;
        }

        private static Param NewParam(this IRepository<Param> repository,
            int scenarioId, ParamResource post, ref CacheTracker cacheTracker)
        {
            // this creates a new param, only after confirming that a matching one does not exist
            Param P = new Param()
            {
                ScenarioID = scenarioId,
                ParamTypeID = post.ParamTypeID,
                Name = String.IsNullOrEmpty(post.Name) 
                        ? repository.GetCanonicalName(post)
                        : post.Name,
            };
            switch (post.ParamTypeID)
            {
                case 1:
                case 2:
                    {
                        DependencyParam DP = new DependencyParam()
                        {
                            FragmentFlowID = (int)post.FragmentFlowID,
                            Value = (double)(post.Value == null 
                                ? repository.GetRepository<FragmentFlow>()
                                    .GetDefaultValue((int)post.FragmentFlowID, post.ScenarioID)
                                : post.Value),
                            ObjectState = ObjectState.Added
                        };
                        /* no more conservation param
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
                         * */
                        P.DependencyParams.Add(DP);
                        cacheTracker.FragmentFlowsTraverse.Add(DP.FragmentFlowID);
                        break;
                    }
                case 4:
                    {
                        var ffp = repository.GetRepository<FlowFlowProperty>().Queryable()
                            .Where(k => k.FlowID == post.FlowID)
                            .Where(k => k.FlowPropertyID == post.FlowPropertyID)
                            .First();
                        FlowPropertyParam fp = new FlowPropertyParam()
                        {
                            FlowFlowPropertyID = ffp.FlowFlowPropertyID,
                            Value = post.Value == null ? ffp.MeanValue : (double)post.Value,
                            ObjectState = ObjectState.Added
                        };
                        P.FlowPropertyParams.Add(fp);
                        cacheTracker.NodeCacheStale = true; // nothing to do about this- no way to tell when flow property conversion is required
                        break;
                    }
                case 5:
                    {
                        var cp = repository.GetRepository<CompositionData>().Queryable()
                            .Where(k => k.CompositionDataID == post.CompositionDataID)
                            .First();

                        P.CompositionParams.Add(new CompositionParam()
                        {
                            CompositionDataID = cp.CompositionDataID,
                            Value = post.Value == null ? cp.Value : (double)post.Value,
                            ObjectState = ObjectState.Added
                        });
                        cacheTracker.ScoreCacheStale = true;
                        break;
                    }
                case 6:
                    {
                        var pdp = repository.GetRepository<ProcessDissipation>().Queryable()
                            .Where(k => k.FlowPropertyEmission.FlowID == post.FlowID)
                            .Where(k => k.ProcessID == post.ProcessID)
                            .First();
                        ProcessDissipationParam pd = new ProcessDissipationParam()
                        {
                            ProcessDissipationID = pdp.ProcessDissipationID,
                            Value = post.Value == null ? pdp.EmissionFactor : (double)post.Value,
                            ObjectState = ObjectState.Added
                        };
                        P.ProcessDissipationParams.Add(pd);
                        cacheTracker.FragmentFlowsStale.AddRange(repository.FragmentFlowsAffected(scenarioId, (int)post.ProcessID));
                        break;
                    }
                case 8:
                    {
                        var pfp = repository.GetRepository<ProcessFlow>().Queryable()
                            .Where(k => k.FlowID == post.FlowID)
                            .Where(k => k.ProcessID == post.ProcessID)
                            .First();
                        ProcessEmissionParam pe = new ProcessEmissionParam()
                        {
                            ProcessFlowID = pfp.ProcessFlowID,
                            Value = post.Value == null ? pfp.Result : (double)post.Value,
                            ObjectState = ObjectState.Added
                        };
                        P.ProcessEmissionParams.Add(pe);
                        cacheTracker.FragmentFlowsStale.AddRange(repository.FragmentFlowsAffected(scenarioId, (int)post.ProcessID));
                        break;
                    }
                case 10:
                    {
                        var cfp = repository.GetRepository<LCIA>().Queryable()
                            .Where(k => k.FlowID == post.FlowID)
                            .Where(k => k.LCIAMethodID == post.LCIAMethodID)
                            .Where(k => String.IsNullOrEmpty(k.Geography) )
                            .First();
                        CharacterizationParam cp = new CharacterizationParam()
                            {
                                LCIAID = cfp.LCIAID,
                                Value = post.Value == null ? cfp.Factor : (double)post.Value,
                                ObjectState = ObjectState.Added
                            };
                        P.CharacterizationParams.Add(cp);
                        cacheTracker.LCIAMethodsStale.Add((int)post.LCIAMethodID);
                        break;
                    }
            }
            P.ObjectState = ObjectState.Added;
            return P;
        }

        /*
        private static DependencyParam Conserve(this IRepository<Param> repository, int dpid, double delta)
        {
            DependencyParam dp = repository.GetRepository<DependencyParam>().Query(k => k.DependencyParamID == dpid)
                .Select().First();
            dp.Value -= delta;
            dp.ObjectState = ObjectState.Modified;
            repository.GetRepository<DependencyParam>().Update(dp);
            return dp;
        }
         * 
         * */

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
            if (put.Value == null)
            {
                cacheTracker.ParamUnchanged.Add(P.ParamID);
            }
            else
            {
                switch (P.ParamTypeID)
                {
                    case 1:
                        {
                            double oldval = P.DependencyParams.First().Value;
                            if (oldval == put.Value)
                            {
                                cacheTracker.ParamUnchanged.Add(P.ParamID);
                            }
                            else
                            {
                                /*
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
                                    double delta = (double)put.Value - oldval;
                                    if (ff.DirectionID != cp.DirectionID)
                                        delta = -1 * delta;

                                    var cdp = repository.Conserve(cp.DependencyParamID, delta);
                                    Ps.Add(cdp.Param);
                                }
                                 * */

                                P.DependencyParam.Value = (double)put.Value;
                                P.DependencyParam.ObjectState = ObjectState.Modified;
                                cacheTracker.FragmentFlowsTraverse.Add(P.DependencyParam.FragmentFlowID);
                                cacheTracker.ParamModified.Add(P.ParamID);
                            }
                            break;
                        }
                        /*
                    case 2:
                        {
                            if (P.DependencyParam.Value == put.Value)
                                cacheTracker.ParamUnchanged.Add(P.ParamID);
                            else
                            {
                                P.DependencyParam.Value = (double)put.Value;
                                P.DependencyParam.ObjectState = ObjectState.Modified;
                                cacheTracker.NodeCacheStale = true;
                                cacheTracker.ParamModified.Add(P.ParamID);
                            }
                            break;
                        }
                        */
                    case 4:
                        {
                            if (P.FlowPropertyParam.Value == put.Value)
                                cacheTracker.ParamUnchanged.Add(P.ParamID);
                            else
                            {
                                P.FlowPropertyParam.Value = (double)put.Value;
                                P.FlowPropertyParam.ObjectState = ObjectState.Modified;
                                cacheTracker.NodeCacheStale = true;
                                cacheTracker.ParamModified.Add(P.ParamID);
                            }
                            break;
                        }
                    case 5:
                        {
                            if (P.CompositionParam.Value == put.Value)
                                cacheTracker.ParamUnchanged.Add(P.ParamID);
                            else
                            {
                                P.CompositionParam.Value = (double)put.Value;
                                P.CompositionParam.ObjectState = ObjectState.Modified;
                                cacheTracker.ScoreCacheStale = true;
                                cacheTracker.ParamModified.Add(P.ParamID);
                            }
                            break;
                        }
                    case 6:
                        {
                            if (P.ProcessDissipationParam.Value == put.Value)
                                cacheTracker.ParamUnchanged.Add(P.ParamID);
                            else
                            {
                                P.ProcessDissipationParam.Value = (double)put.Value;
                                P.ProcessDissipationParam.ObjectState = ObjectState.Modified;
                                cacheTracker.ParamModified.Add(P.ParamID);
                                int processId = repository.GetRepository<ProcessDissipation>().Queryable()
                                    .Where(pd => pd.ProcessDissipationID == P.ProcessDissipationParam.ProcessDissipationID)
                                    .Select(pd => pd.ProcessID)
                                    .First();
                                cacheTracker.FragmentFlowsStale.AddRange(repository.FragmentFlowsAffected(P.ScenarioID, processId));
                            }
                            break;
                        }
                    case 8:
                        {
                            if (P.ProcessEmissionParam.Value == put.Value)
                                cacheTracker.ParamUnchanged.Add(P.ParamID);
                            else
                            {
                                P.ProcessEmissionParam.Value = (double)put.Value;
                                P.ProcessEmissionParam.ObjectState = ObjectState.Modified;
                                cacheTracker.ParamModified.Add(P.ParamID);
                                int processId = repository.GetRepository<ProcessFlow>().Queryable()
                                    .Where(pf => pf.ProcessFlowID == P.ProcessEmissionParam.ProcessFlowID)
                                    .Select(pf => pf.ProcessID)
                                    .First();
                                cacheTracker.FragmentFlowsStale.AddRange(repository.FragmentFlowsAffected(P.ScenarioID, processId));
                            }
                            break;
                        }
                    case 10:
                        {
                            if (P.CharacterizationParam.Value == put.Value)
                                cacheTracker.ParamUnchanged.Add(P.ParamID);
                            else
                            {
                                int lmid = repository.GetRepository<LCIA>().Queryable()
                                                    .Where(k => k.LCIAID == P.CharacterizationParam.LCIAID)
                                                    .Select(k => k.LCIAMethodID)
                                                    .First();
                                P.CharacterizationParam.Value = (double)put.Value;
                                P.CharacterizationParam.ObjectState = ObjectState.Modified;
                                cacheTracker.LCIAMethodsStale.Add(lmid);
                                cacheTracker.ParamModified.Add(P.ParamID);
                            }
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
                    {
                        int ffid = repository.GetRepository<DependencyParam>().Query(k => k.ParamID == paramId)
                            .Select(k => k.FragmentFlowID).First();
                        cacheTracker.FragmentFlowsTraverse.Add(ffid);
                        break;
                    }
                case 4:
                    {
                        cacheTracker.NodeCacheStale = true;
                        break;
                    }
                case 5:
                    {
                        int compositionModel = repository.GetRepository<CompositionParam>().Query(k => k.ParamID == paramId)
                            .Select(k => k.CompositionData.CompositionModelID).First();
                        // have to deal with CompositionSubstitutions here if they ever become a thing
                        List<int> processIds = repository.GetRepository<ProcessComposition>().Query(k => k.CompositionModelID == compositionModel)
                            .Select(k => k.ProcessID).ToList();
                        foreach (int process in processIds)
                            cacheTracker.FragmentFlowsStale.AddRange(repository.FragmentFlowsAffected(P.ScenarioID, process));
                        break;
                    }
                case 6:
                    {
                        int processId = repository.GetRepository<ProcessDissipationParam>().Query(k => k.ParamID == paramId)
                            .Select(k => k.ProcessDissipation.ProcessID).First();
                        cacheTracker.FragmentFlowsStale.AddRange(repository.FragmentFlowsAffected(P.ScenarioID, processId));
                        break;
                    }
                case 8:
                    {
                        int processId = repository.GetRepository<ProcessEmissionParam>().Query(k => k.ParamID == paramId)
                            .Select(k => k.ProcessFlow.ProcessID).First();
                        cacheTracker.FragmentFlowsStale.AddRange(repository.FragmentFlowsAffected(P.ScenarioID, processId));
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

