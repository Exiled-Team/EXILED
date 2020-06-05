// -----------------------------------------------------------------------
// <copyright file="CalmingDownScp096EventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.EventArgs
{
    using Exiled.API.Features;
    using PlayableScps;

    /// <summary>
    /// Contains all informations before SCP-096 calms down.
    /// </summary>
    public class CalmingDownScp096EventArgs : EnragingScp096EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CalmingDownScp096EventArgs"/> class.
        /// </summary>
        /// <param name="scp096">The <see cref="Scp096"/> instance.</param>
        /// <param name="player">The player who's controlling SCP-096.</param>
        /// <param name="isAllowed">Indicates whether the event can be executed or not.</param>
        public CalmingDownScp096EventArgs(Scp096 scp096, Player player, bool isAllowed = true)
            : base(scp096, player, isAllowed)
        {
        }
    }
}