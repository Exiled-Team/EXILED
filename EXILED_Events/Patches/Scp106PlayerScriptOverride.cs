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
        public static void Prefix(Scp106PlayerScript __instance)
        {
			if (!__instance._interactRateLimit.CanExecute(true))
				return;
			if (!__instance.GetComponent<FallDamage>().isGrounded)
				return;

			bool Allow = true;

			Events.InvokeScp106Teleport(__instance.gameObject, __instance.portalPosition, ref Allow);

			if (!Allow)
				return;

			if (__instance.iAm106 && __instance.portalPosition != Vector3.zero && !__instance.goingViaThePortal)
				Timing.RunCoroutine(__instance._DoTeleportAnimation(), Segment.Update);
		}
    }
}
