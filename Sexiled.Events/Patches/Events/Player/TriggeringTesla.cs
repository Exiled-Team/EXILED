// -----------------------------------------------------------------------
// <copyright file="TriggeringTesla.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Sexiled.API.Features;
using Sexiled.Events.EventArgs;

namespace Sexiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using System;
    using System.Collections.Generic;

    using Sexiled.Events.EventArgs;
    using Sexiled.Events.Handlers;

    using HarmonyLib;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="TeslaGateController.FixedUpdate"/>.
    /// Adds the <see cref="Handlers.Player.TriggeringTesla"/> event.
    /// </summary>
    [HarmonyPatch(typeof(TeslaGateController), nameof(TeslaGateController.FixedUpdate))]
    internal static class TriggeringTesla
    {
        private static bool Prefix(TeslaGateController __instance)
        {
            try
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
                        Handlers.Player.OnTriggeringTesla(ev);

                        if (ev.IsTriggerable)
                            teslaGate.ServerSideCode();
                    }
                }

                return false;
            }
            catch (Exception e)
            {
                Log.Error($"Sexiled.Events.Patches.Events.Player.TriggeringTesla: {e}\n{e.StackTrace}");

                return true;
            }
        }
    }
}
