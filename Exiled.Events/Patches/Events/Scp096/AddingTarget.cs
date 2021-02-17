// -----------------------------------------------------------------------
// <copyright file="AddingTarget.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp096
{
#pragma warning disable SA1313
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using Mirror;

    using PlayableScps.Messages;

    using UnityEngine;

    using Scp096 = PlayableScps.Scp096;

    /// <summary>
    /// Patches <see cref="Scp096.AddTarget"/>.
    /// Adds the <see cref="Handlers.Scp096.AddingTarget"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp096), nameof(Scp096.AddTarget))]
    internal static class AddingTarget
    {
        private static bool Prefix(Scp096 __instance, GameObject target)
        {
            ReferenceHub hub = ReferenceHub.GetHub(target);
            if (!__instance.CanReceiveTargets || hub == null || __instance._targets.Contains(hub))
                return false;
            API.Features.Player scp096 = API.Features.Player.Get(__instance.Hub.gameObject);
            API.Features.Player targetPlayer = API.Features.Player.Get(target);
            if (scp096 == null)
            {
                Log.Error("SCP-096.AddTarget: Could not get SCP-096 player object.");
                return true;
            }

            if (targetPlayer == null)
            {
                Log.Error("SCP-096.AddTarget: Could not get Target player object.");
                return true;
            }

            AddingTargetEventArgs ev = new AddingTargetEventArgs(scp096, targetPlayer, 70, __instance.EnrageTimePerReset);
            Exiled.Events.Handlers.Scp096.OnAddingTarget(ev);
            if (ev.IsAllowed)
            {
                if (!__instance._targets.IsEmpty() || __instance.Enraged)
                {
                    if (__instance.AddedTimeThisRage + ev.EnrageTimeToAdd >= __instance.MaximumAddedEnrageTime)
                        ev.EnrageTimeToAdd = 0f;
                    else if (__instance.AddedTimeThisRage + ev.EnrageTimeToAdd > __instance.MaximumAddedEnrageTime)
                        ev.EnrageTimeToAdd = __instance.AddedTimeThisRage + ev.EnrageTimeToAdd - __instance.MaximumAddedEnrageTime;
                    __instance.EnrageTimeLeft += ev.EnrageTimeToAdd;
                    __instance.AddedTimeThisRage += ev.EnrageTimeToAdd;
                }

                __instance._targets.Add(hub);
                __instance.AdjustShield(ev.AhpToAdd);
                NetworkServer.SendToClientOfPlayer<Scp096ToTargetMessage>(hub.characterClassManager.netIdentity, new Scp096ToTargetMessage(hub));
            }

            return false;
        }
    }
}
