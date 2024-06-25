// -----------------------------------------------------------------------
// <copyright file="DoorDamage.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Map;
    using HarmonyLib;
    using Interactables.Interobjects;
    using Interactables.Interobjects.DoorUtils;
    using Mirror;
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

    /// <summary>
    ///     Patches <see cref="BreakableDoor.ServerDamage" />.
    ///     Adds the <see cref="Handlers.Map.DoorDamaging" />, <see cref="Handlers.Map.DoorDestroying" /> and <see cref="Handlers.Map.DoorDestroyed" /> events.
    /// </summary>
    [EventPatch(typeof(Handlers.Map), nameof(Handlers.Map.DoorDestroyed))]
    [EventPatch(typeof(Handlers.Map), nameof(Handlers.Map.DoorDestroying))]
    [EventPatch(typeof(Handlers.Map), nameof(Handlers.Map.DoorDamaging))]
    [HarmonyPatch(typeof(BreakableDoor), nameof(BreakableDoor.ServerDamage))]
    internal static class DoorDamage
    {
        private static bool Prefix(BreakableDoor __instance, float hp, DoorDamageType type, bool __result)
        {
            if (!NetworkServer.active)
            {
                UnityEngine.Debug.LogWarning("[Server] function 'System.Boolean Interactables.Interobjects.BreakableDoor::ServerDamage(System.Single,Interactables.Interobjects.DoorUtils.DoorDamageType)' called when server was not active");
                __result = default;
                return default;
            }

            __result = false;

            if (__instance._destroyed || __instance.Network_destroyed)
                return false;

            if (__instance._ignoredDamageSources.HasFlagFast(type))
                return false;

            if (__instance._brokenPrefab == null || __instance._objectToReplace == null)
                return false;

            DamagingDoorEventArgs damagingDoorEventArgs = new(__instance, hp, type);

            Handlers.Map.OnDoorDamaging(damagingDoorEventArgs);

            hp = damagingDoorEventArgs.DamageAmount;

            if (!damagingDoorEventArgs.IsAllowed)
                return false;

            __instance.RemainingHealth -= hp;
            if (__instance.RemainingHealth <= 0f)
            {
                DestroyingDoorEventArgs destroyingDoorEventArgs = new(__instance, damagingDoorEventArgs.DoorDamageType);

                Handlers.Map.OnDoorDestroying(destroyingDoorEventArgs);

                if (!destroyingDoorEventArgs.IsAllowed)
                {
                    __result = true;
                    return false;
                }

                __instance.Network_destroyed = true;
                DoorEvents.TriggerAction(__instance, DoorAction.Destroyed, null);

                DestroyedDoorEventArgs destroyedDoorEventArgs = new(__instance, damagingDoorEventArgs.DoorDamageType);
                Handlers.Map.OnDoorDestroyed(destroyedDoorEventArgs);
            }

            __result = true;
            return false;
        }
    }
}