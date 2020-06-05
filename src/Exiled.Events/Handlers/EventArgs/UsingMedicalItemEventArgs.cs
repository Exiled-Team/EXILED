// -----------------------------------------------------------------------
// <copyright file="UsingMedicalItemEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.EventArgs
{
    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before a player uses a medical item.
    /// </summary>
    public class UsingMedicalItemEventArgs : UsedMedicalItemEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsingMedicalItemEventArgs"/> class.
        /// </summary>
        /// <param name="player">The player who's going to use the medical item.</param>
        /// <param name="item">The medical item to be used.</param>
        /// <param name="cooldown"><inheritdoc cref="Cooldown"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public UsingMedicalItemEventArgs(Player player, ItemType item, float cooldown, bool isAllowed = true)
            : base(player, item)
        {
            Cooldown = cooldown;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets the medical item cooldown.
        /// </summary>
        public float Cooldown { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}