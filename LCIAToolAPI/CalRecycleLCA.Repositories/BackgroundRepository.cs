using LcaDataModel;
using Repository.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Pattern.Repositories;
using Entities.Models;

/*
        public int RefID { get; set; } // referer-- either FragmentNodeProcessID, FragmentNodeFragmentID, FragmentFlowID, BackgroundID
        public int ScenarioID { get; set; }   // node behavior is scenario dependent
        public int NodeTypeID { get; set; }   // included here for background resolution
        public int? ProcessID { get; set; }     // FragmentNodeProcess.ProcessID, when NodeType is Process
        public int? SubFragmentID { get; set; } // FragmentNodeFragment.SubFragmentID, when NodeType is Fragment
        public int TermFlowID { get; set; }  //  FragmentNodeProcess.FlowID or FragmentNodeFragment.FlowID depending on whether NodeType is Process or Fragment 
*/
namespace CalRecycleLCA.Repositories
{
    public static class BackgroundRepository
    {
        ///** ************************
        public static FragmentNodeResource ResolveBackground(this IRepository<Background> repository, 
						     int? flowId, int directionId, int scenarioId)
	    {
            var background = new FragmentNodeResource();
            int targetId;

            background = repository.GetRepository<ScenarioBackground>().Queryable()
                .Where(x => x.ScenarioID == scenarioId)
                .Where(x => x.FlowID == flowId)
                .Where(x => x.DirectionID == directionId)
               .Select(bg => new FragmentNodeResource
               {
                   NodeTypeID = bg.NodeTypeID,
                   ScenarioID = scenarioId,
                   RefID = (int)bg.ILCDEntityID,
                   TermFlowID = (int)flowId
               }).FirstOrDefault();

            if (background == null)
            {
                background = repository.GetRepository<Background>().Queryable()
                .Where(x => x.FlowID == flowId)
                .Where(x => x.DirectionID == directionId)
                .Select(bg => new FragmentNodeResource
                {
                    NodeTypeID = bg.NodeTypeID,
                    ScenarioID = 0,
                    RefID = (int)bg.ILCDEntityID,
                    TermFlowID = (int)flowId
                }).FirstOrDefault();

                if (background == null)
                {
                    throw new ArgumentNullException("background is null");
                }
            }

            switch (background.NodeTypeID)
            {
                case 1:
                    targetId = repository.GetRepository<Process>().Queryable()
                        .Where(x => x.ILCDEntityID == background.RefID)
                        .Select(z => (int)z.ProcessID).FirstOrDefault();
                    background.ProcessID = targetId;
                    break;
                case 2:
                    targetId = repository.GetRepository<Fragment>().Queryable()
                        .Where(x => x.ILCDEntityID == background.RefID)
                        .Select(z => (int)z.FragmentID).FirstOrDefault();
                    background.SubFragmentID = targetId;
                    break;
            }
            return background;
        }
            
    }
}
