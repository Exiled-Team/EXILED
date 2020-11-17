// -----------------------------------------------------------------------
// <copyright file="DeactivatingWorkstationEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before a player deactivates a workstation.
    /// </summary>
    public class DeactivatingWorkstationEventArgs : ActivatingWorkstationEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeactivatingWorkstationEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="station"><inheritdoc cref="Permissions"/></param>
        /// <param name="isAllowed">Indicates whether the event can be executed or not.</param>
        public DeactivatingWorkstationEventArgs(Player player, WorkStation station, bool isAllowed = true)
            : base(player, station, isAllowed)
        {
        }
    }
}
