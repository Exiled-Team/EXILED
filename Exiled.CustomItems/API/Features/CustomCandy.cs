// -----------------------------------------------------------------------
// <copyright file="CustomCandy.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Spawn;
    using Exiled.Events.EventArgs.Scp330;
    using InventorySystem.Items.Usables.Scp330;
    using MEC;

    /// <summary>
    /// THe custom candy base class.
    /// </summary>
    public abstract class CustomCandy : CustomItem
    {
        private readonly Dictionary<ushort, List<int>> trackedSerials = new();

        private float weight = 100;

        /// <inheritdoc/>
        /// <exception cref="NotSupportedException">Throws when trying to set value to property.</exception>
        public override ItemType Type
        {
            get => ItemType.SCP330;
            set => throw new NotSupportedException("Cannot set ItemType for CustomCandy. To set candy use CandyType.");
        }

        /// <summary>
        /// Gets or sets a chance that candy will become custom.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Throws when <see langword="value"/> is less that 0 or above 100.</exception>
        public override float Weight
        {
            get => weight;
            set
            {
                if (value is > 100 or < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Value must be above zero and less that 100.");

                weight = value;
            }
        }

        /// <inheritdoc/>
        /// <exception cref="NotSupportedException">Throws when you try to access getter or setter.</exception>
        public override SpawnProperties? SpawnProperties
        {
            get => throw new NotSupportedException("Cannot get a SpawnProperties of a CustomCandy.");
            set => throw new NotSupportedException("Cannot set a SpawnProperties of a CustomCandy.");
        }

        /// <summary>
        /// Gets or sets <see cref="CandyKindID"/> of custom candy.
        /// </summary>
        public abstract CandyKindID CandyType { get; set; }

        /// <inheritdoc/>
        public override bool Check(Item? item) => item != null && trackedSerials.ContainsKey(item!.Serial);

        /// <inheritdoc/>
        public override bool Check(Pickup? pickup) => pickup != null && trackedSerials.ContainsKey(pickup!.Serial);

        /// <inheritdoc/>
        public override void Give(Player player, bool displayMessage = true)
        {
            player.TryAddCandy(CandyType);

            if (player.TryGetItem(ItemType.SCP330, out Item item) && item.Is(out Scp330 scp330))
            {
                if (!trackedSerials.ContainsKey(scp330.Serial))
                    trackedSerials.Add(scp330.Serial, new List<int>() { scp330.Candies.Count - 1 });
                else
                    trackedSerials[scp330.Serial].Add(scp330.Candies.Count - 1);
            }
        }

        /// <inheritdoc/>
        public override void Give(Player player, Pickup pickup, bool displayMessage = true)
        {
            if (pickup.Is(out Exiled.API.Features.Pickups.Scp330Pickup scp330Pickup))
            {
                Give(player, displayMessage);

                foreach (var candy in scp330Pickup.Candies)
                    player.TryAddCandy(candy);
            }
        }

        /// <summary>
        /// Checks the specified inventory item to see if it is a custom item.
        /// </summary>
        /// <param name="scp330">Bag to check.</param>
        /// <param name="candyKindID">Candy type to find.</param>
        /// <returns><see langword="true"/> if any candy from <see cref="Scp330"/> is custom. Otherwise, <see langword="false"/>.</returns>
        public bool Check(Scp330? scp330, CandyKindID candyKindID) => scp330 != null && trackedSerials.TryGetValue(scp330.Serial, out List<int> candies) && candies.Contains(scp330.Candies.ToList().IndexOf(candyKindID));

        /// <summary>
        /// Applies an effects to player who eaten a <see cref="CustomCandy"/>.
        /// </summary>
        /// <param name="player">Player who eaten <see cref="CustomCandy"/>.</param>
        protected abstract void ApplyEffect(Player player);

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            Exiled.Events.Handlers.Scp330.InteractingScp330 += OnInternalUsingScp330;
            Exiled.Events.Handlers.Scp330.EatingScp330 += OnInternalUsingCandy;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            Exiled.Events.Handlers.Scp330.InteractingScp330 -= OnInternalUsingScp330;
            Exiled.Events.Handlers.Scp330.EatingScp330 -= OnInternalUsingCandy;
        }

        /// <inheritdoc cref="OnAcquired(Player, Item, bool)"/>
        protected virtual void OnAcquired(Player player, Scp330 scp330, CandyKindID candyKindID, bool displayMessage)
        {
        }

        /// <inheritdoc/>
        protected override void OnAcquired(Player player, Item item, bool displayMessage)
        {
            if (item.Is(out Scp330 scp330))
            {
                OnAcquired(player, scp330, scp330.Candies.Last(), displayMessage);
            }
        }

        private void OnInternalUsingScp330(InteractingScp330EventArgs ev)
        {
            if (ev.Candy == CandyType)
            {
                Timing.CallDelayed(.5f, () =>
                {
                    if (ev.Player.TryGetItem(ItemType.SCP330, out Item item) && item.Is(out Scp330 scp330))
                    {
                        if (!trackedSerials.ContainsKey(scp330.Serial))
                            trackedSerials.Add(scp330.Serial, new List<int>() { scp330.Candies.Count - 1 });
                        else
                            trackedSerials[scp330.Serial].Add(scp330.Candies.Count - 1);

                        OnAcquired(ev.Player, scp330, ev.Candy, true);
                    }
                });
            }
        }

        private void OnInternalUsingCandy(EatingScp330EventArgs ev)
        {
            if (Check(ev.Scp330Bag, ev.Candy.Kind))
            {
                ev.IsAllowed = false;
                trackedSerials[ev.Scp330Bag.Serial].Remove(ev.Scp330Bag.Candies.ToList().IndexOf(ev.Candy.Kind));
                ApplyEffect(ev.Player);
            }
        }
    }
}