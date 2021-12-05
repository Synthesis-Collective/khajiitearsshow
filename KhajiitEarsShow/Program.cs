using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using System.Threading.Tasks;
using Mutagen.Bethesda.Plugins.Exceptions;

namespace KhajiitEarsShow
{
    public class Program
    {
        public static Task<int> Main(string[] args)
        {
            return SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetTypicalOpen(GameRelease.SkyrimSE, "KhajiitEarsShow.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            foreach (var armorAddon in state.LoadOrder.PriorityOrder.ArmorAddon().WinningOverrides())
            {
                try
                {
                    if (!armorAddon.Race.Equals(Skyrim.Race.KhajiitRace)) continue;

                    if (armorAddon.BodyTemplate == null || !armorAddon.BodyTemplate.FirstPersonFlags.HasFlag(BipedObjectFlag.Ears)) continue;

                    var modifiedArmorAddon = state.PatchMod.ArmorAddons.GetOrAddAsOverride(armorAddon);

                    modifiedArmorAddon.BodyTemplate ??= new BodyTemplate();
                    modifiedArmorAddon.BodyTemplate.FirstPersonFlags &= ~BipedObjectFlag.Ears;
                }
                catch (Exception ex)
                {
                    throw RecordException.Enrich(ex, armorAddon);
                }
            }
        }
    }
}
