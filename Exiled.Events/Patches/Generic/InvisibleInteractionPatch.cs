namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;

    using Exiled.API.Enums;
    using Exiled.API.Features;

    using HarmonyLib;

    using Interactables;
    using Interactables.Verification;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="StandardDistanceVerification.ServerCanInteract(ReferenceHub, Interactables.InteractableCollider)"/>.
    /// Implements <see cref="Player.IsInvisibleAbsolute"/> property logic.
    /// </summary>
    [HarmonyPatch(typeof(StandardDistanceVerification), nameof(StandardDistanceVerification.ServerCanInteract))]
    internal static class InvisibleInteractionPatch
    {
        private static bool Prefix(StandardDistanceVerification __instance, ReferenceHub hub, InteractableCollider collider, ref bool __result)
        {
            if (!__instance._allowHandcuffed && !PlayerInteract.CanDisarmedInteract && hub.interCoordinator.Handcuffed)
            {
                __result = false;

                return false;
            }

            if (Vector3.Distance(hub.playerMovementSync.RealModelPosition, collider.transform.position + collider.transform.TransformDirection(collider.VerificationOffset)) < __instance._maxDistance * 1.4f)
            {
                __result = true;

                if (!Player.TryGet(hub, out Player player) || player.IsInvisibleAbsolute)
                    return false;

                player.DisableEffect(EffectType.Invisible);
            }

            return false;
        }
    }
}
