// -----------------------------------------------------------------------
// <copyright file="SpawningFlamingosEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp1507
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;
    using PlayerRoles.PlayableScps.Scp1507;

    /// <summary>
    /// Contains all information before flamingos get spawned.
    /// </summary>
    public class SpawningFlamingosEventArgs : IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpawningFlamingosEventArgs"/> class.
        /// </summary>
        /// <param name="newAlpha"><inheritdoc cref="NewAlpha"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public SpawningFlamingosEventArgs(Player newAlpha, bool isAllowed = true)
        {
            NewAlpha = newAlpha;
            PlayersToSpawn = Player.Get(x => Scp1507Spawner.ValidatePlayer(x.ReferenceHub)).ToHashSet();
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets the player which is being spawned as a new alpha flamingo.
        /// </summary>
        public Player Player { get; set; }

        /// <summary>
        /// Gets or sets a list of players that will be spawned as a flamingo.
        /// </summary>
        public HashSet<Player> PlayersToSpawn { get; set; }

        /// <inheritdoc />
        public bool IsAllowed { get; set; }
    }
}