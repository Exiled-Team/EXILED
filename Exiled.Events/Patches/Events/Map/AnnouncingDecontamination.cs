// -----------------------------------------------------------------------
// <copyright file="AnnouncingDecontamination.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
#pragma warning disable SA1313
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using LightContainmentZoneDecontamination;

    /// <summary>
    /// Patches <see cref="DecontaminationController.UpdateSpeaker"/>.
    /// Adds the <see cref="Map.AnnouncingDecontamination"/> event.
    /// </summary>
    [HarmonyPatch(typeof(DecontaminationController), nameof(DecontaminationController.UpdateSpeaker))]
    internal static class AnnouncingDecontamination
    {
        private static bool Prefix(DecontaminationController __instance, ref bool hard)
        {
            var ev = new AnnouncingDecontaminationEventArgs(__instance._nextPhase, hard);

            Map.OnAnnouncingDecontamination(ev);

            hard = ev.IsGlobal;
            __instance._nextPhase = ev.Id;

            return ev.IsAllowed;
        }
    }
}
