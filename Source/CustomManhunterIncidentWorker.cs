using RimWorld;
using TwitchToolkit;
using ToolkitPatchLib;
using UnityEngine;
using Verse;

namespace ToolkitCustomManhunter
{
    public class CustomManhunterIncidentWorker : IncidentWorker
    {
        public const string LOGGER_NAME = "BeastAttack";
        public const float MAX_POINTS = 9999999f;

        PawnKindDef PawnKindDef;

        public CustomManhunterIncidentWorker(PawnKindDef pawnKindDef = null)
        {
            PawnKindDef = pawnKindDef;
        }

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!base.CanFireNowSub(parms))
            {
                ToolkitPatchLogger.Log(LOGGER_NAME, "Cannot fire custom manhunter from base fire now sub...");
                return false;
            }
            var map = (Map)parms.target;
            IntVec3 intVec;
            bool success = RCellFinder.TryFindRandomPawnEntryCell(out intVec, map, CellFinder.EdgeRoadChance_Animal, false, null);

            ToolkitPatchLogger.Log(LOGGER_NAME, $"Looking for edge chance animal cell: {success}");

            return success;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            IntVec3 intVec;
            if (!RCellFinder.TryFindRandomPawnEntryCell(out intVec, map, CellFinder.EdgeRoadChance_Animal, false, null))
            {
                Helper.Log($"Cant fire custom manhunter. No edge road chance entry cell");
                return false;
            }

            float percentage = parms.points / MAX_POINTS;
            float variance = Random.Range(0, 3);

            float targetCount = ((Settings.MaxCount - Settings.MinCount) * percentage) - variance;

            int count = (int)Mathf.Clamp(targetCount, Settings.MinCount, Settings.MaxCount);
            ToolkitPatchLogger.Log(LOGGER_NAME, $"Spawning {count} beasts based on ((({Settings.MaxCount} - {Settings.MinCount}) * {percentage}) - {variance})");
            for (int i = 0; i < count; i++)
            {
                var loc = CellFinder.RandomClosewalkCellNear(intVec, map, 12, null);
                var pawn = PawnGenerator.GeneratePawn(PawnKindDef, null);

                GenSpawn.Spawn(pawn, loc, map, Rot4.Random, WipeMode.Vanish, false);

                pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Manhunter);
            }

            var text = "CustomManhunterEnteringLabel".Translate(PawnKindDef.GetLabelPlural(-1));

            Find.LetterStack.ReceiveLetter("CustomManhunterStoriesLetterLabel".Translate(), text, LetterDefOf.NegativeEvent, new TargetInfo(intVec, map, false), null, null);
            return true;
        }
    }
}
