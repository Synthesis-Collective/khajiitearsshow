using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Mutagen.Bethesda.FormKeys.SkyrimSE;

namespace KhajiitEarsShow
{
    public class Program
    {
        public static int Main(string[] args)
        {
            return SynthesisPipeline.Instance.Patch<ISkyrimMod, ISkyrimModGetter>(
                args: args,
                patcher: RunPatch,
                new UserPreferences()
                {
                    ActionsForEmptyArgs = new RunDefaultPatcher()
                    {
                        IdentifyingModKey = "KhajiitEarsShow.esp",
                        TargetRelease = GameRelease.SkyrimSE
                    }
                }
            );
        }

        public static void RunPatch(SynthesisState<ISkyrimMod, ISkyrimModGetter> state)
        {
            foreach (var armorAddon in state.LoadOrder.PriorityOrder.WinningOverrides<IArmorAddonGetter>())
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
