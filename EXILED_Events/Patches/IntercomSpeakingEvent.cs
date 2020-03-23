using Harmony;
using System;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(Intercom), nameof(Intercom.CallCmdSetTransmit))]
	public class IntercomSpeakingEvent
	{
		public static bool Prefix(Intercom __instance, bool player)
		{
			if (EventPlugin.IntercomSpeakingEventPatchDisable)
				return true;

			try
			{
				if (!__instance._interactRateLimit.CanExecute(true) || Intercom.AdminSpeaking)
					return false;

				bool allow = true;

				if (player)
				{
					if (!__instance.ServerAllowToSpeak())
						return false;

					Events.InvokeIntercomSpeak(__instance.gameObject, ref allow);

					if (allow)
						Intercom.host.RequestTransmission(__instance.gameObject);
				}
				else
				{
					if (!(Intercom.host.Networkspeaker == __instance.gameObject))
						return false;

					Events.InvokeIntercomSpeak(__instance.gameObject, ref allow);

					if (allow)
						Intercom.host.RequestTransmission(null);
				}

				return false;
			}
			catch (Exception exception)
			{
				Log.Error($"IntercomSpeakEvent error: {exception}");
				return true;
			}
		}
	}
}