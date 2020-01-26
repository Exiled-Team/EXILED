using System;
using EXILED.Shared.Helpers;
using Harmony;

namespace EXILED.Events.Patches
{
  [HarmonyPatch(typeof(NicknameSync), "SetNick")]
	public class PlayerJoinEvent
  {
    public static void Postfix(NicknameSync __instance)
    {
      if (EventPlugin.PlayerJoinEventPatchDisable)
        return;
      
      try
      {
        ReferenceHub hub = PlayersHelper.GetPlayer(__instance.gameObject);
        if (!string.IsNullOrEmpty(hub.characterClassManager.UserId))
          Events.Events.InvokePlayerJoin(hub);
      }
      catch (Exception e)
      {
        LogHelper.Error($"PlayerJoin Event error: {e}");
      }
    }
  }
}