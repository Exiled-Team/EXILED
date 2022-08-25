// -----------------------------------------------------------------------
// <copyright file="GainingExperienceEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp079
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    ///     Contains all information before SCP-079 gains experience.
    /// </summary>
    public class GainingExperienceEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="GainingExperienceEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="gainType">
        ///     <inheritdoc cref="GainType" />
        /// </param>
        /// <param name="amount">
        ///     <inheritdoc cref="Amount" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public GainingExperienceEventArgs(Player player, ExpGainType gainType, float amount, bool isAllowed = true)
        {
            Player = player;
            GainType = gainType;
            Amount = amount;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the experience gain type.
        /// </summary>
        public ExpGainType GainType { get; }

        /// <summary>
        ///     Gets or sets the amount of experience to be gained.
        /// </summary>
        public float Amount { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the experience is successfully granted.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets the player who's controlling SCP-079.
        /// </summary>
        public Player Player { get; }
    }
}
