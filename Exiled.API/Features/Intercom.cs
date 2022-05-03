// -----------------------------------------------------------------------
// <copyright file="Intercom.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    /// <summary>
    /// A set of tools to easily handle the Intercom.
    /// </summary>
    public static class Intercom
    {
        /// <summary>
        /// Gets or sets the text displayed on the intercom screen.
        /// </summary>
        public static string DisplayText
        {
            get => global::Intercom.host.CustomContent;
            set => global::Intercom.host.CustomContent = value;
        }

        /// <summary>
        /// Gets or sets the current state of the intercom.
        /// </summary>
        public static global::Intercom.State State
        {
            get => global::Intercom.host.IntercomState;
            set => global::Intercom.host.IntercomState = value;
        }

        /// <summary>
        /// Gets a value indicating whether or not the intercom is currently being used.
        /// </summary>
        public static bool InUse => State == global::Intercom.State.Transmitting || State == global::Intercom.State.TransmittingBypass || State == global::Intercom.State.AdminSpeaking;

        /// <summary>
        /// Gets the <see cref="Player"/> that is using the intercom.<br>Will be <see langword="null"/> if <see cref="InUse"/> is <see langword="false"/>.</br>
        /// </summary>
        public static Player Speaker => !InUse ? null : Player.Get(global::Intercom.host.speaker);

        /// <summary>
        /// Gets or sets the remaining cooldown of the intercom.
        /// </summary>
        public static float RemainingCooldown
        {
            get => global::Intercom.host.remainingCooldown;
            set => global::Intercom.host.remainingCooldown = value;
        }

        /// <summary>
        /// Gets or sets the remaining speech time of the intercom.
        /// </summary>
        public static float SpeechRemainingTime
        {
            get => !InUse ? 0f : global::Intercom.host.speechRemainingTime;
            set => global::Intercom.host.speechRemainingTime = value;
        }

        /// <summary>
        /// Plays the intercom's sound.
        /// </summary>
        /// <param name="start">Sets a value indicating whether or not the sound is the intercom's start speaking sound.</param>
        /// <param name="transmitterId">Sets the transmitterId.</param>
        public static void PlaySound(bool start, int transmitterId = 0) => global::Intercom.host.RpcPlaySound(start, transmitterId);

        /// <summary>
        /// Reset the intercom's cooldown.
        /// </summary>
        public static void Reset() => RemainingCooldown = -1f;
    }
}
