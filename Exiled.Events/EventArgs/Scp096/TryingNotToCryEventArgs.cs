// -----------------------------------------------------------------------
// <copyright file="TryingNotToCryEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp096
{
    using System.Linq;

    using API.Features;
    using Interactables.Interobjects.DoorUtils;
    using Interfaces;

    using PlayerRoles.PlayableScps.Scp096;

    using UnityEngine;

    /// <summary>
    ///     Contains all information before SCP-096 tries not to cry.
    /// </summary>
    public class TryingNotToCryEventArgs : IPlayerEvent, IDoorEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TryingNotToCryEventArgs" /> class.
        /// </summary>
        /// <param name="scp096">
        ///     <inheritdoc cref="Scp096" />
        /// </param>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public TryingNotToCryEventArgs(Scp096Role scp096, Player player, bool isAllowed = true)
        {
            Scp096 = scp096;
            Player = player;
            Door = Physics.Raycast(player.CameraTransform.position, player.CameraTransform.forward, out RaycastHit hit, 1f) ?
                Door.Get(hit.collider.gameObject) : null;
            GameObject = hit.collider.gameObject;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the SCP-096 instance.
        /// </summary>
        public Scp096Role Scp096 { get; }

        /// <summary>
        ///     Gets the player who is controlling SCP-096.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets the <see cref="API.Features.Door" /> to be cried on.
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