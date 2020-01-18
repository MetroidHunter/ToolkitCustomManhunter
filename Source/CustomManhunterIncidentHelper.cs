using RimWorld;
using System.Collections.Generic;
using System.Linq;
using ToolkitPatchLib;
using TwitchToolkit;
using TwitchToolkit.IncidentHelpers;
using TwitchToolkit.Store;
using Verse;

namespace ToolkitCustomManhunter
{
    public class CustomManhunterIncidentHelper : IncidentHelperVariables
    {
        public const string LOGGER_NAME = "BeastAttack";

        public int pointsWager = 0;
        public IIncidentTarget target = null;
        public IncidentWorker worker = null;
        public IncidentParms parms = null;
        private bool separateChannel = false;

        public override Viewer Viewer { get; set; }

        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            this.separateChannel = separateChannel;
            this.Viewer = viewer;
            string[] command = message.Split(' ');
            if (command.Length < 3)
            {
                VariablesHelpers.ViewerDidWrongSyntax(viewer.username, storeIncident.syntax, separateChannel);
                return false;
            }

            if (!VariablesHelpers.PointsWagerIsValid(
                    command[2],
                    viewer,
                    ref pointsWager,
                    ref storeIncident,
                    separateChannel
                ))
            {
                ToolkitPatchLogger.Log(LOGGER_NAME, "Cannot fire custom manhunter. No valid points wagered");
                return false;
            }

            target = Current.Game.AnyPlayerHomeMap;
            if (target == null)
            {
                ToolkitPatchLogger.Log(LOGGER_NAME, "Cannot fire custom manhunter. No valid tile");
                return false;
            }

            parms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.RaidBeacon, target);
            parms.points = IncidentHelper_PointsHelper.RollProportionalGamePoints(storeIncident, pointsWager, parms.points);

            IEnumerable<string> animals = Settings.AnimalMap.Where(x => x.Value).Select(x => x.Key);

            ToolkitPatchLogger.Log(LOGGER_NAME, $"{animals.Count()}/{Settings.AnimalMap.Count} animals for selection.");

            string animal = animals.RandomElement();

            ToolkitPatchLogger.Log(LOGGER_NAME, $"Chose animal {animal} for the attack beast");

            ThingDef def = ThingDef.Named(animal);

            worker = new CustomManhunterIncidentWorker(PawnKindDef.Named(animal));
            worker.def = IncidentDef.Named("BeastAttack");

            bool success =  worker.CanFireNow(parms);

            if (!success)
            {
                WorkerCanFireCheck.CheckDefaultFireNowConditions(worker, parms, worker.def);
            }

            return success;
        }

        public override void TryExecute()
        {
            if (worker.TryExecute(parms))
            {
                Viewer.TakeViewerCoins(pointsWager);
                Viewer.CalculateNewKarma(this.storeIncident.karmaType, pointsWager);
                VariablesHelpers.SendPurchaseMessage($"Starting beast attack with {pointsWager} points wagered and {(int)parms.points} raid points purchased by {Viewer.username}", separateChannel);
                return;
            }
            Toolkit.client.SendMessage($"@{Viewer.username} could not generate parms for beast attack.", separateChannel);
        }


    }
}
