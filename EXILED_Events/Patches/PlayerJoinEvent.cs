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
                Timing.CallDelayed(0.25f, () => { 
                    foreach(ReferenceHub player in Player.GetHubs())
                    {
                        if(player.characterClassManager.NetworkMuted)
                            player.characterClassManager.SetDirtyBit(1ul);
                    }
                });

                ReferenceHub hub = Player.GetPlayer(__instance.gameObject);
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