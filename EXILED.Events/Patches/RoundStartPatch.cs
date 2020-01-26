using System;
using EXILED.Shared.Helpers;
using Harmony;

namespace EXILED.Events.Patches
{
    [HarmonyPatch(typeof(CharacterClassManager), "CmdStartRound")]
    public class RoundStartPatch
    {
        public static void Prefix()
        {
            try
            {
                Events.Events.InvokeRoundStart();
            }
            catch (Exception e)
            {
                LogHelper.Error($"Round end event error: {e}");
            }
        }
    }
}