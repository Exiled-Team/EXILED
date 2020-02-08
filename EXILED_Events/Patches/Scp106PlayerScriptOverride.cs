using Harmony;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EXILED.Patches
{
    [HarmonyPatch(typeof(Scp106PlayerScript), "CallCmdUsePortal")]
    public class Scp106PlayerScriptOverride
    {
        public static bool Prefix(Scp106PlayerScript __instance)
        {
	        try
	        {
		        if (!__instance._interactRateLimit.CanExecute(true))
			        return false;
		        if (!__instance.GetComponent<FallDamage>().isGrounded)
			        return false;

		        bool allow = true;

		        Events.InvokeScp106Teleport(__instance.gameObject, __instance.portalPosition, ref allow);

		        if (!allow)
			        return false;

		        if (__instance.iAm106 && __instance.portalPosition != Vector3.zero && !__instance.goingViaThePortal)
			        Timing.RunCoroutine(__instance._DoTeleportAnimation(), Segment.Update);
		        return true;
	        }
	        catch (Exception e)
	        {
		        Log.Error($"SCP106Portal Error: {e}");
		        return true;
	        }
        }
    }
}
