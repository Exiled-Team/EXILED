// -----------------------------------------------------------------------
// <copyright file="TryingNotToCry.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp096
{
#pragma warning disable SA1313
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using Interactables.Interobjects.DoorUtils;

    using PlayableScps;

    using UnityEngine;

    /// <summary>
    /// Patches the <see cref="Scp096.TryNotToCry"/> method.
    /// Adds the <see cref="Handlers.Scp096.TryingNotToCry"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp096), nameof(Scp096.TryNotToCry))]
    internal static class TryingNotToCry
    {
        private static readonly int DoorMask = LayerMask.GetMask("Door");

        private static bool Prefix(Scp096 __instance)
        {
            if (!Physics.Raycast(__instance.Hub.PlayerCameraReference.position, __instance.Hub.PlayerCameraReference.forward, out RaycastHit hitInfo, 1f, DoorMask))
                return false;

            DoorVariant componentInParent = hitInfo.collider.gameObject.GetComponentInParent<DoorVariant>();
            if (componentInParent == null || componentInParent.GetExactState() > 0.0 || Scp096._takenDoors.ContainsKey(__instance.Hub.gameObject))
                return false;

            var ev = new TryingNotToCryEventArgs(__instance, API.Features.Player.Get(__instance.Hub), componentInParent);

            Handlers.Scp096.OnTryingNotToCry(ev);

            if (ev.IsAllowed)
            {
                Scp096._takenDoors.Add(__instance.Hub.gameObject, componentInParent);
                __instance.PlayerState = Scp096PlayerState.TryNotToCry;
            }

            return false;
        }
    }
}
