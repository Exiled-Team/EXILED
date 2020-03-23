using Harmony;
using System;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(BanHandler), nameof(BanHandler.IssueBan))]
	public class PlayerBannedEvent
	{
		public static void Postfix(BanDetails ban, BanHandler.BanType banType)
		{
			try
			{
				Events.InvokePlayerBanned(ban, banType);
			}
			catch (Exception exception)
			{
				Log.Error($"PlayerBannedEvent error: {exception}");
			}
		}
	}
}
