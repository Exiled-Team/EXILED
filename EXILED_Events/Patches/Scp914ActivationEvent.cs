using Harmony;
using Mirror;
using Scp914;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdUse914))]
	public class Scp914ActivationEvent
	{
		public static bool Prefix(PlayerInteract __instance)
		{
			if (!__instance._playerInteractRateLimit.CanExecute(true) || __instance._hc.CufferId > 0 && !__instance.CanDisarmedInteract || (Scp914Machine.singleton.working || !__instance.ChckDis(Scp914Machine.singleton.button.position)))
				return false;
			
			bool allow = true;
			double time = 0;
			Events.InvokeScp914Activation(__instance.gameObject, ref allow, ref time);
			
			Scp914Machine.singleton.RpcActivate(NetworkTime.time + time);
			__instance.OnInteract();
			
			return false;
		}
	}
}