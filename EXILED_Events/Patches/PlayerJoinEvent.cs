using System;
using GameCore;
using Harmony;

namespace EXILED.Patches
{
  [HarmonyPatch(typeof(NicknameSync), "SetNick")]
	public class PlayerJoinEvent
  {
    public static void Postfix(NicknameSync __instance)
    {
      try
      {
        Events.InvokePlayerJoin(Plugin.GetPlayer(__instance.gameObject));
      }
      catch (Exception e)
      {
        Plugin.Error($"PlayerJoin Event error: {e}");
      }
    }
  }
}