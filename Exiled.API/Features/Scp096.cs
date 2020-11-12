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
        /// Gets or Sets a value indicating the max shield amount SCP-096 can have during his docile state.
        /// </summary>
        public static float MaxShield { get; set; } = 350;

        /// <summary>
        /// Gets a list of player ids who will be turned away from SCP-096.
        /// </summary>
        public static HashSet<Player> TurnedPlayers { get; } = new HashSet<Player>(20);
    }
}
