using Exiled.Events.Attributes;
using Exiled.Events.EventArgs.Door;
using Exiled.Events.EventArgs.Map;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using HarmonyLib;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using Mirror;
using System.Diagnostics;
using UnityEngine;

namespace Exiled.Events.Patches.Events.Map
{
    public class ServerDamage
    {

        /// <summary>
        ///     Patches <see cref="BreakableDoor.ServerDamage" />.
        ///     Adds the <see cref="Handlers.Door.DoorDamaging" />, <see cref="Handlers.Door.DoorDestroying" /> and <see cref="Handlers.Door.DoorDestroyed" /> events.
        /// </summary>
        [EventPatch(typeof(Handlers.Map), nameof(Handlers.Map.DoorDestroyed))]
        [EventPatch(typeof(Handlers.Map), nameof(Handlers.Map.DoorDestroying))]
        [EventPatch(typeof(Handlers.Map), nameof(Handlers.Map.DoorDamaging))]
        [HarmonyPatch(typeof(BreakableDoor), nameof(BreakableDoor.ServerDamage))]
        internal class BreakableDoorServerDamage
        {
            private static bool Prefix(BreakableDoor __instance, float Health, DoorDamageType type, bool __result)
            {
                if (!NetworkServer.active)
                {
                    Debug.LogWarning("[Server] function 'System.Boolean Interactables.Interobjects.BreakableDoor::ServerDamage(System.Single,Interactables.Interobjects.DoorUtils.DoorDamageType)' called when server was not active");
                    __result = default;
                    return default;
                }

                if (__instance._destroyed || __instance.Network_destroyed)
                {
                    __result = false;
                    return false;
                }

                if (__instance._ignoredDamageSources.HasFlagFast(type))
                {
                    __result = false;
                    return false;
                }

                if (__instance._brokenPrefab == null || __instance._objectToReplace == null)
                {
                    __result = false;
                    return false;
                }

                DamagingDoorEventArgs damagingDoorEventArgs = new(__instance, Health, type);

                Handlers.Map.OnDoorDamaging(damagingDoorEventArgs);

                Health = damagingDoorEventArgs.Health;
                type = damagingDoorEventArgs.DoorDamageType;

                if (!damagingDoorEventArgs.IsAllowed)
                {
                    __result = false;
                    return false;
                }

                __instance.RemainingHealth -= Health;
                if (__instance.RemainingHealth <= 0f)
                {
                    DestroyingDoorEventArgs destroyingDoorEventArgs = new(__instance);

                    Handlers.Map.OnDoorDestroying(destroyingDoorEventArgs);

                    if (!destroyingDoorEventArgs.IsAllowed)
                    {
                        __instance.RemainingHealth += Health;
                        __result = false;
                        return false;
                    }

                    __instance.Network_destroyed = true;
                    DoorEvents.TriggerAction(__instance, DoorAction.Destroyed, null);
                    DestroyedDoorEventArgs destroyedDoorEventArgs = new(__instance);

                    Handlers.Map.OnDoorDestroyed(destroyedDoorEventArgs);
                }

                __result = true;
                return false;
            }
        }
    }
}
