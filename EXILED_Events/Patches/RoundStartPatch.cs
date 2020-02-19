using System;
using Harmony;

namespace EXILED.Patches
{
  [HarmonyPatch(typeof (CharacterClassManager), nameof(CharacterClassManager.CmdStartRound))]
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
        Log.Error($"Round end event error: {e}");
      }
    }
  }
}