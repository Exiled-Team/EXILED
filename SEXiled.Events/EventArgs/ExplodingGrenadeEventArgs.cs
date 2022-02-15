// -----------------------------------------------------------------------
// <copyright file="ExplodingGrenadeEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
namespace SEXiled.Events.EventArgs
{
    using System;
    using System.Collections.Generic;

    using SEXiled.API.Enums;

    using SEXiled.API.Features;

    using InventorySystem.Items.ThrowableProjectiles;

    using NorthwoodLib.Pools;

    using UnityEngine;

    /// <summary>
    /// Contains all information before a grenade explodes.
    /// </summary>
    public class ExplodingGrenadeEventArgs : EventArgs
    {
        private static Dictionary<Type, GrenadeType> grenadeDictionary = new Dictionary<Type, GrenadeType>()
        {
            { typeof(FlashbangGrenade), GrenadeType.Flashbang },
            { typeof(ExplosionGrenade), GrenadeType.FragGrenade },
            { typeof(Scp018Projectile), GrenadeType.Scp018 },
            { typeof(Scp2176Projectile), GrenadeType.Scp2176 },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="ExplodingGrenadeEventArgs"/> class.
        /// </summary>
        /// <param name="thrower"><inheritdoc cref="Thrower"/></param>
        /// <param name="grenade"><inheritdoc cref="Grenade"/></param>
        /// <param name="targets"><inheritdoc cref="TargetsToAffect"/></param>
        public ExplodingGrenadeEventArgs(Player thrower, EffectGrenade grenade, Collider[] targets)
        {
            Thrower = thrower ?? Server.Host;
            GrenadeType = grenadeDictionary[grenade.GetType()];
            Grenade = grenade;
            TargetsToAffect = ListPool<Player>.Shared.Rent();
            foreach (Collider collider in targets)
            {
                if (!(Grenade is ExplosionGrenade) || !collider.TryGetComponent(out IDestructible destructible) || !ReferenceHub.TryGetHubNetID(destructible.NetworkId, out ReferenceHub hub))
                    continue;

                Player player = Player.Get(hub);
                if (player == null)
                    continue;

                if (!TargetsToAffect.Contains(player))
                    TargetsToAffect.Add(player);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExplodingGrenadeEventArgs"/> class.
        /// </summary>
        /// <param name="thrower"><inheritdoc cref="Thrower"/></param>
        /// <param name="grenade"><inheritdoc cref="Grenade"/></param>
        /// <param name="players"><inheritdoc cref="TargetsToAffect"/></param>
        public ExplodingGrenadeEventArgs(Player thrower, EffectGrenade grenade, List<Player> players)
        {
            Thrower = thrower ?? Server.Host;
            GrenadeType = grenadeDictionary[grenade.GetType()];
            Grenade = grenade;
            TargetsToAffect = ListPool<Player>.Shared.Rent();
            TargetsToAffect.AddRange(players);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ExplodingGrenadeEventArgs"/> class.
        /// </summary>
        ~ExplodingGrenadeEventArgs() => ListPool<Player>.Shared.Return(TargetsToAffect);

        /// <summary>
        /// Gets the player who thrown the grenade.
        /// </summary>
        public Player Thrower { get; }

        /// <summary>
        /// Gets the players who could be affected by the grenade, if any, and the damage that would hurt them.
        /// </summary>
        public List<Player> TargetsToAffect { get; }

        /// <summary>
        /// Gets the <see cref="API.Enums.GrenadeType"/> of the grenade.
        /// </summary>
        public GrenadeType GrenadeType { get; }

        /// <summary>
        /// Gets the grenade that is exploding.
        /// </summary>
        public EffectGrenade Grenade { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the grenade can be thrown.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}
