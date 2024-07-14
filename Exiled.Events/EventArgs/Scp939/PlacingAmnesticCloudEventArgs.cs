// -----------------------------------------------------------------------
// <copyright file="PlacingAmnesticCloudEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp939
{
    using API.Features;
    using Exiled.API.Features.Roles;
    using Interfaces;

    /// <summary>
    /// Contains all information before SCP-939 uses its amnestic cloud ability.
    /// </summary>
    public class PlacingAmnesticCloudEventArgs : IScp939Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlacingAmnesticCloudEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="state">
        /// Whether SCP-939 is attempting to place an amnestic cloud.
        /// </param>
        /// <param name="isReady">
        /// Whether the cooldown is ready.
        /// </param>
        /// <param name="cooldown">
        /// SCP-939's amnestic cloud cooldown.
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public PlacingAmnesticCloudEventArgs(Player player, bool state, bool isReady, float cooldown, bool isAllowed = true)
        {
            Player = player;
            Scp939 = Player.Role.As<Scp939Role>();
            State = state;
            IsReady = isReady;
            Cooldown = cooldown;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets a value indicating whether SCP-939 can place an amnestic cloud.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets a value indicating whether SCP-939 is ready to place its amnestic cloud.
        /// </summary>
        public bool State { get; }

        /// <summary>
        /// Gets or sets a value indicating whether SCP-939's amnestic cloud cooldown is ready.
        /// </summary>
        public bool IsReady { get; set; }

        /// <summary>
        /// Gets or sets a value indicating SCP-939's amnestic cloud cooldown.
        /// </summary>
        public float Cooldown { get; set; }

        /// <summary>
        /// Gets the player who's controlling SCP-939.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp939Role Scp939 { get; }
    }
}