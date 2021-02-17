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

    using NorthwoodLib.Pools;

    using UnityEngine;

    /// <summary>
    /// Contains all informations before a grenade explodes.
    /// </summary>
    public class ExplodingGrenadeEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExplodingGrenadeEventArgs"/> class.
        /// </summary>
        /// <param name="thrower"><inheritdoc cref="Thrower"/></param>
        /// <param name="targetToDamages"><inheritdoc cref="TargetToDamages"/></param>
        /// <param name="isFrag"><inheritdoc cref="IsFrag"/></param>
        /// <param name="grenade"><inheritdoc cref="Grenade"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ExplodingGrenadeEventArgs(Player thrower, Dictionary<Player, float> targetToDamages, bool isFrag, GameObject grenade, bool isAllowed = true)
        {
            Thrower = thrower ?? Server.Host;
            TargetToDamages = targetToDamages;
            IsFrag = isFrag;
            Grenade = grenade;
            IsAllowed = isAllowed;

#pragma warning disable CS0618 // Type or member is obsolete
            Targets = ListPool<Player>.Shared.Rent(TargetToDamages.Keys);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ExplodingGrenadeEventArgs"/> class.
        /// </summary>
        ~ExplodingGrenadeEventArgs() => ListPool<Player>.Shared.Return(Targets);
#pragma warning restore CS0618 // Type or member is obsolete

        /// <summary>
        /// Gets the player who thrown the grenade.
        /// </summary>
        public Player Thrower { get; }

        /// <summary>
        /// Gets the players who could be affected by the grenade, if any, and the damage that would hurt them.
        /// </summary>
        public Dictionary<Player, float> TargetToDamages { get; }

        /// <summary>
        /// Gets the players who could be affected by the grenade, if any.
        /// </summary>
        [Obsolete("It will be changed to IEnumerable<Player>")]
        public List<Player> Targets { get; }

        /// <summary>
        /// Gets a value indicating whether the grenade is a frag or flash grenade.
        /// </summary>
        public bool IsFrag { get; }

        /// <summary>
        /// Gets the grenade that is exploding.
        /// </summary>
        public GameObject Grenade { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the grenade can be thrown.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
