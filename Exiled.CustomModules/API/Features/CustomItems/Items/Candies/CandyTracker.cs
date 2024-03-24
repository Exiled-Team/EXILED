// -----------------------------------------------------------------------
// <copyright file="CandyTracker.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomItems.Items.Candies
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Scp330;
    using UnityEngine;

    /// <summary>
    /// A custom tracker for candies.
    /// </summary>
    public class CandyTracker : ItemTracker
    {
        /// <inheritdoc cref="Exiled.Events.Handlers.Scp330.OnInteractingScp330"/>
        internal void OnInteractingScp330(InteractingScp330EventArgs ev)
        {
            if (!ev.Player.TryGetItems(x => x.Type == ItemType.SCP330, out IEnumerable<Item> items) ||
                !items.Single().Is(out Scp330 scp330))
                return;

            if (!scp330.TryGetComponent(out CandyBehaviour behaviour))
            {
                behaviour = scp330.AddComponent<CandyBehaviour>();

                if (ev.Candy != behaviour.CandySettings.CandyType || Random.value * 100 <= behaviour.CandySettings.Weight)
                {
                    scp330.RemoveComponent<CandyBehaviour>();
                    return;
                }

                AddOrTrack(scp330);
                behaviour.TrackedCandies.Add(0);
                return;
            }

            if (ev.Candy != behaviour.CandySettings.CandyType || Random.value * 100 <= behaviour.CandySettings.Weight)
                return;

            behaviour.TrackedCandies.Add(scp330.Candies.Count);
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnItemAdded"/>
        internal void OnInternalItemAdded(ItemAddedEventArgs ev)
        {
            if (IsTracked(ev.Pickup) && ev.Item.Is(out Scp330 scp330))
            {
                if (scp330.TryGetComponent(out CandyBehaviour behaviour))
                {
                    behaviour.TrackedCandies.Add(scp330.Candies.Count - 1);
                }
                else
                {
                    behaviour = scp330.AddComponent<CandyBehaviour>();
                    behaviour.TrackedCandies.Add(0);
                }
            }
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            Exiled.Events.Handlers.Scp330.InteractingScp330 += OnInteractingScp330;
            Exiled.Events.Handlers.Player.ItemAdded += OnInternalItemAdded;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            Exiled.Events.Handlers.Scp330.InteractingScp330 += OnInteractingScp330;
            Exiled.Events.Handlers.Player.ItemAdded -= OnInternalItemAdded;
        }
    }
}