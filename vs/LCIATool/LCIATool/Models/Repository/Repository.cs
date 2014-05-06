﻿using System;
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

        public IQueryable<ProcessModel> ProcessDDL()
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
                             join fp in context.FlowProperties on f.FlowPropertyID equals fp.FlowPropertyID
                             //where f.FlowTypeID == 2 && (p.ProcessID == processID || processID == 0) && (lm.LCIAMethodID == lciaMethodId || lciaMethodId == 0) && (lm.ImpactCategoryID == impactCategoryId || impactCategoryId == 0)
                             where f.FlowTypeID == 2 && l.Location == null && pf.DirectionID == l.DirectionID
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