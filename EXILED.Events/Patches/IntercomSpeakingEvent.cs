using Harmony;

namespace EXILED.Events.Patches
{
	[HarmonyPatch(typeof(Intercom), nameof(Intercom.CallCmdSetTransmit))]
	public class IntercomSpeakingEvent
	{
		public static bool Prefix(Intercom __instance, bool player)
		{
			if (EventPlugin.IntercomSpeakingEventPatchDisable)
				return true;
			
			if (!__instance._interactRateLimit.CanExecute(true) || Intercom.AdminSpeaking)
				return false;

			bool allow = true;
			if (player)
			{
				if (!__instance.ServerAllowToSpeak())
					return false;
				Events.Events.InvokeIntercomSpeak(__instance.gameObject, ref allow);
				if (allow)
					Intercom.host.RequestTransmission(__instance.gameObject);
			}
			else
			{
				if (!(Intercom.host.Networkspeaker == __instance.gameObject))
					return false;
				Events.Events.InvokeIntercomSpeak(__instance.gameObject, ref allow);
				if (allow)
					Intercom.host.RequestTransmission(null);
			}

			return false;
		}
	}
}