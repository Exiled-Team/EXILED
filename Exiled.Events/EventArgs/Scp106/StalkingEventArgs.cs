// -----------------------------------------------------------------------
// <copyright file="StalkingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp106
{
    using System;

    using API.Features;
    using Interfaces;
    using PlayerRoles.PlayableScps.Scp106;

    using Scp106Role = API.Features.Roles.Scp106Role;

    /// <summary>
    ///     Contains all information before SCP-106 uses the stalk ability.
    /// </summary>
    public class StalkingEventArgs : IScp106Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StalkingEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public StalkingEventArgs(Player player, bool isAllowed = true)
        {
            Player = player;
            Scp106 = player.Role.As<Scp106Role>();
            IsAllowed = isAllowed;
            MinimumVigor = Scp106StalkAbility.MinVigorToSubmerge;
        }

        /// <summary>
        /// Gets the <see cref="Scp106StalkAbility"/>.
        /// </summary>
        [Obsolete("Use Scp106.StalkAbility instead of this")]
        public Scp106StalkAbility Scp106StalkAbility => Scp106.StalkAbility;

        /// <summary>
        /// Gets or sets the current vigor when SCP-106 starts to stalk.
        /// </summary>
        public float Vigor
        {
            [Obsolete("Use Scp106.Vigor instead of this")]
            get => Scp106.Vigor;
            [Obsolete("Use Scp106.Vigor instead of this")]
            set => Scp106.Vigor = value;
        }

        /// <summary>
        /// Gets or sets the required minimum vigor to stalk.
        /// </summary>
        public float MinimumVigor { get; set; }

        /// <summary>
        /// Gets the player who's controlling SCP-106.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp106Role Scp106 { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-106 can stalk.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}