using System;
using Harmony;

namespace EXILED.Patches
{
    [HarmonyPatch(typeof(AlphaWarheadController), nameof(AlphaWarheadController.Detonate))]
    public class WarheadDetonationPatch
    {
        public static void Prefix()
        {
            try
            {
                Events.InvokeWarheadDetonation();
            }
            catch (Exception e)
            {
                Log.Error($"Warhead Detonation event error: {e}");
            }
        }
    }
}