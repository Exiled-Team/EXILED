// -----------------------------------------------------------------------
// <copyright file="Candy.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using InventorySystem.Items.Usables.Scp330;

    /// <summary>
    /// A Wrapper class for <see cref="Scp330Bag"/>.
    /// </summary>
    public class Candy
    {
        /// <summary>
        /// Give an random Candy to the player.
        /// </summary>
        /// <param name="player">Player to give a random candy to.</param>
        public static void GiveRandomCandy(Player player)
        {
            int randomCandyType = UnityEngine.Random.Range(1, 8);

            Scp330Bag bag;
            if (!Scp330Bag.TryGetBag(player.ReferenceHub, out bag))
                return;

            bag.TryAddSpecific((CandyKindID)randomCandyType);
            bag.ServerRefreshBag();
        }

        /// <summary>
        /// Give a specific Candy to the player.
        /// </summary>
        /// <param name="player">Player to give the candy to.</param>
        /// <param name="candy"><see cref="CandyKindID"/>.</param>
        public static void GiveCandy(Player player, CandyKindID candy)
        {
            Scp330Bag bag;
            if (!Scp330Bag.TryGetBag(player.ReferenceHub, out bag))
                return;

            bag.TryAddSpecific(candy);
            bag.ServerRefreshBag();
        }

        /// <summary>
        /// Gives an Scp-330 Bag to the player.
        /// </summary>
        /// <param name="player">Player to give a bag to.</param>
        public static void GiveBag(Player player)
        {
            Scp330Bag bag;
            if (Scp330Bag.TryGetBag(player.ReferenceHub, out bag))
                return;
            player.AddItem(ItemType.SCP330);
        }
    }
}
