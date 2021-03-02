using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using System.Threading.Tasks;

namespace KhajiitEarsShow
{
    public class Program
    {
        public static Task<int> Main(string[] args)
        {
            return SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .Run(args, new RunPreferences()
                {
                    ActionsForEmptyArgs = new RunDefaultPatcher()
                    {
                        IdentifyingModKey = "KhajiitEarsShow.esp",
                        TargetRelease = GameRelease.SkyrimSE,
                        BlockAutomaticExit = false,
                    }
                });
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            foreach (var armorAddon in state.LoadOrder.PriorityOrder.ArmorAddon().WinningOverrides())
            {
                try
                {
                    if (!armorAddon.Race.FormKey.Equals(Skyrim.Race.KhajiitRace)) continue;

                    if (armorAddon.BodyTemplate == null || !armorAddon.BodyTemplate.FirstPersonFlags.HasFlag(BipedObjectFlag.Ears)) continue;

                    var modifiedArmorAddon = state.PatchMod.ArmorAddons.GetOrAddAsOverride(armorAddon);

                    if (modifiedArmorAddon.BodyTemplate == null)
                    {
                        modifiedArmorAddon.BodyTemplate = new BodyTemplate();
                    }

                    modifiedArmorAddon.BodyTemplate.FirstPersonFlags &= ~BipedObjectFlag.Ears;
                }
                catch (Exception ex)
                {
                    throw RecordException.Factory(ex, armorAddon);
                }
            }
        }
    }
}
