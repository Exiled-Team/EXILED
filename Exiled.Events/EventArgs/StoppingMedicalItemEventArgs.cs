// -----------------------------------------------------------------------
// <copyright file="StoppingMedicalItemEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before a player cancels using a medical item.
    /// </summary>
    public class StoppingMedicalItemEventArgs : UsingMedicalItemEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoppingMedicalItemEventArgs"/> class.
        /// </summary>
        /// <param name="player">The player who's stopping the use of the medical item.</param>
        /// <param name="item">The medical item that won't be consumed.</param>
        /// <param name="cooldown">The cooldown left for completing the use of the medical item.</param>
        /// <param name="isAllowed"><inheritdoc cref="UsingMedicalItemEventArgs.IsAllowed"/></param>
        public StoppingMedicalItemEventArgs(Player player, ItemType item, float cooldown, bool isAllowed = true)
            : base(player, item, cooldown, isAllowed)
        {
        }

        /// <summary>
        /// Gets the medical item cooldown.
        /// </summary>
        public new float Cooldown => base.Cooldown;
    }
}
