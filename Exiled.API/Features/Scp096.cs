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
    /// Represents the general role of Scp096.
    /// </summary>
    public class Scp096
    {
        /// <summary>
        /// Gets or Sets a value indicating the max shield amount Scp096 can have.
        /// </summary>
        public static float MaxShield { get; set; } = 500;

        /// <summary>
        /// Gets a list of player ids who will be turned away from SCP-096.
        /// </summary>
        public static List<Player> TurnedPlayers { get; private set; } = new List<Player>();
    }
}
