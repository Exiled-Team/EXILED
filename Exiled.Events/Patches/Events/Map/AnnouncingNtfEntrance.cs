// -----------------------------------------------------------------------
// <copyright file="AnnouncingNtfEntrance.cs" company="Exiled Team">
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

    /// <summary>
    /// Patch the <see cref="NineTailedFoxAnnouncer.AnnounceNtfEntrance(int, int, char)"/>.
    /// Adds the <see cref="Map.AnnouncingNtfEntrance"/> event.
    /// </summary>
    [HarmonyPatch(typeof(NineTailedFoxAnnouncer), nameof(NineTailedFoxAnnouncer.AnnounceNtfEntrance))]
    public class AnnouncingNtfEntrance
    {
        /// <summary>
        /// Prefix of <see cref="NineTailedFoxAnnouncer.AnnounceNtfEntrance(int, int, char)"/>.
        /// </summary>
        /// <param name="_scpsLeft"><inheritdoc cref="AnnouncingNtfEntranceEventArgs.ScpsLeft"/></param>
        /// <param name="_mtfNumber"><inheritdoc cref="AnnouncingNtfEntranceEventArgs.NtfNumber"/></param>
        /// <param name="_mtfLetter"><inheritdoc cref="AnnouncingNtfEntranceEventArgs.NtfLetter"/></param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(ref int _scpsLeft, ref int _mtfNumber, ref char _mtfLetter)
        {
            var ev = new AnnouncingNtfEntranceEventArgs(_scpsLeft, _mtfNumber, _mtfLetter);

            Map.OnAnnouncingNtfEntrance(ev);

            return ev.IsAllowed;
        }
    }
}
