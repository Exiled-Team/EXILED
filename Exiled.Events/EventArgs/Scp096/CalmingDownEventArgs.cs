// -----------------------------------------------------------------------
// <copyright file="CalmingDownEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp096
{
    using Exiled.API.Features;

    using Scp096 = PlayableScps.Scp096;

    /// <summary>
    ///     Contains all information before SCP-096 calms down.
    /// </summary>
    public class CalmingDownEventArgs : EnragingEventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CalmingDownEventArgs" /> class.
        /// </summary>
        /// <param name="scp096">The <see cref="PlayableScps.Scp096" /> instance.</param>
        /// <param name="player">The player who's controlling SCP-096.</param>
        /// <param name="isAllowed">Indicates whether or not SCP-096 can calm down.</param>
        public CalmingDownEventArgs(Scp096 scp096, Player player, bool isAllowed = true)
            : base(scp096, player, isAllowed)
        {
        }
    }
}
