// -----------------------------------------------------------------------
// <copyright file="AnnouncingDecontamination.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Sexiled.Events.EventArgs;
using Sexiled.Events.Handlers;

namespace Sexiled.Events.Patches.Events.Map
{
#pragma warning disable SA1313
    using Sexiled.Events.EventArgs;
    using Sexiled.Events.Handlers;

    using HarmonyLib;

    using LightContainmentZoneDecontamination;

    /// <summary>
    /// Patches <see cref="DecontaminationController.UpdateSpeaker"/>.
    /// Adds the <see cref="Map.AnnouncingDecontamination"/> event.
    /// </summary>
    [HarmonyPatch(typeof(DecontaminationController), nameof(DecontaminationController.UpdateSpeaker))]
    internal static class AnnouncingDecontamination
    {
        private static void Prefix(DecontaminationController __instance, ref bool hard)
        {
            var ev = new AnnouncingDecontaminationEventArgs(__instance._nextPhase, hard);

            Handlers.Map.OnAnnouncingDecontamination(ev);
        }
    }
}
