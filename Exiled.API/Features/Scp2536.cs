// -----------------------------------------------------------------------
// <copyright file="Scp2536.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Christmas.Scp2536;
    using PlayerRoles;

    /// <summary>
    /// A wrapper for <see cref="Scp2536Controller"/>.
    /// </summary>
    public static class Scp2536
    {
        /// <summary>
        /// Gets the <see cref="Scp2536Controller"/>.
        /// </summary>
        public static Scp2536Controller Controller => Scp2536Controller.Singleton;

        /// <summary>
        /// Gets the <see cref="Scp2536GiftController"/>.
        /// </summary>
        public static Scp2536GiftController GiftController => Controller.GiftController;

        /// <summary>
        /// Gets or sets list of ignored player. They can't interact with SCP-2536.
        /// </summary>
        public static HashSet<Player> IgnoredPlayers
        {
            get => Controller._ignoredPlayers.Select(x => Player.Get(x)).ToHashSet();
            set
            {
                Controller._ignoredPlayers.Clear();

                foreach (var player in value)
                    Controller._ignoredPlayers.Add(player.NetId);
            }
        }

        /// <summary>
        /// Gets array of <see cref="Team"/> that are able to interact with SCP-2536.
        /// </summary>
        public static Team[] WhitelistedTeams => Scp2536Controller.WhitelistedTeams;

        /// <summary>
        /// Gets a value indicating whether or not SCP-2536 is hidden or not.
        /// </summary>
        public static bool IsHidden => Controller._hidden;

        /// <summary>
        /// Gets the collection of all available gifts.
        /// </summary>
        public static IReadOnlyCollection<Scp2536GiftBase> AvailableGifts => GiftController.Gifts;

        /// <summary>
        /// Gets or sets a <see cref="Dictionary{TKey,TValue}"/> with cooldowns.
        /// </summary>
        public static Dictionary<Player, Stopwatch> Cooldowns
        {
            get => GiftController._cooldowns.ToDictionary(x => Player.Get(x.Key), y => y.Value);
            set
            {
                GiftController._cooldowns.Clear();

                foreach (var kvp in value)
                    GiftController._cooldowns.Add(kvp.Key.NetId, kvp.Value);
            }
        }

        /// <summary>
        /// Gets all available <see cref="Scp2536GiftController.GiftBox"/>.
        /// </summary>
        public static IEnumerable<Scp2536GiftController.GiftBox> Boxes => GiftController.GiftBoxes;

        /// <summary>
        /// Gets a gift of provided type.
        /// </summary>
        /// <typeparam name="T">Gift type.</typeparam>
        /// <returns>Gift. Otherwise, <see langword="null"/>.</returns>
        public static T GetGift<T>()
            where T : Scp2536GiftBase => GiftController.ServerGetGift<T>();

        /// <summary>
        /// Gives random gift to player.
        /// </summary>
        /// <param name="player">Player to who give gift.</param>
        public static void GiveRandomGift(Player player) => GiftController.ServerGrantRandomGift(player.ReferenceHub);

        /// <summary>
        /// Gives specified gift to player.
        /// </summary>
        /// <param name="gift">Gift to give.</param>
        /// <param name="player">Player to who give gift.</param>
        public static void GiveGift(Scp2536GiftBase gift, Player player) => GiftController.GrantGift(gift, player.ReferenceHub);

        /// <summary>
        /// Tries to give to player a weapon as a gift.
        /// </summary>
        /// <param name="player">Player to who give gift.</param>
        /// <returns><see langword="true"/> if weapon was successfully gaven. Otherwise, <see langword="false"/>.</returns>
        public static bool TryGiveWeaponGift(Player player) => GiftController.TryGrantWeapon(player.ReferenceHub);
    }
}