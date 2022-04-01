// -----------------------------------------------------------------------
// <copyright file="TriggeringTesla.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player {
#pragma warning disable SA1313
    using System;
    using System.Collections.Generic;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="TeslaGateController.FixedUpdate"/>.
    /// Adds the <see cref="Player.TriggeringTesla"/> event.
    /// </summary>
    [HarmonyPatch(typeof(TeslaGateController), nameof(TeslaGateController.FixedUpdate))]
    internal static class TriggeringTesla {
        private static bool Prefix(TeslaGateController __instance) {
            try {
                foreach (TeslaGate teslaGate in __instance.TeslaGates) {
                    if (!teslaGate.isActiveAndEnabled || teslaGate.InProgress)
                        continue;

                    if (teslaGate.NetworkInactiveTime > 0f) {
                        teslaGate.NetworkInactiveTime = Mathf.Max(0f, teslaGate.InactiveTime - Time.fixedDeltaTime);
                        continue;
                    }

                    bool inIdleRange = false;
                    bool isTriggerable = false;
                    foreach (KeyValuePair<GameObject, ReferenceHub> allHub in ReferenceHub.GetAllHubs()) {
                        if (allHub.Value.isDedicatedServer || allHub.Value.characterClassManager.CurClass == RoleType.Spectator)
                            continue;

                        if (teslaGate.PlayerInIdleRange(allHub.Value)) {
                            TriggeringTeslaEventArgs ev = new TriggeringTeslaEventArgs(API.Features.Player.Get(allHub.Key), teslaGate, teslaGate.PlayerInHurtRange(allHub.Key), teslaGate.PlayerInRange(allHub.Value));
                            Player.OnTriggeringTesla(ev);

                            if (ev.IsTriggerable && !isTriggerable)
                                isTriggerable = ev.IsTriggerable;

                            if (!inIdleRange)
                                inIdleRange = ev.IsInIdleRange;
                        }
                    }

                    if (isTriggerable)
                        teslaGate.ServerSideCode();

                    if (inIdleRange != teslaGate.isIdling)
                        teslaGate.ServerSideIdle(inIdleRange);
                }

                return false;
            }
            catch (Exception e) {
                API.Features.Log.Error($"Exiled.Events.Patches.Events.Player.TriggeringTesla: {e}\n{e.StackTrace}");

                return true;
            }
        }
    }
}
