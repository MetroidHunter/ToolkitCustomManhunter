using RimWorld;
using System;
using ToolkitPatchLib;
using TwitchToolkit;
using TwitchToolkit.Votes;

namespace ToolkitCustomManhunter
{
    public class CustomManhunterVotingHelper<T> : VotingHelper where T : IncidentWorker
    {
        private IncidentDef incidentDef;
        private IncidentCategoryDef category;
        private IncidentWorker worker = null;
        private IncidentParms parms = null;
        private bool shouldForceFire = false;

        public CustomManhunterVotingHelper(IncidentCategoryDef cat, IncidentDef incidentDef, bool shouldForceFire = false)
        {
            category = cat;
            this.incidentDef = incidentDef;
            this.shouldForceFire = shouldForceFire;
        }

        public override bool IsPossible()
        {
            ToolkitPatchLogger.Log(incidentDef.defName, "Checking if possible..");
            worker = Activator.CreateInstance<T>();
            worker.def = incidentDef;

            parms = StorytellerUtility.DefaultParmsNow(category, target);
            parms.forced = shouldForceFire;

            bool success = worker.CanFireNow(parms);

            if (!success)
            {
                WorkerCanFireCheck.CheckDefaultFireNowConditions(worker, parms, incidentDef);
            }

            ToolkitPatchLogger.Log(incidentDef.defName, $"Can fire with params '{parms.ToString()}' on worker {worker.ToString()}? {success}");

            return success;

        }

        public override void TryExecute()
        {
            bool success = worker.TryExecute(parms);
            ToolkitPatchLogger.Log(incidentDef.defName, $"Executed. Result: {success}");
        }
    }
}
