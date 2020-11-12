// -----------------------------------------------------------------------
// <copyright file="Cassie.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using MEC;

    using Respawning;

    /// <summary>
    /// A set of tools to use in-game C.A.S.S.I.E more easily.
    /// </summary>
    public static class Cassie
    {
        /// <summary>
        /// Gets a value indicating whether or not a C.A.S.S.I.E announcement is currently playing. Does not include decontamination messages.
        /// </summary>
        public static bool IsSpeaking => !NineTailedFoxAnnouncer.singleton.Free;

        /// <summary>
        /// Reproduce a C.A.S.S.I.E message.
        /// </summary>
        /// <param name="message">The message to be reproduced.</param>
        /// <param name="isHeld">Indicates whether C.A.S.S.I.E has to hold the message.</param>
        /// <param name="isNoisy">Indicates whether C.A.S.S.I.E has to make noises or not during the message.</param>
        public static void Message(string message, bool isHeld = false, bool isNoisy = true) => RespawnEffectsController.PlayCassieAnnouncement(message, isHeld, isNoisy);

        /// <summary>
        /// Reproduce a C.A.S.S.I.E message after a certain amount of seconds.
        /// </summary>
        /// <param name="message">The message to be reproduced.</param>
        /// <param name="delay">The seconds that have to pass before reproducing the message.</param>
        /// <param name="isHeld">Indicates whether C.A.S.S.I.E has to hold the message.</param>
        /// <param name="isNoisy">Indicates whether C.A.S.S.I.E has to make noises or not during the message.</param>
        public static void DelayedMessage(string message, float delay, bool isHeld = false, bool isNoisy = true)
        {
            Timing.CallDelayed(delay, () => RespawnEffectsController.PlayCassieAnnouncement(message, isHeld, isNoisy));
        }
    }
}
