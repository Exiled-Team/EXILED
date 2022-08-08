// -----------------------------------------------------------------------
// <copyright file="CustomGrenade.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API.Features
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups.Projectiles;
    using Exiled.Events.EventArgs;

    using InventorySystem.Items.ThrowableProjectiles;

    using Server = Exiled.API.Features.Server;

    /// <summary>
    /// The Custom Grenade base class.
    /// </summary>
    public abstract class CustomGrenade : CustomItem
    {
        /// <summary>
        /// Gets or sets the <see cref="ItemType"/> to use for this item.
        /// </summary>
        public override ItemType Type
        {
            get => base.Type;
            set
            {
                if (!value.IsThrowable() && value != ItemType.None)
                    throw new ArgumentOutOfRangeException("Type", value, "Invalid grenade type.");

                base.Type = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets a value that determines if the grenade should explode immediately when contacting any surface.
        /// </summary>
        public abstract bool ExplodeOnCollision { get; set; }

        /// <summary>
        /// Gets or sets a value indicating how long the grenade's fuse time should be.
        /// </summary>
        public abstract float FuseTime { get; set; }

        /// <summary>
        /// Checks to see if the grenade is a tracked custom grenade.
        /// </summary>
        /// <param name="grenade">The <see cref="Projectile">grenade</see> to check.</param>
        /// <returns>True if it is a custom grenade.</returns>
        public virtual bool Check(Projectile grenade) => TrackedSerials.Contains(grenade.Serial);

        // TODO: reimplement
        /*
        /// <summary>
        /// Gets a value indicating what thrown grenades are currently being tracked.
        /// </summary>
        [YamlIgnore]
        protected HashSet<ThrownProjectile> Tracked { get; } = new();
        */

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            Events.Handlers.Player.ThrowingRequest += OnInternalThrowingRequest;
            Events.Handlers.Player.ThrowingItem += OnInternalThrowingItem;
            Events.Handlers.Map.ExplodingGrenade += OnInternalExplodingGrenade;
            Events.Handlers.Map.ChangedIntoGrenade += OnInternalChangedIntoGrenade;

            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            Events.Handlers.Player.ThrowingRequest -= OnInternalThrowingRequest;
            Events.Handlers.Player.ThrowingItem -= OnInternalThrowingItem;
            Events.Handlers.Map.ExplodingGrenade -= OnInternalExplodingGrenade;
            Events.Handlers.Map.ChangedIntoGrenade -= OnInternalChangedIntoGrenade;

            base.UnsubscribeEvents();
        }

        /// <summary>
        /// Handles tracking thrown requests by custom grenades.
        /// </summary>
        /// <param name="ev"><see cref="ThrowingRequestEventArgs"/>.</param>
        protected virtual void OnThrowingRequest(ThrowingRequestEventArgs ev)
        {
        }

        /// <summary>
        /// Handles tracking thrown custom grenades.
        /// </summary>
        /// <param name="ev"><see cref="ThrowingRequestEventArgs"/>.</param>
        protected virtual void OnThrowingItem(ThrowingItemEventArgs ev)
        {
        }

        /// <summary>
        /// Handles tracking exploded custom grenades.
        /// </summary>
        /// <param name="ev"><see cref="ExplodingGrenadeEventArgs"/>.</param>
        protected virtual void OnExploding(ExplodingGrenadeEventArgs ev)
        {
        }

        /// <summary>
        /// Handles the tracking of custom grenade pickups that are changed into live grenades by a frag grenade explosion.
        /// </summary>
        /// <param name="ev"><see cref="ChangedIntoGrenadeEventArgs"/>.</param>
        protected virtual void OnChangedIntoGrenade(ChangedIntoGrenadeEventArgs ev)
        {
        }

        private void OnInternalThrowingRequest(ThrowingRequestEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            Log.Debug($"{ev.Player.Nickname} send throw request, item: {Name}!", CustomItems.Instance.Config.Debug);
            if (ev.RequestType == ThrowRequest.BeginThrow)
            {
                OnThrowingRequest(ev);
                return;
            }

            OnThrowingRequest(ev);

            switch (ev.Item)
            {
                case ExplosiveGrenade explosiveGrenade:
                    explosiveGrenade.FuseTime = FuseTime;
                    break;
                case FlashGrenade flashGrenade:
                    flashGrenade.FuseTime = FuseTime;
                    break;
            }
        }

        private void OnInternalThrowingItem(ThrowingItemEventArgs ev)
        {
            if (Check(ev.Item))
                OnThrowingItem(ev);
        }

        private void OnInternalExplodingGrenade(ExplodingGrenadeEventArgs ev)
        {
            if (Check(ev.Grenade))
            {
                Log.Debug($"A {Name} is exploding!!", CustomItems.Instance.Config.Debug);
                OnExploding(ev);
            }
        }

        private void OnInternalChangedIntoGrenade(ChangedIntoGrenadeEventArgs ev)
        {
            if (!Check(ev.Pickup))
                return;

            ev.FuseTime = FuseTime;

            OnChangedIntoGrenade(ev);

            if (ExplodeOnCollision)
                ev.Projectile.GameObject.AddComponent<Exiled.API.Features.Components.CollisionHandler>().Init((ev.Pickup.PreviousOwner ?? Server.Host).GameObject, ev.Projectile.Base);
        }
    }
}
