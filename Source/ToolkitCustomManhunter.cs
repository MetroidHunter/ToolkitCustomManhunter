using UnityEngine;
using Verse;
using System.Linq;
using System.Collections.Generic;
using TwitchToolkit;
using ToolkitPatchLib;

namespace ToolkitCustomManhunter
{
    public class ToolkitCustomManhunter : Mod
    {
        private Vector2 scrollPosition = Vector2.zero;

        public ToolkitCustomManhunter(ModContentPack content) : base(content)
        {
            ToolkitPatchLogger.LOGGERNAME = "CMITP";
            GetSettings<Settings>(); //load settings
        }

        public override string SettingsCategory() => "Toolkit - Custom Manhunter Incident";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            
            Listing_Standard listing = new Listing_Standard();

            ToolkitPatchLogger.Log("Mod", $"SettingsLoading");

            IEnumerable<ThingDef> AllAnimals = DefDatabase<ThingDef>.AllDefs.Where(x => x.race?.Animal ?? false).OrderByDescending(x => x.defName);

            Rect outRect = new Rect(0, 0, inRect.width, inRect.height - 50f);
            Rect viewRect = new Rect(0f, 0f, outRect.width - 20, AllAnimals.Count() * 31f);

            listing.Begin(inRect);
            listing.BeginScrollView(outRect, ref scrollPosition, ref viewRect);

            listing.Label("Assign animals to show up in the manhunter event");

            listing.Gap(12);

            foreach (ThingDef animal in AllAnimals)
            {
                bool shouldUse = false;
                if (Settings.AnimalMap.ContainsKey(animal.defName))
                {
                    shouldUse = Settings.AnimalMap[animal.defName];
                }

                listing.CheckboxLabeled($"{animal.defName}", ref shouldUse, animal.description);
                Settings.AnimalMap[animal.defName] = shouldUse;

                listing.Gap(6);
            }

            listing.EndScrollView(ref viewRect);
            listing.End();

            base.DoSettingsWindowContents(inRect);
        }

        public override void WriteSettings()
        {
            ToolkitPatchLogger.Log("Mod", "WriteSettings??");
            base.WriteSettings();
        }
    }

    public class Settings : ModSettings
    {
        public static Dictionary<string, bool> AnimalMap = new Dictionary<string, bool>();

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref AnimalMap, "AnimalMap", LookMode.Value);

            if (AnimalMap != null)
                ToolkitPatchLogger.Log("Mod", $"Loaded/Saved AnimalMap {AnimalMap.Count}");
            else
                ToolkitPatchLogger.Log("Mod", $"Loaded/Saved AnimalMap null");

            if (AnimalMap == null || !AnimalMap.Any())
            {
                AnimalMap = DefDatabase<ThingDef>.AllDefs.Where(x => x.race?.Animal ?? false).ToDictionary(x => x.defName, y => false);

                if (AnimalMap.ContainsKey("Thrumbo"))
                {
                    AnimalMap["Thrumbo"] = true;
            }
            }

            
        }
    }
}
