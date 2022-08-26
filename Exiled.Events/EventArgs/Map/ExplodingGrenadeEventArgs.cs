// -----------------------------------------------------------------------
// <copyright file="ExplodingGrenadeEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using System.Collections.Generic;

    using Exiled.API.Features;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Pickups.Projectiles;
    using Exiled.Events.EventArgs.Interfaces;

    using InventorySystem.Items.ThrowableProjectiles;

    using NorthwoodLib.Pools;

    using UnityEngine;

    /// <summary>
    ///     Contains all information before a grenade explodes.
    /// </summary>
    public class ExplodingGrenadeEventArgs : IPlayerEvent, IDeniableEvent, IProjectileEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ExplodingGrenadeEventArgs" /> class.
        /// </summary>
        /// <param name="thrower">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="grenade">
        ///     <inheritdoc cref="Projectile" />
        /// </param>
        /// <param name="targets">
        ///     <inheritdoc cref="TargetsToAffect" />
        /// </param>
        public ExplodingGrenadeEventArgs(Player thrower, EffectGrenade grenade, Collider[] targets)
        {
            Player = thrower ?? Server.Host;
            Projectile = (Projectile)Pickup.Get(grenade);
            Position = Projectile.Position;
            TargetsToAffect = ListPool<Player>.Shared.Rent();
            foreach (Collider collider in targets)
            {
                if (Projectile.Base is not ExplosionGrenade || !collider.TryGetComponent(out IDestructible destructible) || !ReferenceHub.TryGetHubNetID(destructible.NetworkId, out ReferenceHub hub))
                    continue;

                Player player = Player.Get(hub);
                if (player is null)
                    continue;

                if (!TargetsToAffect.Contains(player))
                    TargetsToAffect.Add(player);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExplodingGrenadeEventArgs"/> class.
        /// </summary>
        /// <param name="thrower"><inheritdoc cref="Player"/></param>
        /// <param name="position"><inheritdoc cref="Position"/></param>
        /// <param name="grenade"><inheritdoc cref="Projectile"/></param>
        /// <param name="targets"><inheritdoc cref="TargetsToAffect"/></param>
        public ExplodingGrenadeEventArgs(Player thrower, Vector3 position, EffectGrenade grenade, Collider[] targets)
        {
            Player = thrower ?? Server.Host;
            Projectile = (Projectile)Pickup.Get(grenade);
            Position = position;
            TargetsToAffect = ListPool<Player>.Shared.Rent();
            foreach (Collider collider in targets)
            {
                if (Projectile.Base is not ExplosionGrenade || !collider.TryGetComponent(out IDestructible destructible) || !ReferenceHub.TryGetHubNetID(destructible.NetworkId, out ReferenceHub hub))
                    continue;

                Player player = Player.Get(hub);
                if (player is null)
                    continue;

                if (!TargetsToAffect.Contains(player))
                    TargetsToAffect.Add(player);
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExplodingGrenadeEventArgs" /> class.
        /// </summary>
        /// <param name="thrower">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="grenade">
        ///     <inheritdoc cref="Projectile" />
        /// </param>
        /// <param name="players">
        ///     <inheritdoc cref="TargetsToAffect" />
        /// </param>
        public ExplodingGrenadeEventArgs(Player thrower, EffectGrenade grenade, List<Player> players)
        {
            Player = thrower ?? Server.Host;
            Projectile = (Projectile)Pickup.Get(grenade);
            Position = Projectile.Position;
            TargetsToAffect = ListPool<Player>.Shared.Rent(players);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ExplodingGrenadeEventArgs"/> class.
        /// </summary>
        ~ExplodingGrenadeEventArgs() => ListPool<Player>.Shared.Return(TargetsToAffect);

        /// <summary>
        /// Gets the position where is exploding.
        /// </summary>
        public Vector3 Position { get; }

        /// <summary>
        ///     Gets the players who could be affected by the grenade, if any, and the damage that would hurt them.
        /// </summary>
        public List<Player> TargetsToAffect { get; }

        /// <summary>
        /// Gets the grenade that is exploding.
        /// </summary>
        public Projectile Projectile { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the grenade can be thrown.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        ///     Gets the player who thrown the grenade.
        /// </summary>
        public Player Player { get; }
    }
}
