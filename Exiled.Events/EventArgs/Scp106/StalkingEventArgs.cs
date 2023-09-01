// -----------------------------------------------------------------------
// <copyright file="StalkingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp106
{
    using API.Features;

    using Interfaces;

    using PlayerRoles.PlayableScps.Scp106;

    /// <summary>
    ///     Contains all information before SCP-106 use the stalk ability.
    /// </summary>
    public class StalkingEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StalkingEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="scp106StalkAbility"><inheritdoc cref="Scp106StalkAbility"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public StalkingEventArgs(Player player, Scp106StalkAbility scp106StalkAbility, bool isAllowed = true)
        {
            Player = player;
            Scp106StalkAbility = scp106StalkAbility;
            IsAllowed = isAllowed;
            Vigor = scp106StalkAbility.Vigor.CurValue;
            MinimumVigor = Scp106StalkAbility.MinVigorToSubmerge;
        }

        /// <summary>
        /// Gets the <see cref="Scp106StalkAbility"/>.
        /// </summary>
        public Scp106StalkAbility Scp106StalkAbility { get; }

        /// <summary>
        /// Gets or sets the current vigor when SCP-106 starts to stalk.
        /// </summary>
        public float Vigor { get; set; }

        /// <summary>
        /// Gets or sets the required minimum vigor to stalk.
        /// </summary>
        public float MinimumVigor { get; set; }

        /// <summary>
        /// Gets the player who's controlling SCP-106.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-106 can stalk.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}