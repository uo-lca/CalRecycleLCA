using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LCIATool.Models.Repository
{
    public class Repository : IRepository
    {
        private LCAToolDevEntities2 context = new LCAToolDevEntities2();

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
            return context.ImpactCategories
                .Select(ic => new ImpactCategoryModel
                {
                    ImpactCategoryID = ic.ImpactCategoryID,
                    Name = ic.Name

                })
            .AsQueryable<ImpactCategoryModel>();
        }

        public IQueryable<ProcessModel> ProcessDDL()
        {

            return context.Processes
            .Select(p => new ProcessModel
            {
                ProcessID = p.ProcessID,
                Name = p.Name

            })
            .AsQueryable<ProcessModel>();
        }

        public IQueryable<LCIAMethodModel> LCIAMethodDDL()
        {

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

            var _lciaList = (from p in context.Processes
                             join pf in context.ProcessFlows on p.ProcessID equals pf.ProcessID
                             join f in context.Flows on pf.FlowID equals f.FlowID
                             join l in context.LCIAs on f.FlowID equals l.FlowID
                             join d in context.Directions on l.DirectionID equals d.DirectionID
                             join lm in context.LCIAMethods on l.LCIAMethodID equals lm.LCIAMethodID
                             join ft in context.FlowTypes on f.FlowTypeID equals ft.FlowTypeID
                             join fp in context.FlowProperties on f.FlowPropertyID equals fp.FlowPropertyID
                             //where f.FlowTypeID == 2 && (p.ProcessID == processID || processID == 0) && (lm.LCIAMethodID == lciaMethodId || lciaMethodId == 0) && (lm.ImpactCategoryID == impactCategoryId || impactCategoryId == 0)
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
    }
}