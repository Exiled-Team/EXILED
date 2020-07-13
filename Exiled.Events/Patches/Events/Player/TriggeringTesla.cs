// -----------------------------------------------------------------------
// <copyright file="TriggeringTesla.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
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
    internal class TriggeringTesla
    {
        private static bool Prefix(TeslaGateController __instance)
        {
            foreach (KeyValuePair<GameObject, ReferenceHub> allHub in ReferenceHub.GetAllHubs())
            {
                if (allHub.Value.characterClassManager.CurClass == RoleType.Spectator)
                    continue;
                foreach (TeslaGate teslaGate in __instance.TeslaGates)
                {
                    if (!teslaGate.PlayerInRange(allHub.Value) || teslaGate.InProgress)
                        continue;

                    var ev = new TriggeringTeslaEventArgs(API.Features.Player.Get(allHub.Key), teslaGate.PlayerInHurtRange(allHub.Key));
                    Player.OnTriggeringTesla(ev);

                    if (ev.IsTriggerable)
                        teslaGate.ServerSideCode();
                }
            }

            return false;
        }
    }
}
