// -----------------------------------------------------------------------
// <copyright file="PlacingBloodEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    using UnityEngine;

    /// <summary>
    ///     Contains all information before placing a blood decal.
    /// </summary>
    public class PlacingBloodEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PlacingBloodEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="position">
        ///     <inheritdoc cref="Position" />
        /// </param>
        /// <param name="type">
        ///     <inheritdoc cref="Type" />
        /// </param>
        /// <param name="multiplier">
        ///     <inheritdoc cref="Multiplier" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public PlacingBloodEventArgs(Player player, Vector3 position, int type, float multiplier, bool isAllowed = true)
        {
            Player = player;
            Position = position;
            Type = (BloodType) type;
            Multiplier = multiplier;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets or sets the blood placing position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        ///     Gets or sets the blood type.
        /// </summary>
        public BloodType Type { get; set; }

        /// <summary>
        ///     Gets or sets the blood multiplier.
        /// </summary>
        public float Multiplier { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the blood can be placed.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets the player who's placing the blood.
        /// </summary>
        public Player Player { get; }
    }
}
