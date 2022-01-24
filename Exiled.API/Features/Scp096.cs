// -----------------------------------------------------------------------
// <copyright file="Scp096.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System.Collections.Generic;

    /// <summary>
    /// A set of tools to modify SCP-096's behaviour.
    /// </summary>
    public static class Scp096
    {
        /// <summary>
        /// Gets a list of player ids who will be turned away from SCP-096. Turned players will not enrage SCP-096.
        /// </summary>
        /// <example>
        /// The following example prevents Class-D at the start of the round from triggering SCP-096.
        /// <code language="cs">
        /// // Executes at the start of a round.
        /// public void OnStartingRound()
        /// {
        ///     foreach (Player player in Player.List)
        ///     {
        ///         if (player.Role == RoleType.ClassD)
        ///         {
        ///             Scp096.TurnedPlayers.Add(player)
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        public static HashSet<Player> TurnedPlayers { get; } = new HashSet<Player>(20);
    }
}
