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
    using Exiled.Events.EventArgs.Scp330;
    using InventorySystem.Items.Usables.Scp330;
    using MEC;

    /// <summary>
    /// THe custom candy base class.
    /// </summary>
    public abstract class CustomCandy : CustomItem
    {
        private readonly Dictionary<ushort, List<int>> trackedSerials = new();

        /// <inheritdoc/>
        /// <exception cref="NotSupportedException">Throws when trying to set value to property.</exception>
        public override ItemType Type
        {
            get => ItemType.SCP330;
            set => throw new NotSupportedException("Cannot set ItemType for CustomCandy. To set candy use CandyType.");
        }

        /// <summary>
        /// Gets or sets <see cref="CandyKindID"/> of custom candy.
        /// </summary>
        public abstract CandyKindID CandyType { get; set; }

        /// <inheritdoc/>
        public override bool Check(Item? item) => item != null && trackedSerials.ContainsKey(item!.Serial);

        /// <inheritdoc/>
        public override bool Check(Pickup? pickup) => pickup != null && trackedSerials.ContainsKey(pickup!.Serial);

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

        private void OnInternalUsingScp330(InteractingScp330EventArgs ev)
        {
            if (ev.Candy == CandyType)
            {
                Timing.CallDelayed(.5f, () =>
                {
                    if (!ev.Player.TryGetItem(ItemType.SCP330, out Item item) || !item.Is(out Scp330 scp330))
                        return;

                    if (trackedSerials.ContainsKey(scp330.Serial))
                        trackedSerials[scp330.Serial].Add(scp330.Candies.Count - 1);
                    else
                        trackedSerials.Add(scp330.Serial, new List<int> { 0 });
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