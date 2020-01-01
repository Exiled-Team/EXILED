using Harmony;

namespace EXILED.Patches
{
  [HarmonyPatch(typeof (CharacterClassManager), "CmdStartRound")]
  public class RoundStartPatch
  {
    public static void Prefix()
    {
      Events.InvokeRoundStart();
    }
  }
}