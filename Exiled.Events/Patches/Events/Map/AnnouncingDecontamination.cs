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
    internal class AnnouncingDecontamination
    {
        /// <summary>
        /// Gets or sets a value indicating whether stops the Announcement Event from triggering.
        /// Prevents an issue where the event is constantly called after Decon occurs.
        /// </summary>
        internal static bool StopAnnouncing { get; set; }

        private static bool Prefix(DecontaminationController __instance, bool hard)
        {
            if (StopAnnouncing)
                return true;

            var ev = new AnnouncingDecontaminationEventArgs(__instance._nextPhase, hard);

            Map.OnAnnouncingDecontamination(ev);

            __instance._nextPhase = ev.Id;

            StopAnnouncing = ev.Id == 5;

            return ev.IsAllowed;
        }
    }
}
