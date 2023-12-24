// -----------------------------------------------------------------------
// <copyright file="CustomWeapon.cs" company="Exiled Team">
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
    using Exiled.API.Features.DamageHandlers;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.Events.EventArgs.Player;

    using InventorySystem.Items.Firearms.Attachments;
    using InventorySystem.Items.Firearms.Attachments.Components;
    using InventorySystem.Items.Firearms.BasicMessages;
    using UnityEngine;

    using Firearm = Exiled.API.Features.Items.Firearm;
    using Player = Exiled.API.Features.Player;

    /// <summary>
    /// The Custom Weapon base class.
    /// </summary>
    public abstract class CustomWeapon : CustomItem
    {
        /// <summary>
        /// Gets or sets value indicating what <see cref="Attachment"/>s the weapon will have.
        /// </summary>
        public virtual AttachmentName[] Attachments { get; set; } = { };

        /// <inheritdoc/>
        public override ItemType Type
        {
            get => base.Type;
            set
            {
                if (!value.IsWeapon(false) && value != ItemType.None)
                    throw new ArgumentOutOfRangeException($"{nameof(Type)}", value, "Invalid weapon type.");

                base.Type = value;
            }
        }

        /// <summary>
        /// Gets or sets the weapon damage.
        /// </summary>
        public abstract float Damage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating how big of a clip the weapon will have.
        /// </summary>
        public virtual byte ClipSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to allow friendly fire with this weapon on FF-enabled servers.
        /// </summary>
        public virtual bool FriendlyFire { get; set; }

        /// <inheritdoc />
        public override Pickup? Spawn(Vector3 position, Player? previousOwner = null)
        {
            if (Item.Create(Type) is not Firearm firearm)
            {
                Log.Debug($"{nameof(Spawn)}: Item is not Firearm.");
                return null;
            }

            if (!Attachments.IsEmpty())
                firearm.AddAttachment(Attachments);

            firearm.Ammo = ClipSize;

            Pickup? pickup = firearm.CreatePickup(position);

            if (pickup is null)
            {
                Log.Debug($"{nameof(Spawn)}: Pickup is null.");
                return null;
            }

            pickup.Weight = Weight;
            pickup.Scale = Scale;
            if (previousOwner is not null)
                pickup.PreviousOwner = previousOwner;

            TrackedSerials.Add(pickup.Serial);
            return pickup;
        }

        /// <inheritdoc />
        public override Pickup? Spawn(Vector3 position, Item item, Player? previousOwner = null)
        {
            if (item is Firearm firearm)
            {
                if (!Attachments.IsEmpty())
                    firearm.AddAttachment(Attachments);

                byte ammo = firearm.Ammo;
                Log.Debug($"{nameof(Name)}.{nameof(Spawn)}: Spawning weapon with {ammo} ammo.");
                Pickup? pickup = firearm.CreatePickup(position);
                pickup.Scale = Scale;

                if (previousOwner is not null)
                    pickup.PreviousOwner = previousOwner;

                TrackedSerials.Add(pickup.Serial);
                return pickup;
            }

            return base.Spawn(position, item, previousOwner);
        }

        /// <inheritdoc/>
        public override void Give(Player player, bool displayMessage = true)
        {
            Item item = player.AddItem(Type);

            if (item is Firearm firearm)
            {
                if (!Attachments.IsEmpty())
                    firearm.AddAttachment(Attachments);

                firearm.Ammo = ClipSize;
            }

            Log.Debug($"{nameof(Give)}: Adding {item.Serial} to tracker.");
            TrackedSerials.Add(item.Serial);

            OnAcquired(player, item, displayMessage);
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ReloadingWeapon += OnInternalReloading;
            Exiled.Events.Handlers.Player.Shooting += OnInternalShooting;
            Exiled.Events.Handlers.Player.Shot += OnInternalShot;
            Exiled.Events.Handlers.Player.Hurting += OnInternalHurting;

            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ReloadingWeapon -= OnInternalReloading;
            Exiled.Events.Handlers.Player.Shooting -= OnInternalShooting;
            Exiled.Events.Handlers.Player.Shot -= OnInternalShot;
            Exiled.Events.Handlers.Player.Hurting -= OnInternalHurting;

            base.UnsubscribeEvents();
        }

        /// <summary>
        /// Handles reloading for custom weapons.
        /// </summary>
        /// <param name="ev"><see cref="ReloadingWeaponEventArgs"/>.</param>
        protected virtual void OnReloading(ReloadingWeaponEventArgs ev)
        {
        }

        /// <summary>
        /// Handles shooting for custom weapons.
        /// </summary>
        /// <param name="ev"><see cref="ShootingEventArgs"/>.</param>
        protected virtual void OnShooting(ShootingEventArgs ev)
        {
        }

        /// <summary>
        /// Handles shot for custom weapons.
        /// </summary>
        /// <param name="ev"><see cref="ShotEventArgs"/>.</param>
        protected virtual void OnShot(ShotEventArgs ev)
        {
        }

        /// <summary>
        /// Handles hurting for custom weapons.
        /// </summary>
        /// <param name="ev"><see cref="HurtingEventArgs"/>.</param>
        protected virtual void OnHurting(HurtingEventArgs ev)
        {
            if (ev.IsAllowed && Damage > 0f)
                ev.Amount = Damage;
        }

        private void OnInternalReloading(ReloadingWeaponEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            Log.Debug($"{nameof(Name)}.{nameof(OnInternalReloading)}: Reloading weapon. Calling external reload event..");
            OnReloading(ev);

            Log.Debug($"{nameof(Name)}.{nameof(OnInternalReloading)}: External event ended. {ev.IsAllowed}");
            if (!ev.IsAllowed)
            {
                Log.Debug($"{nameof(Name)}.{nameof(OnInternalReloading)}: External event turned is allowed to false, returning.");
                return;
            }

            Log.Debug($"{nameof(Name)}.{nameof(OnInternalReloading)}: Continuing with internal reload..");
            ev.IsAllowed = false;

            byte remainingClip = ((Firearm)ev.Player.CurrentItem).Ammo;

            if (remainingClip >= ClipSize)
                return;

            Log.Debug($"{ev.Player.Nickname} ({ev.Player.UserId}) [{ev.Player.Role}] is reloading a {Name} ({Id}) [{Type} ({remainingClip}/{ClipSize})]!");

            AmmoType ammoType = ev.Firearm.AmmoType;

            if (!ev.Player.Ammo.ContainsKey(ammoType.GetItemType()))
            {
                Log.Debug($"{nameof(Name)}.{nameof(OnInternalReloading)}: {ev.Player.Nickname} does not have ammo to reload this weapon.");
                return;
            }

            ev.Player.Connection.Send(new RequestMessage(ev.Firearm.Serial, RequestType.Reload));

            byte amountToReload = (byte)Math.Min(ClipSize - remainingClip, ev.Player.Ammo[ammoType.GetItemType()]);

            if (amountToReload <= 0)
                return;

            ev.Player.ReferenceHub.playerEffectsController.GetEffect<CustomPlayerEffects.Invisible>().Intensity = 0;

            ev.Player.Ammo[ammoType.GetItemType()] -= amountToReload;
            ev.Player.Inventory.SendAmmoNextFrame = true;

            ((Firearm)ev.Player.CurrentItem).Ammo = (byte)(((Firearm)ev.Player.CurrentItem).Ammo + amountToReload);

            Log.Debug($"{ev.Player.Nickname} ({ev.Player.UserId}) [{ev.Player.Role}] reloaded a {Name} ({Id}) [{Type} ({((Firearm)ev.Player.CurrentItem).Ammo}/{ClipSize})]!");
        }

        private void OnInternalShooting(ShootingEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            OnShooting(ev);
        }

        private void OnInternalShot(ShotEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            OnShot(ev);
        }

        private void OnInternalHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker is null)
            {
                return;
            }

            if (ev.Player is null)
            {
                Log.Debug($"{Name}: {nameof(OnInternalHurting)}: target null");
                return;
            }

            if (!Check(ev.Attacker.CurrentItem))
            {
                Log.Debug($"{Name}: {nameof(OnInternalHurting)}: !Check()");
                return;
            }

            if (ev.Attacker == ev.Player)
            {
                Log.Debug($"{Name}: {nameof(OnInternalHurting)}: attacker == target");
                return;
            }

            if (ev.DamageHandler is null)
            {
                Log.Debug($"{Name}: {nameof(OnInternalHurting)}: Handler null");
                return;
            }

            if (!ev.DamageHandler.CustomBase.BaseIs(out FirearmDamageHandler firearmDamageHandler))
            {
                Log.Debug($"{Name}: {nameof(OnInternalHurting)}: Handler not firearm");
                return;
            }

            if (!Check(firearmDamageHandler.Item))
            {
                Log.Debug($"{Name}: {nameof(OnInternalHurting)}: type != type");
                return;
            }

            if (!FriendlyFire && (ev.Attacker.Role.Team == ev.Player.Role.Team))
            {
                Log.Debug($"{Name}: {nameof(OnInternalHurting)}: FF is disabled for this weapon!");
                return;
            }

            OnHurting(ev);
        }
    }
}
