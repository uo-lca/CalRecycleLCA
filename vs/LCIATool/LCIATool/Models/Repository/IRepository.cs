using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCIATool.Models.Repository
{
    //repository interface 
    public interface IRepository
    {
        IQueryable<Process> Processes { get; }
        IQueryable<ProcessFlow> ProcessFlows { get; }
        IQueryable<Flow> Flows { get; }
        IQueryable<LCIA> LCIAs { get; }
        IQueryable<LCIAMethod> LCIAMethods { get; }
        IQueryable<FlowType> FlowTypes { get; }
        IQueryable<FlowProperty> FlowProperties { get; }
        IQueryable<Direction> Directions { get; }
        IQueryable<ImpactCategory> ImpactCategories { get; }

        IQueryable<ProcessModel> ProcessDDL(int flows);
        IQueryable<ImpactCategoryModel> ImpactCategoryDDL();
        IQueryable<LCIAMethodModel> LCIAMethodDDL();
        IQueryable<LCIAComputationModel> LCIAComputation();
        IQueryable<IntermediateFlowModel> IntermediateFlow(int balance, int processId);
        double IntermediateFlowSum(int balance, int processId);
         
    }
}
