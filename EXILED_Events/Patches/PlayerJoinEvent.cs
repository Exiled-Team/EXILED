using EXILED.Extensions;
using Harmony;
using MEC;
using System;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(NicknameSync), nameof(NicknameSync.SetNick))]
	public class PlayerJoinEvent
	{
		public static void Postfix(NicknameSync __instance)
		{
			if (EventPlugin.PlayerJoinEventPatchDisable)
				return;

			try
			{
				EventPlugin.ToMultiAdmin($"Player connect: ");

				if (PlayerManager.players.Count >= CustomNetworkManager.slots)
					EventPlugin.ToMultiAdmin($"Server full");

				//DO NOT Change this coroutine (Mutefixer)
				Timing.CallDelayed(0.25f, () =>
				{
					foreach(ReferenceHub hub in Player.GetHubs())
						if(hub.IsMuted())
							hub.characterClassManager.SetDirtyBit(1ul);
				});

				ReferenceHub player = __instance.gameObject.GetPlayer();
				if(!string.IsNullOrEmpty(player.characterClassManager.UserId))
					Events.InvokePlayerJoin(player);
			}
			catch (Exception exception)
			{
				Log.Error($"PlayerJoinEvent error: {exception}");
			}
		}
	}
}