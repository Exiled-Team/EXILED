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
        /// Gets or sets the content of the intercom.
        /// </summary>
        public static string Content
        {
            get => global::Intercom.host.CustomContent;
            set => global::Intercom.host.CustomContent = value;
        }

        /// <summary>
        /// Gets the current state of the intercom.
        /// </summary>
        public static global::Intercom.State IntercomState => global::Intercom.host.IntercomState;

        /// <summary>
        /// Gets a value indicating whether or not the intercom is currently being used.
        /// </summary>
        public static bool InUse => IntercomState == global::Intercom.State.Transmitting || IntercomState == global::Intercom.State.TransmittingBypass || IntercomState == global::Intercom.State.AdminSpeaking;

        /// <summary>
        /// Gets the <see cref="Player"/> that is using the intercom. Will be <see langword="null"/> if <see cref="InUse"/> is <see langword="false"/>.
        /// </summary>
        public static Player Speaker => Player.Get(global::Intercom.host.speaker);

        /// <summary>
        /// Gets or sets the remaining speech time of the intercom.
        /// </summary>
        public static float SpeechRemainingTime
        {
            get => global::Intercom.host.speechRemainingTime;
            set => global::Intercom.host.speechRemainingTime = value;
        }

        /// <summary>
        /// Plays the intercom's sound.
        /// </summary>
        /// <param name="start">Sets a value indicating whether or not the sound is the intercom's start speaking sound.</param>
        /// <param name="transmitterId">Sets the transmitterId.</param>
        public static void PlayIntercomSound(bool start, int transmitterId = 0) => global::Intercom.host.RpcPlaySound(start, transmitterId);
    }
}
