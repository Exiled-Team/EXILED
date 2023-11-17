using Exiled.Events.Attributes;
using Exiled.Events.EventArgs.Door;

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
        [EventPatch(typeof(Handlers.Door), nameof(Handlers.Door.DoorDestroyed))]
        [EventPatch(typeof(Handlers.Door), nameof(Handlers.Door.DoorDestroying))]
        [EventPatch(typeof(Handlers.Door), nameof(Handlers.Door.DoorDamaging))]
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

                DoorDamagingEventArgs doorDamagingEventArgs = new(__instance, Health, type);

                Handlers.Door.OnDoorDamaging(doorDamagingEventArgs);

                Health = doorDamagingEventArgs.Health;
                type = doorDamagingEventArgs.DoorDamageType;

                if (!doorDamagingEventArgs.IsAllowed)
                {
                    __result = false;
                    return false;
                }

                __instance.RemainingHealth -= Health;
                if (__instance.RemainingHealth <= 0f)
                {
                    DoorDestroyingEventArgs doorDestroyingEventArgs = new(__instance);

                    Handlers.Door.OnDoorDestroying(doorDestroyingEventArgs);

                    if (!doorDestroyingEventArgs.IsAllowed)
                    {
                        __instance.RemainingHealth += Health;
                        __result = false;
                        return false;
                    }

                    __instance.Network_destroyed = true;
                    DoorEvents.TriggerAction(__instance, DoorAction.Destroyed, null);
                    DoorDestroyedEventArgs doorDestroyedEventArgs = new(__instance);

                    Handlers.Door.OnDoorDestroyed(doorDestroyedEventArgs);
                }

                __result = true;
                return false;
            }
        }
    }
}
