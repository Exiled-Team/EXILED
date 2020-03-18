using System;
using EXILED.Extensions;
using Harmony;
using MEC;

namespace EXILED.Patches
{
    [HarmonyPatch(typeof(NicknameSync), nameof(NicknameSync.SetNick))]
    public class PlayerJoinEvent
    {
        public static void Postfix(NicknameSync __instance)
        {
            if (EventPlugin.PlayerJoinEventPatchDisable)
                return;

            Log.Info($"Player connect: ");
            if(PlayerManager.players.Count >= CustomNetworkManager.slots)
                Log.Info($"Server full");

            try
            {
                MuteHandler.Mutes.Clear();
                MuteHandler.Reload();
                Timing.CallDelayed(1f, () => {
                    if (MuteHandler.QueryPersistantMute(__instance._ccm.UserId))
                        __instance._ccm.SetMuted(true);
                    else
                        __instance._ccm.SetMuted(false);
                });

                ReferenceHub hub = __instance.gameObject.GetPlayer();
                if (!string.IsNullOrEmpty(hub.characterClassManager.UserId))
                    Events.InvokePlayerJoin(hub);
            }
            catch (Exception e)
            {
                Log.Error($"PlayerJoin Event error: {e}");
            }
        }
    }
}