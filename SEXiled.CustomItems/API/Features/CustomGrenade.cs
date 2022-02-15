// -----------------------------------------------------------------------
// <copyright file="CustomGrenade.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.CustomItems.API.Features
{
    using System;
    using System.Collections.Generic;

    using SEXiled.API.Enums;
    using SEXiled.API.Extensions;
    using SEXiled.API.Features;
    using SEXiled.API.Features.Items;
    using SEXiled.Events.EventArgs;

    using Footprinting;

    using InventorySystem.Items;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.ThrowableProjectiles;

    using MEC;

    using Mirror;

    using UnityEngine;

    using YamlDotNet.Serialization;

    /// <inheritdoc />
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
                if (!value.IsThrowable())
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
        /// Gets a value indicating what thrown grenades are currently being tracked.
        /// </summary>
        [YamlIgnore]
        protected HashSet<ThrownProjectile> Tracked { get; } = new HashSet<ThrownProjectile>();

        /// <summary>
        /// Gives the <see cref="CustomItem"/> to a player.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> who will receive the item.</param>
        /// <param name="displayMessage">Indicates whether or not <see cref="CustomItem.ShowPickedUpMessage"/> will be called when the player receives the item.</param>
        public override void Give(Player player, bool displayMessage = true) => Give(player, new Throwable((ThrowableItem)player.Inventory.CreateItemInstance(Type, true)), displayMessage);

        /// <summary>
        /// Spawns a live grenade object on the map.
        /// </summary>
        /// <param name="position">The <see cref="Vector3"/> to spawn the grenade at.</param>
        /// <param name="force">The amount of force to throw with.</param>
        /// <param name="fuseTime">The <see cref="float"/> fuse time of the grenade.</param>
        /// <param name="grenadeType">The <see cref="GrenadeType"/> of the grenade to spawn.</param>
        /// <param name="player">The <see cref="Player"/> to count as the thrower of the grenade.</param>
        /// <returns>The <see cref="Pickup"/> being spawned.</returns>
        public virtual Pickup Throw(Vector3 position, float force, float fuseTime = 3f, ItemType grenadeType = ItemType.GrenadeHE, Player player = null)
        {
            if (player == null)
                player = Server.Host;

            Throwable throwable = (Throwable)Item.Create(grenadeType, player);

            ThrownProjectile thrownProjectile = UnityEngine.Object.Instantiate(throwable.Base.Projectile, position, throwable.Owner.CameraTransform.rotation);
            Transform transform = thrownProjectile.transform;
            PickupSyncInfo newInfo = new PickupSyncInfo()
            {
                ItemId = throwable.Type,
                Locked = !throwable.Base._repickupable,
                Serial = ItemSerialGenerator.GenerateNext(),
                Weight = Weight,
                Position = transform.position,
                Rotation = new LowPrecisionQuaternion(transform.rotation),
            };
            if (thrownProjectile is TimeGrenade time)
                time._fuseTime = fuseTime;
            thrownProjectile.NetworkInfo = newInfo;
            thrownProjectile.PreviousOwner = new Footprint(throwable.Owner.ReferenceHub);
            NetworkServer.Spawn(thrownProjectile.gameObject);
            thrownProjectile.InfoReceived(default, newInfo);
            if (thrownProjectile.TryGetComponent(out Rigidbody component))
                throwable.Base.PropelBody(component, throwable.Base.FullThrowSettings.StartTorque, ThrowableNetworkHandler.GetLimitedVelocity(player.ReferenceHub.playerMovementSync.PlayerVelocity), force, throwable.Base.FullThrowSettings.UpwardsFactor);
            thrownProjectile.ServerActivate();
            Tracked.Add(thrownProjectile);

            if (ExplodeOnCollision)
                thrownProjectile.gameObject.AddComponent<SEXiled.API.Features.Components.CollisionHandler>().Init(player.GameObject, (EffectGrenade)thrownProjectile);

            return Pickup.Get(thrownProjectile);
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            Events.Handlers.Player.ThrowingItem += OnInternalThrowing;
            Events.Handlers.Map.ExplodingGrenade += OnInternalExplodingGrenade;
            Events.Handlers.Map.ChangingIntoGrenade += OnInternalChangingIntoGrenade;

            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            Events.Handlers.Player.ThrowingItem -= OnInternalThrowing;
            Events.Handlers.Map.ExplodingGrenade -= OnInternalExplodingGrenade;
            Events.Handlers.Map.ChangingIntoGrenade -= OnInternalChangingIntoGrenade;

            base.UnsubscribeEvents();
        }

        /// <summary>
        /// Handles tracking thrown custom grenades.
        /// </summary>
        /// <param name="ev"><see cref="ThrowingItemEventArgs"/>.</param>
        protected virtual void OnThrowing(ThrowingItemEventArgs ev)
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
        /// <param name="ev"><see cref="ChangingIntoGrenadeEventArgs"/>.</param>
        protected virtual void OnChangingIntoGrenade(ChangingIntoGrenadeEventArgs ev)
        {
        }

        /// <inheritdoc/>
        protected override void OnWaitingForPlayers()
        {
            Tracked.Clear();

            base.OnWaitingForPlayers();
        }

        /// <summary>
        /// Checks to see if the grenade is a tracked custom grenade.
        /// </summary>
        /// <param name="grenade">The <see cref="GameObject"/> of the grenade to check.</param>
        /// <returns>True if it is a custom grenade.</returns>
        protected bool Check(ThrownProjectile grenade) => TrackedSerials.Contains(grenade.Info.Serial);

        private void OnInternalThrowing(ThrowingItemEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            Log.Debug($"{ev.Player.Nickname} has thrown a {Name}!", CustomItems.Instance.Config.Debug);
            if (ev.RequestType == ThrowRequest.BeginThrow)
            {
                OnThrowing(ev);
                if (!ev.IsAllowed)
                    ev.IsAllowed = false;
                return;
            }

            OnThrowing(ev);

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

        private void OnInternalExplodingGrenade(ExplodingGrenadeEventArgs ev)
        {
            if (Check(ev.Grenade))
            {
                Log.Debug($"A {Name} is exploding!!", CustomItems.Instance.Config.Debug);
                OnExploding(ev);
            }
        }

        private void OnInternalChangingIntoGrenade(ChangingIntoGrenadeEventArgs ev)
        {
            if (!Check(ev.Pickup))
                return;

            ev.FuseTime = FuseTime;
            ev.Type = Type;

            OnChangingIntoGrenade(ev);

            if (ev.IsAllowed)
            {
                Timing.CallDelayed(0.25f, () => Throw(ev.Pickup.Position, 0f, ev.FuseTime, ev.Type));
                ev.Pickup.Destroy();
                ev.IsAllowed = false;
            }
        }
    }
}
