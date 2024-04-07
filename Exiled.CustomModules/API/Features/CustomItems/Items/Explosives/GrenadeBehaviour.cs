// -----------------------------------------------------------------------
// <copyright file="GrenadeBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomItems.Items.Explosives
{
    using Exiled.API.Features;
    using Exiled.API.Features.Components;
    using Exiled.API.Features.Core.Generic;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Pickups.Projectiles;
    using Exiled.API.Features.Roles;
    using Exiled.CustomModules.API.Features.CustomItems.Items;
    using Exiled.Events.EventArgs.Map;
    using Exiled.Events.EventArgs.Player;

    using InventorySystem.Items;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.ThrowableProjectiles;

    using Mirror;
    using UnityEngine;

    /// <summary>
    /// Represents the base class for custom grenade behaviors.
    /// </summary>
    /// <remarks>
    /// This class extends <see cref="ItemBehaviour"/>.
    /// <br/>It provides a foundation for creating custom behaviors associated with in-game grenades.
    /// </remarks>
    public abstract class GrenadeBehaviour : ItemBehaviour
    {
        /// <inheritdoc cref="ItemBehaviour.Settings"/>.
        public GrenadeSettings GrenadeSettings => Settings.Cast<GrenadeSettings>();

        /// <inheritdoc cref="EBehaviour{T}.Owner"/>
        public Throwable Throwable => Owner.Cast<Throwable>();

        /// <summary>
        /// Throw the custom grenade object.
        /// </summary>
        /// <param name="position">The <see cref="Vector3"/> position to throw at.</param>
        /// <param name="force">The amount of force to throw with.</param>
        /// <param name="weight">The <see cref="float"/> weight of the Grenade.</param>
        /// <param name="fuseTime">The <see cref="float"/> fuse time of the grenade.</param>
        /// <param name="grenadeType">The <see cref="ItemType"/> of the grenade to spawn.</param>
        /// <param name="player">The <see cref="Player"/> to count as the thrower of the grenade.</param>
        /// <returns>The spawned <see cref="Pickup"/>.</returns>
        public virtual Pickup Throw(Vector3 position, float force, float weight, float fuseTime = 3f, ItemType grenadeType = ItemType.GrenadeHE, Player player = null)
        {
            if (!player)
                player = Server.Host;

            if (!player.Role.Is(out FpcRole fpcRole))
                return null;

            Vector3 velocity = fpcRole.FirstPersonController.FpcModule.Motor.Velocity;
            Throwable throwable = (Throwable)Item.Create(grenadeType, player);
            ThrownProjectile thrownProjectile = Object.Instantiate(throwable.Base.Projectile, position, throwable.Owner.CameraTransform.rotation);

            PickupSyncInfo newInfo = new()
            {
                ItemId = throwable.Type,
                Locked = !throwable.Base._repickupable,
                Serial = ItemSerialGenerator.GenerateNext(),
                WeightKg = weight,
            };

            if (thrownProjectile is TimeGrenade time)
                time._fuseTime = fuseTime;

            thrownProjectile.NetworkInfo = newInfo;
            thrownProjectile.PreviousOwner = new(throwable.Owner.ReferenceHub);

            NetworkServer.Spawn(thrownProjectile.gameObject);

            thrownProjectile.InfoReceivedHook(default, newInfo);

            if (thrownProjectile.TryGetComponent(out Rigidbody component))
                throwable.Base.PropelBody(component, throwable.Base.FullThrowSettings.StartTorque, ThrowableNetworkHandler.GetLimitedVelocity(velocity), force, throwable.Base.FullThrowSettings.UpwardsFactor);

            thrownProjectile.ServerActivate();

            return Pickup.Get(thrownProjectile);
        }

        /// <inheritdoc/>
        protected override void PostInitialize()
        {
            base.PostInitialize();

            if (Owner is not Throwable _)
            {
                Log.Debug($"{CustomItem.Name} is not a Grenade", true);
                Destroy();
            }

            if (!Settings.Cast(out GrenadeSettings _))
            {
                Log.Debug($"{CustomItem.Name}'s settings are not suitable for a Grenade", true);
                Destroy();
            }
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            Exiled.Events.Handlers.Player.ThrowingRequest += OnInternalThrowingRequest;
            Exiled.Events.Handlers.Player.ThrownProjectile += OnInternalThrownProjectile;
            Exiled.Events.Handlers.Map.ExplodingGrenade += OnInternalExplodingGrenade;
            Exiled.Events.Handlers.Map.ChangedIntoGrenade += OnInternalChangedIntoGrenade;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            Exiled.Events.Handlers.Player.ThrowingRequest -= OnInternalThrowingRequest;
            Exiled.Events.Handlers.Player.ThrownProjectile -= OnInternalThrownProjectile;
            Exiled.Events.Handlers.Map.ExplodingGrenade -= OnInternalExplodingGrenade;
            Exiled.Events.Handlers.Map.ChangedIntoGrenade -= OnInternalChangedIntoGrenade;
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
        /// <param name="ev"><see cref="ThrownProjectileEventArgs"/>.</param>
        protected virtual void OnThrownProjectile(ThrownProjectileEventArgs ev)
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

        /// <inheritdoc cref="OnThrowingRequest(ThrowingRequestEventArgs)"/>
        private protected void OnInternalThrowingRequest(ThrowingRequestEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            OnThrowingRequest(ev);
        }

        /// <inheritdoc cref="OnThrownProjectile(ThrownProjectileEventArgs)"/>
        private protected void OnInternalThrownProjectile(ThrownProjectileEventArgs ev)
        {
            if (!Check(ev.Throwable))
                return;

            OnThrownProjectile(ev);

            if (ev.Projectile is TimeGrenadeProjectile timeGrenade)
                timeGrenade.FuseTime = GrenadeSettings.FuseTime;

            if (GrenadeSettings.ExplodeOnCollision)
                ev.Projectile.GameObject.AddComponent<EffectGrenadeCollision>().Init((ev.Player ?? Server.Host).GameObject, ev.Projectile.Base);
        }

        /// <inheritdoc cref="OnExploding(ExplodingGrenadeEventArgs)"/>
        private protected void OnInternalExplodingGrenade(ExplodingGrenadeEventArgs ev)
        {
            if (!Check(ev.Projectile))
                return;

            OnExploding(ev);
        }

        /// <inheritdoc cref="OnChangedIntoGrenade(ChangedIntoGrenadeEventArgs)"/>
        private protected void OnInternalChangedIntoGrenade(ChangedIntoGrenadeEventArgs ev)
        {
            if (!Check(ev.Pickup))
                return;

            if (ev.Projectile is TimeGrenadeProjectile timedGrenade)
                timedGrenade.FuseTime = GrenadeSettings.FuseTime;

            OnChangedIntoGrenade(ev);

            if (GrenadeSettings.ExplodeOnCollision)
                ev.Projectile.GameObject.AddComponent<EffectGrenadeCollision>().Init((ev.Pickup.PreviousOwner ?? Server.Host).GameObject, ev.Projectile.Base);
        }
    }
}