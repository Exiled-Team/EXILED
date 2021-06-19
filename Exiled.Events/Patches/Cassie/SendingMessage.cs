// -----------------------------------------------------------------------
// <copyright file="SendingMessage.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Cassie
{
    using System;
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;
    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="Respawning.RespawnEffectsController.PlayCassieAnnouncement(string, bool, bool)"/>.
    /// Adds the <see cref="Cassie.SendingMessage"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Respawning.RespawnEffectsController), nameof(Respawning.RespawnEffectsController.PlayCassieAnnouncement))]
    internal static class SendingMessage
    {
        private static bool Prefix(ref string words, ref bool makeHold, ref bool makeNoise)
        {
            var ev = new SendingMessageEventArgs(words, makeHold, makeNoise);
            Handlers.Cassie.OnSendingMessage(ev);

            words = ev.Words;
            makeHold = ev.MakeHold;
            makeNoise = ev.MakeNoise;

            return ev.IsAllowed;
        }
    }
}
