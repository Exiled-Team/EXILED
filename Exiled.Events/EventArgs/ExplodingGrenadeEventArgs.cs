// -----------------------------------------------------------------------
// <copyright file="ExplodingGrenadeEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
namespace Exiled.Events.EventArgs
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Features;

    using UnityEngine;

    /// <summary>
    /// Contains all informations before a grenade explodes.
    /// </summary>
    public class ExplodingGrenadeEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExplodingGrenadeEventArgs"/> class.
        /// </summary>
        /// <param name="targets"><inheritdoc cref="Targets"/></param>
        /// <param name="isFrag"><inheritdoc cref="IsFrag"/></param>
        /// <param name="grenade"><inheritdoc cref="Grenade"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ExplodingGrenadeEventArgs(List<Player> targets, bool isFrag, GameObject grenade, bool isAllowed = true)
        {
            Targets = targets;
            IsFrag = isFrag;
            Grenade = grenade;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the players who will be affected by the grenade (if any).
        /// </summary>
        public List<Player> Targets { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed or not.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets a value indicating whether the grenade is a frag or flash grenade.
        /// </summary>
        public bool IsFrag { get; }

        /// <summary>
        /// Gets the grenade that is exploding.
        /// </summary>
        public GameObject Grenade { get; }
    }
}
