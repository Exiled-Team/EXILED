// -----------------------------------------------------------------------
// <copyright file="PlacingBloodEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using API.Features;

    using Interfaces;

    using UnityEngine;

    /// <summary>
    ///     Contains all information before placing a blood decal.
    /// </summary>
    public class PlacingBloodEventArgs : IPlayerEvent, ITargetEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PlacingBloodEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="target">
        ///     <inheritdoc cref="Target" />
        /// </param>
        /// <param name="hit">
        ///     <inheritdoc cref="RaycastHit" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public PlacingBloodEventArgs(Player player, Player target, RaycastHit hit, bool isAllowed = true)
        {
            Player = player;
            Target = target;
            Position = hit.point;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc />
        public Player Player { get; }

        /// <inheritdoc />
        public Player Target { get; }

        /// <summary>
        ///     Gets or sets the blood placing position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <inheritdoc />
        public bool IsAllowed { get; set; }
    }
}