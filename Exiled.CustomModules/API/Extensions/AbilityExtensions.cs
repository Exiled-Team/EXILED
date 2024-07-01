// -----------------------------------------------------------------------
// <copyright file="AbilityExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.CustomModules.API.Features;
    using Exiled.CustomModules.API.Features.CustomAbilities;
    using Exiled.CustomModules.API.Features.ItemAbilities;
    using Exiled.CustomModules.API.Features.PickupAbilities;
    using Exiled.CustomModules.API.Features.PlayerAbilities;

    /// <summary>
    /// A set of extensions for <see cref="CustomAbility{T}"/>.
    /// </summary>
    public static class AbilityExtensions
    {
        /// <summary>
        /// Gets the <see cref="CustomAbility{TEntity}"/> associated with the specified <see cref="GameEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the game entity.</typeparam>
        /// <param name="entity">The game entity.</param>
        /// <returns>The <see cref="CustomAbility{TEntity}"/> if found; otherwise, null.</returns>
        public static CustomAbility<TEntity> Get<TEntity>(this GameEntity entity)
            where TEntity : GameEntity
        {
            if (!CustomAbility<GameEntity>.Manager.TryGetValue(entity, out HashSet<CustomAbility<GameEntity>> abilities))
                return null;

            foreach (CustomAbility<GameEntity> ability in abilities)
            {
                if (ability is CustomAbility<TEntity> typedAbility)
                    return typedAbility;
            }

            return null;
        }

        /// <summary>
        /// Gets the <typeparamref name="TAbility"/> associated with the specified <see cref="Player"/>.
        /// </summary>
        /// <typeparam name="TAbility">The type of the player ability.</typeparam>
        /// <param name="player">The player.</param>
        /// <returns>The <typeparamref name="TAbility"/> if found; otherwise, null.</returns>
        public static TAbility Get<TAbility>(this Player player)
            where TAbility : PlayerAbility =>
            player.As<Pawn>().TryGetCustomAbility(out TAbility ability) ? ability : null;

        /// <summary>
        /// Gets the <typeparamref name="TAbility"/> associated with the specified <see cref="Item"/>.
        /// </summary>
        /// <typeparam name="TAbility">The type of the item ability.</typeparam>
        /// <param name="item">The item.</param>
        /// <returns>The <typeparamref name="TAbility"/> if found; otherwise, null.</returns>
        public static TAbility Get<TAbility>(this Item item)
            where TAbility : ItemAbility =>
            CustomAbility<Item>.TryGet(item, out IEnumerable<CustomAbility<Item>> abilities) ?
                abilities.FirstOrDefault(a => a is TAbility) as TAbility : null;

        /// <summary>
        /// Gets the <typeparamref name="TAbility"/> associated with the specified <see cref="Pickup"/>.
        /// </summary>
        /// <typeparam name="TAbility">The type of the pickup ability.</typeparam>
        /// <param name="pickup">The pickup.</param>
        /// <returns>The <typeparamref name="TAbility"/> if found; otherwise, null.</returns>
        public static TAbility Get<TAbility>(this Pickup pickup)
            where TAbility : PickupAbility =>
            CustomAbility<Pickup>.TryGet(pickup, out IEnumerable<CustomAbility<Pickup>> abilities) ?
                abilities.FirstOrDefault(a => a is TAbility) as TAbility : null;

        /// <summary>
        /// Tries to get the <see cref="CustomAbility{TEntity}"/> associated with the specified <see cref="GameEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the game entity.</typeparam>
        /// <param name="entity">The game entity.</param>
        /// <param name="ability">When this method returns, contains the <see cref="CustomAbility{TEntity}"/>, if found; otherwise, null.</param>
        /// <returns><c>true</c> if the <see cref="CustomAbility{TEntity}"/> is found; otherwise, <c>false</c>.</returns>
        public static bool TryGet<TEntity>(this GameEntity entity, out CustomAbility<TEntity> ability)
            where TEntity : GameEntity
        {
            ability = null;

            if (CustomAbility<GameEntity>.EntitiesValue.TryGetValue(entity, out HashSet<CustomAbility<GameEntity>> abilities))
                ability = abilities.FirstOrDefault(a => a is CustomAbility<TEntity>) as CustomAbility<TEntity>;

            return ability != null;
        }

        /// <summary>
        /// Tries to get the <typeparamref name="TAbility"/> associated with the specified <see cref="Player"/>.
        /// </summary>
        /// <typeparam name="TAbility">The type of the player ability.</typeparam>
        /// <param name="player">The player.</param>
        /// <param name="playerAbility">When this method returns, contains the <typeparamref name="TAbility"/>, if found; otherwise, null.</param>
        /// <returns><c>true</c> if the <typeparamref name="TAbility"/> is found; otherwise, <c>false</c>.</returns>
        public static bool TryGet<TAbility>(this Player player, out TAbility playerAbility)
            where TAbility : PlayerAbility
        {
            playerAbility = null;
            if (!CustomAbility<Player>.TryGet(player, out IEnumerable<CustomAbility<Player>> abilities))
                return false;

            playerAbility = abilities.FirstOrDefault(a => a is TAbility) as TAbility;
            return playerAbility != null;
        }

        /// <summary>
        /// Tries to get the <typeparamref name="TAbility"/> associated with the specified <see cref="Item"/>.
        /// </summary>
        /// <typeparam name="TAbility">The type of the item ability.</typeparam>
        /// <param name="item">The item.</param>
        /// <param name="itemAbility">When this method returns, contains the <typeparamref name="TAbility"/>, if found; otherwise, null.</param>
        /// <returns><c>true</c> if the <typeparamref name="TAbility"/> is found; otherwise, <c>false</c>.</returns>
        public static bool TryGet<TAbility>(this Item item, out TAbility itemAbility)
            where TAbility : ItemAbility
        {
            itemAbility = null;
            if (!CustomAbility<Item>.TryGet(item, out IEnumerable<CustomAbility<Item>> abilities))
                return false;

            itemAbility = abilities.FirstOrDefault(a => a is TAbility) as TAbility;
            return itemAbility != null;
        }

        /// <summary>
        /// Tries to get the <typeparamref name="TAbility"/> associated with the specified <see cref="Pickup"/>.
        /// </summary>
        /// <typeparam name="TAbility">The type of the pickup ability.</typeparam>
        /// <param name="pickup">The pickup.</param>
        /// <param name="pickupAbility">When this method returns, contains the <typeparamref name="TAbility"/>, if found; otherwise, null.</param>
        /// <returns><c>true</c> if the <typeparamref name="TAbility"/> is found; otherwise, <c>false</c>.</returns>
        public static bool TryGet<TAbility>(this Pickup pickup, out TAbility pickupAbility)
            where TAbility : PickupAbility
        {
            pickupAbility = null;
            if (!CustomAbility<Pickup>.TryGet(pickup, out IEnumerable<CustomAbility<Pickup>> abilities))
                return false;

            pickupAbility = abilities.FirstOrDefault(a => a is TAbility) as TAbility;
            return pickupAbility != null;
        }
    }
}
