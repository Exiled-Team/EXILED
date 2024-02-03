// -----------------------------------------------------------------------
// <copyright file="CandyBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomItems.Items.Candies
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Behaviours;
    using Exiled.API.Features.Core.Generic;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Scp330;
    using UnityEngine;

    /// <summary>
    /// Represents the base class for custom candies behaviors.
    /// </summary>
    /// <remarks>
    /// This class extends <see cref="ItemBehaviour"/>.
    /// <br/>It provides a foundation for creating custom behaviors associated with in-game candies.
    /// </remarks>
    public abstract class CandyBehaviour : ItemBehaviour
    {
        /// <summary>
        /// Gets or sets a <see cref="HashSet{T}"/> containing all tracked indexes in <see cref="Scp330"/>.
        /// </summary>
        public HashSet<int> TrackedCandies { get; protected set; }

        /// <inheritdoc cref="ItemBehaviour.Settings"/>
        public CandySettings CandySettings => Settings.Cast<CandySettings>();

        /// <inheritdoc cref="EBehaviour{T}.Owner"/>.
        public Scp330 Scp330 => Owner.Cast<Scp330>();

        /// <summary>
        /// Applies effect to player.
        /// </summary>
        /// <param name="player">The player to apply effects to.</param>
        public virtual void ApplyEffects(Pawn player)
        {
        }

        /// <inheritdoc/>
        protected override void PostInitialize()
        {
            base.PostInitialize();

            if (Owner is not Scp330 _)
            {
                Log.Debug($"{CustomItem.Name} is not a Scp330 bag!", true);
                Destroy();
            }

            if (Settings is not CandySettings _)
            {
                Log.Debug($"{CustomItem.Name} settings is not suitable for candies!", true);
                Destroy();
            }

            CandySettings.SelectedText = new($"Custom candies in this bag:\n{string.Join("\n", TrackedCandies.Select(x => x++))}");
            TrackedCandies = new();
        }

        /// <inheritdoc/>
        protected override void OnDestroyed()
        {
            base.OnDestroyed();

            TrackedCandies.Clear();
        }

        /// <inheritdoc/>
        protected override bool Check(Item owner) => base.Check(owner) && owner.Is(out Scp330 scp330) && TrackedCandies.Contains(scp330.SelectedCandyId);

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            Exiled.Events.Handlers.Scp330.EatingScp330 += OnEatingScp330Internal;
            Exiled.Events.Handlers.Scp330.InteractingScp330 += OnInteractingScp330Internal;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            Exiled.Events.Handlers.Scp330.EatingScp330 -= OnEatingScp330Internal;
            Exiled.Events.Handlers.Scp330.InteractingScp330 -= OnInteractingScp330Internal;
        }

        /// <inheritdoc/>
        protected override void OnAcquired(Player player, Item item, bool displayMessage = true)
        {
            base.OnAcquired(player, item, displayMessage);

            if (Scp330.Candies.Count == 0)
            {
                TrackedCandies.Add(Scp330.Candies.Count);
                Scp330.AddCandy(CandySettings.CandyType);
            }
            else
            {
                TrackedCandies.Add(Scp330.Candies.Count - 1);
            }
        }

        /// <summary>
        /// Fired when player is eating custom candy.
        /// </summary>
        /// <param name="ev">The <see cref="EatingScp330EventArgs"/> event instance.</param>
        protected virtual void OnEatingCandy(EatingScp330EventArgs ev)
        {
        }

        /// <summary>
        /// Fired when player interacts with SCP-330 and gets a custom candy.
        /// </summary>
        /// <param name="ev">The <see cref="EatingScp330EventArgs"/> event instance.</param>
        protected virtual void OnInteracting(InteractingScp330EventArgs ev)
        {
        }

        /// <inheritdoc cref="OnEatingCandy"/>
        private protected void OnEatingScp330Internal(EatingScp330EventArgs ev)
        {
            if (!ev.Player.TryGetItems(x => x.Type == ItemType.SCP330, out IEnumerable<Item> items) || !items.Single().Is(out Scp330 scp330) || !Check(scp330))
                return;

            ev.Candy = new BaseCandy(CandySettings, ApplyEffects);
            ev.Player.ShowTextDisplay(CandySettings.EatenCustomCandyMessage);

            OnEatingCandy(ev);
        }

        /// <inheritdoc cref="OnInteracting"/>
        private protected void OnInteractingScp330Internal(InteractingScp330EventArgs ev)
        {
            if (ev.Candy != CandySettings.CandyType || Random.value * 100 >= CandySettings.Weight ||
                !ev.Player.TryGetItems(x => x.Type == ItemType.SCP330, out IEnumerable<Item> items) ||
                !items.Single().Is(out Scp330 scp330))
                return;

            TrackedCandies.Add(scp330.SelectedCandyId);
            StaticActor.Get<ItemTracker>().AddOrTrack(scp330);
            scp330.AddComponent<CandyBehaviour>();
            ev.Player.ShowTextDisplay(CandySettings.ReceivedCustomCandyMessage);

            OnInteracting(ev);
        }
    }
}