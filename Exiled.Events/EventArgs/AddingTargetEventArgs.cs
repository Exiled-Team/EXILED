// -----------------------------------------------------------------------
// <copyright file="AddingTargetEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs {
    using System;

    using Exiled.API.Features;

    using UnityEngine;

    using Scp096 = PlayableScps.Scp096;

    /// <summary>
    /// Contains all informations before adding a target to SCP-096.
    /// </summary>
    public class AddingTargetEventArgs : EventArgs {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddingTargetEventArgs"/> class.
        /// </summary>
        /// <param name="scp096"><inheritdoc cref="Scp096"/></param>
        /// <param name="target"><inheritdoc cref="Target"/></param>
        /// <param name="enrageTimeToAdd"><inheritdoc cref="EnrageTimeToAdd"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public AddingTargetEventArgs(Player scp096, Player target, float enrageTimeToAdd, bool isAllowed = true) {
            Scp096 = scp096;
            Target = target;
            EnrageTimeToAdd = scp096.CurrentScp is Scp096 scp
                ? scp.AddedTimeThisRage + enrageTimeToAdd <= PlayableScps.Scp096.MaximumAddedEnrageTime
                    ? enrageTimeToAdd
                    : Mathf.Abs((scp.AddedTimeThisRage + enrageTimeToAdd) - PlayableScps.Scp096.MaximumAddedEnrageTime)
                : 0f;
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
        /// Gets or sets how much time is added to SCP-096's enrage timer if <see cref="IsAllowed"/> is true.
        /// </summary>
        /// <remarks>This does not affect anything if he doesn't already have any targets before this event is called.</remarks>
        public float EnrageTimeToAdd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the target is allowed to be added.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
