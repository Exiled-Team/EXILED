// -----------------------------------------------------------------------
// <copyright file="AddingTargetEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before adding a target to SCP-096.
    /// </summary>
    public class AddingTargetEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddingTargetEventArgs"/> class.
        /// </summary>
        /// <param name="scp096"><inheritdoc cref="Scp096"/></param>
        /// <param name="target"><inheritdoc cref="Target"/></param>
        /// <param name="ahpToAdd"><inheritdoc cref="AhpToAdd"/></param>
        /// <param name="enrageTimeToAdd"><inheritdoc cref="EnrageTimeToAdd"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public AddingTargetEventArgs(Player scp096, Player target, int ahpToAdd, float enrageTimeToAdd, bool isAllowed = true)
        {
            Scp096 = scp096;
            Target = target;
            AhpToAdd = ahpToAdd;
            EnrageTimeToAdd = enrageTimeToAdd;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="Player"/> that is controlling SCP-096.
        /// </summary>
        public Player Scp096 { get; }

        /// <summary>
        /// Gets the <see cref="Player"/> being added as a target.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets or sets the amount of AHP to add to SCP-096 if <see cref="IsAllowed"/> is true.
        /// </summary>
        public int AhpToAdd { get; set; }

        /// <summary>
        /// Gets or sets how much time is added to SCP-096's enrage timer if <see cref="IsAllowed"/> is true.
        /// Note: This does not affect anything if he doesn't already have any targets before this event is called.
        /// </summary>
        public float EnrageTimeToAdd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the target is allowed to be added.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
