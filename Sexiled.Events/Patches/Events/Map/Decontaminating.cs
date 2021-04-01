// -----------------------------------------------------------------------
// <copyright file="Decontaminating.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Sexiled.Events.EventArgs;
using Sexiled.Events.Handlers;

namespace Sexiled.Events.Patches.Events.Map
{
    using Sexiled.Events.EventArgs;
    using Sexiled.Events.Handlers;

    using HarmonyLib;

    using LightContainmentZoneDecontamination;

    /// <summary>
    /// Patches <see cref="DecontaminationController.FinishDecontamination"/>.
    /// Adds the <see cref="Map.Decontaminating"/> event.
    /// </summary>
    [HarmonyPatch(typeof(DecontaminationController), nameof(DecontaminationController.FinishDecontamination))]
    internal static class Decontaminating
    {
        private static bool Prefix()
        {
            var ev = new DecontaminatingEventArgs();

            Handlers.Map.OnDecontaminating(ev);

            return ev.IsAllowed;
        }
    }
}
