using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXILED.Patches
{
    [HarmonyPatch(typeof(Scp914.Scp914Machine), "ProcessItems")]
    public class _914MachineOverride
    {
        public void Postfix(Scp914.Scp914Machine __instance)
        {
            if (EventPlugin.NineFourteenMachinePatchDisable)
                return;
            try
            {
                Events.InvokeSCP914Upgrade(__instance, __instance.players, __instance.items, __instance.knobState);
            }
            catch (Exception e)
            {
                Plugin.Error($"SCP914Upgrade Patch error: {e}");
                return;
            }
        }
    }
}
