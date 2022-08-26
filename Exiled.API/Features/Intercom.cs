// -----------------------------------------------------------------------
// <copyright file="Intercom.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using UnityEngine;

    using BaseIntercom = global::Intercom;

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
            get => BaseIntercom.host.CustomContent;
            set => BaseIntercom.host.CustomContent = value;
        }

        /// <summary>
        /// Gets or sets the current state of the intercom.
        /// </summary>
        public static BaseIntercom.State State
        {
            get => BaseIntercom.host.IntercomState;
            set => BaseIntercom.host.IntercomState = value;
        }

        /// <summary>
        /// Gets the intercom's <see cref="UnityEngine.GameObject"/>.
        /// </summary>
        public static GameObject GameObject
        {
            get => BaseIntercom.host.gameObject;
        }

        /// <summary>
        /// Gets the intercom's <see cref="UnityEngine.Transform"/>.
        /// </summary>
        public static Transform Transform
        {
            get => BaseIntercom.host.transform;
        }

        /// <summary>
        /// Gets a value indicating whether or not the intercom is currently being used.
        /// </summary>
        public static bool InUse
        {
            get => State is BaseIntercom.State.Transmitting or BaseIntercom.State.TransmittingBypass or BaseIntercom.State.AdminSpeaking;
        }

        /// <summary>
        /// Gets the <see cref="Player"/> that is using the intercom.
        /// </summary>
        /// <remarks>Will be <see langword="null"/> if <see cref="InUse"/> is <see langword="false"/>.</remarks>
        public static Player Speaker
        {
            get => !InUse ? null : Player.Get(BaseIntercom.host.speaker);
        }

        /// <summary>
        /// Gets or sets the remaining cooldown of the intercom.
        /// </summary>
        public static float RemainingCooldown
        {
            get => BaseIntercom.host.remainingCooldown;
            set => BaseIntercom.host.remainingCooldown = value;
        }

        /// <summary>
        /// Gets or sets the remaining speech time of the intercom.
        /// </summary>
        public static float SpeechRemainingTime
        {
            get => !InUse ? 0f : BaseIntercom.host.speechRemainingTime;
            set => BaseIntercom.host.speechRemainingTime = value;
        }

        /// <summary>
        /// Plays the intercom's sound.
        /// </summary>
        /// <param name="start">Sets a value indicating whether or not the sound is the intercom's start speaking sound.</param>
        /// <param name="transmitterId">Sets the transmitterId.</param>
        public static void PlaySound(bool start, int transmitterId = 0) => BaseIntercom.host.RpcPlaySound(start, transmitterId);

        /// <summary>
        /// Reset the intercom's cooldown.
        /// </summary>
        public static void Reset() => RemainingCooldown = -1f;

        /// <summary>
        /// Times out the intercom.
        /// </summary>
        public static void Timeout()
        {
            if (InUse)
            {
                SpeechRemainingTime = -1f;
            }
        }
    }
}