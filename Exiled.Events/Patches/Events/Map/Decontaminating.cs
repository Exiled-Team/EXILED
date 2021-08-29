// -----------------------------------------------------------------------
// <copyright file="Decontaminating.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

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
            DecontaminatingEventArgs ev = new DecontaminatingEventArgs();

            Map.OnDecontaminating(ev);

            return ev.IsAllowed;
        }
    }
}
