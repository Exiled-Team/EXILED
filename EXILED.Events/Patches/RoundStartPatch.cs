using System;
using EXILED.Shared;
using Harmony;

namespace EXILED.Events.Patches
{
  [HarmonyPatch(typeof (CharacterClassManager), "CmdStartRound")]
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
        Plugin.Error($"Round end event error: {e}");
      }
    }
  }
}