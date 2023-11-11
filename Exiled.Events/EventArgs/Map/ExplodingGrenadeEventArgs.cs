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
    using Exiled.API.Features.Pools;
    using Exiled.Events.EventArgs.Interfaces;
    using Exiled.Events.Patches.Generic;

    using Footprinting;

    using InventorySystem.Items.ThrowableProjectiles;

    using UnityEngine;

    /// <summary>
    ///     Contains all information before a grenade explodes.
    /// </summary>
    public class ExplodingGrenadeEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExplodingGrenadeEventArgs"/> class.
        /// </summary>
        /// <param name="thrower"><inheritdoc cref="Player"/></param>
        /// <param name="position"><inheritdoc cref="Position"/></param>
        /// <param name="grenade"><inheritdoc cref="Projectile"/></param>
        /// <param name="targets"><inheritdoc cref="TargetsToAffect"/></param>
        public ExplodingGrenadeEventArgs(Footprint thrower, Vector3 position, ExplosionGrenade grenade, Collider[] targets)
        {
            Player = Player.Get(thrower.Hub);
            Projectile = (EffectGrenadeProjectile)Pickup.Get(grenade);
            Position = position;
            TargetsToAffect = ListPool<Player>.Pool.Get();

            if (Projectile.Base is not ExplosionGrenade)
                return;

            foreach (Collider collider in targets)
            {
                if (!collider.TryGetComponent(out IDestructible destructible) || !ReferenceHub.TryGetHubNetID(destructible.NetworkId, out ReferenceHub hub))
                    continue;

                Player player = Player.Get(hub);
                if (player is null)
                    continue;

                switch (Player is null)
                {
                    case false:
                        {
                            if (Server.FriendlyFire || IndividualFriendlyFire.CheckFriendlyFirePlayer(thrower, hub))
                            {
                                if (!TargetsToAffect.Contains(player))
                                    TargetsToAffect.Add(player);
                            }
                        }

                        break;
                    case true:
                        {
                            if (Server.FriendlyFire || thrower.Hub == Server.Host.ReferenceHub || HitboxIdentity.CheckFriendlyFire(thrower.Role, hub.roleManager.CurrentRole.RoleTypeId))
                            {
                                if (!TargetsToAffect.Contains(player))
                                    TargetsToAffect.Add(player);
                            }
                        }

                        break;
                }
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
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public ExplodingGrenadeEventArgs(Player thrower, EffectGrenade grenade, bool isAllowed = true)
        {
            Player = thrower ?? Server.Host;
            Projectile = (EffectGrenadeProjectile)Pickup.Get(grenade);
            Position = Projectile.Position;
            TargetsToAffect = ListPool<Player>.Pool.Get(Player.List);
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ExplodingGrenadeEventArgs"/> class.
        /// </summary>
        ~ExplodingGrenadeEventArgs()
        {
            ListPool<Player>.Pool.Return(TargetsToAffect);
        }

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
        public EffectGrenadeProjectile Projectile { get; }

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