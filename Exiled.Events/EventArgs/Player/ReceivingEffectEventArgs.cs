// -----------------------------------------------------------------------
// <copyright file="ReceivingEffectEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;

    using CustomPlayerEffects;

    using Interfaces;

    /// <summary>
    /// Contains all information before a player receives a <see cref="StatusEffectBase" />.
    /// </summary>
    public class ReceivingEffectEventArgs : IPlayerEvent, IDeniableEvent
    {
        private byte intensity;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivingEffectEventArgs" /> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="effect"><inheritdoc cref="Effect"/></param>
        /// <param name="intensity">The intensity the effect is being changed to.</param>
        /// <param name="currentIntensity"><inheritdoc cref="CurrentIntensity"/></param>
        /// <param name="duration"><inheritdoc cref="Duration"/></param>
        public ReceivingEffectEventArgs(Player player, StatusEffectBase effect, byte intensity, byte currentIntensity, float duration)
        {
            Player = player;
            Effect = effect;
            this.intensity = intensity;
            CurrentIntensity = currentIntensity;
            Duration = duration;
        }

        /// <summary>
        /// Gets the <see cref="Player" /> receiving the effect.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="StatusEffectBase" /> being received.
        /// </summary>
        public StatusEffectBase Effect { get; }

        /// <summary>
        /// Gets or sets a value indicating how long the effect will last. If its value is 0, then it doesn't always reflect the real effect duration.
        /// </summary>
        public float Duration { get; set; } = 0;

        /// <summary>
        /// Gets or sets the value of the new intensity of the effect. Setting this to <c>0</c> is the same as setting IsAllowed to
        /// <see langword="false" />.
        /// </summary>
        public byte Intensity
        {
            get => intensity;
            set
            {
                intensity = value;

                if (intensity == 0)
                    IsAllowed = false;
            }
        }

        /// <summary>
        /// Gets the value of the intensity of this effect on the player.
        /// </summary>
        public byte CurrentIntensity { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the effect will be applied.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}