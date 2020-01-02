using System;
using Harmony;

namespace EXILED.Patches
{
  [HarmonyPatch(typeof (CharacterClassManager), "CmdStartRound")]
  public class RoundStartPatch
  {
    public static void Prefix()
    {
      try
      {
        Events.InvokeRoundStart();
      }
      catch (Exception e)
      {
        Plugin.Error($"Round end event error: {e}");
      }
    }
  }
}