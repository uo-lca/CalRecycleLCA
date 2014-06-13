using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LCIATool.Models.Repository
{
    //Repository class implements all of the functions listed in IRepository class.
    //These functions can then be called from within the web API via the interface.
    public class Repository : IRepository
    {
        private LCAToolDevEntities1 context = new LCAToolDevEntities1();

        public IQueryable<Process> Processes
        {
            get { return context.Processes; }
        }

        public IQueryable<ProcessFlow> ProcessFlows
        {
            get { return context.ProcessFlows; }
        }

        public IQueryable<Flow> Flows
        {
            get { return context.Flows; }
        }

        public IQueryable<LCIA> LCIAs
        {
            get { return context.LCIAs; }
        }

        public IQueryable<LCIAMethod> LCIAMethods
        {
            get { return context.LCIAMethods; }
        }

        public IQueryable<FlowType> FlowTypes
        {
            get { return context.FlowTypes; }
        }

        public IQueryable<FlowProperty> FlowProperties
        {
            get { return context.FlowProperties; }
        }

        public IQueryable<Direction> Directions
        {
            get { return context.Directions; }
        }

        public IQueryable<ImpactCategory> ImpactCategories
        {
            get { return context.ImpactCategories; }
        }

        public IQueryable<ImpactCategoryModel> ImpactCategoryDDL()
        {
            //returns a list of Impact Categories as IQueryable<ImpactCategoryModel> type
            //with just the fields needed for the dropdownlist
            return context.ImpactCategories
                .Select(ic => new ImpactCategoryModel
                {
                    ImpactCategoryID = ic.ImpactCategoryID,
                    Name = ic.Name

                })
            .AsQueryable<ImpactCategoryModel>();
        }

        //public IQueryable<ProcessModel> ProcessDDL()
        //{
        //    //returns a list of Processes as IQueryable<ProcessModel> type
        //    //with just the fields needed for the dropdownlist
        //    return context.Processes
        //    .Select(p => new ProcessModel
        //    {
        //        ProcessID = p.ProcessID,
        //        Name = p.Name

        //    })
        //    .AsQueryable<ProcessModel>();
        //}

        public IQueryable<ProcessModel> ProcessDDL(int flows)
        {
            if (flows == 1)
            {
                var processFlows = context.ProcessFlows
         .Where(e => e.Flow.FlowType.FlowTypeID == 2)
             .GroupBy(p => new
                   {
                       ProcessID = p.ProcessID,
                       ProcessName = p.Process.Name

                   })
    .SelectMany(cl => cl.Select(p => new ProcessModel
                   {
                       ProcessID = p.ProcessID,
                       Name = p.Process.Name
                   }).Distinct()).AsQueryable();
                return processFlows;
            }
            else if (flows == 2)
            {
                var processFlows = context.ProcessFlows
             .Where(e => e.Flow.FlowType.FlowTypeID == 2)
             .OrderBy(e => e.ProcessID)
              .GroupBy(r => new { r.ProcessID })
              .Select(r => new ProcessFlowModel
              {
                  ProcessID = r.Key.ProcessID,
                  emcounts = r.GroupBy(g => g.ProcessFlowID).Count()
              }).AsQueryable();

                var results = (from processes in context.Processes
                               join pFlows in processFlows
                                   on processes.ProcessID equals pFlows.ProcessID into joined
                               from pFlows in joined.DefaultIfEmpty()
                               where pFlows.emcounts == null
                               select new ProcessModel
                               {
                                   ProcessID = processes.ProcessID,
                                   Name = processes.Name
                               }).AsQueryable();

                return results;

            }
            else
            {
                //returns a list of Processes as IQueryable<ProcessModel> type
                    //with just the fields needed for the dropdownlist
                    return context.Processes
                    .Select(p => new ProcessModel
                    {
                        ProcessID = p.ProcessID,
                        Name = p.Name

                    })
                    .AsQueryable<ProcessModel>();
            }
           


        }

        public IQueryable<LCIAMethodModel> LCIAMethodDDL()
        {
            //returns a list of LCIAMethods as IQueryable<LCIAMethodModel> type
            //with just the fields needed for the dropdownlist
            return context.LCIAMethods
            .Select(lm => new LCIAMethodModel
            {
                LCIAMethodID = lm.LCIAMethodID,
                Name = lm.Name,
                ImpactCategoryID = lm.ImpactCategoryID


            })
            .AsQueryable<LCIAMethodModel>();
        }

        public IQueryable<LCIAComputationModel> LCIAComputation()
        {
            //returns a list of records as IQueryable<LCIAComputationModel> type
            //these records are grouped by process
            var _lciaList = (from p in context.Processes
                             join pf in context.ProcessFlows on p.ProcessID equals pf.ProcessID
                             join f in context.Flows on pf.FlowID equals f.FlowID
                             join l in context.LCIAs on f.FlowID equals l.FlowID
                             join d in context.Directions on l.DirectionID equals d.DirectionID
                             join lm in context.LCIAMethods on l.LCIAMethodID equals lm.LCIAMethodID
                             join ft in context.FlowTypes on f.FlowTypeID equals ft.FlowTypeID
                             //where f.FlowTypeID == 2 && (p.ProcessID == processID || processID == 0) && (lm.LCIAMethodID == lciaMethodId || lciaMethodId == 0) && (lm.ImpactCategoryID == impactCategoryId || impactCategoryId == 0)
                             where f.FlowTypeID == 2 && l.Geography == null && pf.DirectionID == l.DirectionID
                             group new { p, pf, f, l, lm, ft } by new
                             {
                                 Process = p.Name,
                                 f.FlowID,
                                 ft.FlowTypeID,
                                 p.ProcessID,
                                 lm.LCIAMethodID,
                                 lm.ImpactCategoryID,
                                 Flow = f.Name,
                                 d.Name,
                                 Quantity = pf.Result,
                                 pf.STDev,
                                 l.Factor
                             } into g
                             select new LCIAComputationModel
                             {
                                 ProcessName = g.Key.Process,
                                 FlowID = g.Key.FlowID,
                                 Flow = g.Key.Flow,
                                 FlowTypeID = g.Key.FlowTypeID,
                                 ProcessID = g.Key.ProcessID,
                                 LCIAMethodID = g.Key.LCIAMethodID,
                                 ImpactCategoryID = g.Key.ImpactCategoryID,
                                 Direction = g.Key.Name,
                                 Quantity = g.Key.Quantity,
                                 STDev = g.Key.STDev,
                                 Factor = g.Key.Factor,
                                 LCIAResult = g.Key.Quantity * g.Key.Factor
                             }).AsQueryable();

            return _lciaList;
        }

        public IQueryable<IntermediateFlowModel> IntermediateFlow(int balance, int processId)
        {
            if (balance == 1)
            {

                var query = context.ProcessFlows
                    .Where(e => e.Flow.FlowType.FlowTypeID != 2 && e.Flow.ReferenceFlowProperty != null && (e.Process.ProcessID == processId || processId == 0))
                    .GroupBy(p => new
                    {
                        ReferenceProperty = p.Flow.FlowProperty.Name,
                        ReferenceUnit = p.Flow.FlowProperty.UnitGroup.ReferenceUnit
                    })
                    .SelectMany(pf => pf.Select(p => new IntermediateFlowModel
                    {
                        ReferenceProperty = p.Flow.FlowProperty.Name,
                        ReferenceUnit = p.Flow.FlowProperty.UnitGroup.Name,
                        Quantity = p.Result,
                        NetFlowIn = p.Result * (p.Direction.Name == "Input" ? +1 : p.Direction.Name == "Output" ? -1 : 0)
                    })).AsQueryable();
                return query.OrderBy(pFlow => new { pFlow.ReferenceProperty, pFlow.ReferenceUnit });


            }
            else
            {
                //get all intermediate flows
                var query = context.ProcessFlows
               .Where(e => e.Flow.FlowType.FlowTypeID != 2 && e.Flow.ReferenceFlowProperty != null && (e.Process.ProcessID == processId || processId == 0))
               .GroupBy(p => new
               {
                   ReferenceUnit = p.Flow.FlowProperty.UnitGroup.ReferenceUnit
               })
               .SelectMany(pf => pf.Select(p => new IntermediateFlowModel
               {
                   ProcessFlowID = p.ProcessFlowID,
                   FlowName = p.Flow.Name,
                   FlowDirection = p.Direction.Name,
                   ReferenceProperty = p.Flow.FlowProperty.Name,
                   ReferenceUnit = p.Flow.FlowProperty.UnitGroup.ReferenceUnit,
                   Quantity = p.Result,
                   FlowType = p.Flow.FlowType.Name,
                   FlowTypeID = p.Flow.FlowType.FlowTypeID,
                   MaxUnit = null,
                   SankeyWidth = null
               })).AsQueryable();
                //return query.OrderBy(pFlow => new { pFlow.FlowPropertyName, pFlow.ReferenceUnit });




                //Normalize the flow quantities on a by-Reference-Unit basis in order to scale properly in the visualization. 
                //To this we need to add a SankeyWidth column which is derived from the largest flow in any given unit
                var maxUnit = query
                .Where(e => e.FlowTypeID != 2 )
               .GroupBy(referenceunit => new
               {
                   refUnit = referenceunit.ReferenceUnit
               }
               )
               .Select(
                    maxUnitGroup =>
                        new MaxUnitModel
                        {
                            ReferenceUnit = maxUnitGroup.Key.refUnit,
                            ProcessFlowResult = maxUnitGroup.Max(res => res.Quantity),
                        }).AsQueryable();



                //Join the two linq queries 
                var results = query.Join(maxUnit,
                    main => main.ReferenceUnit,
                    mUnit => mUnit.ReferenceUnit,
                    (main, mUnit) => new IntermediateFlowModel
                        {
                            ProcessFlowID = main.ProcessFlowID,
                            FlowName = main.FlowName,
                            FlowDirection = main.FlowDirection,
                            ReferenceProperty = main.ReferenceProperty,
                            ReferenceUnit = main.ReferenceUnit,
                            Quantity = main.Quantity,
                            FlowType = main.FlowType,
                            FlowTypeID = main.FlowTypeID,
                            MaxUnit = mUnit.ProcessFlowResult,
                            SankeyWidth = (main.Quantity / mUnit.ProcessFlowResult)
                        }
                 ).AsQueryable();

                return results;

            }
        }

        public double IntermediateFlowSum(int balance, int processId)
        {
            IQueryable<IntermediateFlowModel> list = IntermediateFlow(balance, processId);

            double sum = Convert.ToDouble(list.Sum(od => od.Quantity));
            return sum;

            //int cnt = list.Count();
            //return cnt;
        }
    }
}