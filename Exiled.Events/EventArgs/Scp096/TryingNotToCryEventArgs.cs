// -----------------------------------------------------------------------
// <copyright file="TryingNotToCryEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp096
{
    using API.Features;
    using Exiled.API.Features.Doors;
    using Interfaces;
    using UnityEngine;

    using Scp096Role = API.Features.Roles.Scp096Role;

    /// <summary>
    ///     Contains all information before SCP-096 tries not to cry.
    /// </summary>
    public class TryingNotToCryEventArgs : IScp096Event, IDoorEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TryingNotToCryEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public TryingNotToCryEventArgs(Player player, bool isAllowed = true)
        {
            Player = player;
            Scp096 = player.Role.As<Scp096Role>();
            GameObject = Physics.Raycast(player.CameraTransform.position, player.CameraTransform.forward, out RaycastHit hit, 1f) ?
                        hit.collider.gameObject : null;
            Door = Door.Get(GameObject);
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public Scp096Role Scp096 { get; }

        /// <summary>
        ///     Gets the player who is controlling SCP-096.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets the <see cref="API.Features.Doors.Door" /> to be cried on.
        ///     <remarks>the value can be null</remarks>
        /// </summary>
        public Door Door { get; }

        /// <summary>
        ///     Gets the <see cref="UnityEngine.GameObject" /> to be cried on.
        /// </summary>
        public GameObject GameObject { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not SCP-096 can try not to cry.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}