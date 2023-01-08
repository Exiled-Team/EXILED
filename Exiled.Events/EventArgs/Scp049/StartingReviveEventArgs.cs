// -----------------------------------------------------------------------
// <copyright file="StartingReviveEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp049
{
    using API.Features;
    using Interfaces;
    using PlayerRoles.PlayableScps.Scp049;

    /// <summary>
    ///     Contains all information before SCP-049 begins recalling a player.
    /// </summary>
    public class StartingReviveEventArgs : IPlayerEvent, IDeniableEvent, IRagdollEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartingReviveEventArgs"/> class.
        /// </summary>
        /// <param name="target">  <inheritdoc cref="StartingReviveEventArgs.Target"/></param>
        /// <param name="scp049">  <inheritdoc cref="StartingReviveEventArgs.Player"/></param>
        /// <param name="ragdoll">  <inheritdoc cref="StartingReviveEventArgs.Ragdoll"/></param>
        /// <param name="resurrectError">  <inheritdoc cref="StartingReviveEventArgs.RessurectError"/></param>
        /// <param name="isAllowed"> <inheritdoc cref="StartingReviveEventArgs.IsAllowed"/></param>
        public StartingReviveEventArgs(Player target, Player scp049, BasicRagdoll ragdoll,  Scp049ResurrectAbility.ResurrectError resurrectError,  bool isAllowed = true)
        {
            Target = target;
            Player = scp049;
            Ragdoll = Ragdoll.Get(ragdoll);
            RessurectError = resurrectError;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets server processing of revive based on <see cref="Scp049ResurrectAbility.ResurrectError"/>.
        /// </summary>
        public Scp049ResurrectAbility.ResurrectError RessurectError { get; set; }

        /// <summary>
        ///     Gets the player who's getting recalled.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether the recall can begin.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets the player who is controlling SCP-049.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets the Ragdoll who's getting recalled.
        /// </summary>
        public Ragdoll Ragdoll { get; }
    }
}