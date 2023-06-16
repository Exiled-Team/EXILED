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
            GameObject = Physics.Raycast(player.CameraTransform.position, player.CameraTransform.forward, out RaycastHit hit, 1f) ?
                        hit.collider.gameObject : null;
            Door = Door.Get(GameObject);
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the SCP-096 instance.
        /// </summary>
        public Scp096Role Scp096 { get; }

        /// <inheritdoc />
        public Player Player { get; }

        /// <inheritdoc />
        /// <remarks>the value can be null.</remarks>
        public Door Door { get; }

        /// <summary>
        ///     Gets the <see cref="UnityEngine.GameObject" /> to be cried on.
        /// </summary>
        public GameObject GameObject { get; }

        /// <inheritdoc />
        public bool IsAllowed { get; set; }
    }
}