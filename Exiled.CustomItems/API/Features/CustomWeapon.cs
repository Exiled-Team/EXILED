// -----------------------------------------------------------------------
// <copyright file="CustomWeapon.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API.Features
{
    using System;

    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using UnityEngine;

    using YamlDotNet.Serialization;

    using static CustomItems;

    /// <inheritdoc />
    public abstract class CustomWeapon : CustomItem
    {
        /// <summary>
        /// Gets or sets the weapon modifiers.
        /// </summary>
        public abstract Modifiers Modifiers { get; set; }

        /// <inheritdoc/>
        public override ItemType Type
        {
            get => base.Type;
            set
            {
                if (!value.IsWeapon())
                    throw new ArgumentOutOfRangeException("Type", value, "Invalid weapon type.");

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
        public virtual uint ClipSize
        {
            get => (uint)Durability;
            set => Durability = value;
        }

        /// <inheritdoc/>
        [YamlIgnore]
        public override float Durability { get; set; }

        /// <inheritdoc/>
        public override void Spawn(Vector3 position) => Spawned.Add(Item.Spawn(Type, ClipSize, position, default, Modifiers.SightType, Modifiers.BarrelType, Modifiers.OtherType));

        /// <inheritdoc/>
        public override void Give(Player player, bool displayMessage)
        {
            Inventory.SyncItemInfo syncItemInfo = new Inventory.SyncItemInfo()
            {
                durability = ClipSize,
                id = Type,
                uniq = ++Inventory._uniqId,
                modBarrel = Modifiers.BarrelType,
                modSight = Modifiers.SightType,
                modOther = Modifiers.OtherType,
            };

            player.Inventory.items.Add(syncItemInfo);

            InsideInventories.Add(syncItemInfo.uniq);

            if (displayMessage)
                ShowPickedUpMessage(player);
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            Events.Handlers.Player.ReloadingWeapon += OnInternalReloading;
            Events.Handlers.Player.Shooting += OnInternalShooting;
            Events.Handlers.Player.Shot += OnInternalShot;
            Events.Handlers.Player.Hurting += OnInternalHurting;

            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            Events.Handlers.Player.ReloadingWeapon -= OnInternalReloading;
            Events.Handlers.Player.Shooting -= OnInternalShooting;
            Events.Handlers.Player.Shot -= OnInternalShot;
            Events.Handlers.Player.Hurting -= OnInternalHurting;

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
            if (ev.Attacker != ev.Target)
                ev.Amount = Damage;
        }

        private void OnInternalReloading(ReloadingWeaponEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            ev.IsAllowed = false;

            OnReloading(ev);

            uint remainingClip = (uint)ev.Player.CurrentItem.durability;

            if (remainingClip >= ClipSize)
                return;

            Log.Debug($"{ev.Player.Nickname} ({ev.Player.UserId}) [{ev.Player.Role}] is reloading a {Name} ({Id}) [{Type} ({remainingClip}/{ClipSize})]!", Instance.Config.Debug);

            if (ev.IsAnimationOnly)
            {
                ev.Player.ReloadWeapon();
            }
            else
            {
                int ammoType = ev.Player.ReferenceHub.weaponManager.weapons[ev.Player.ReferenceHub.weaponManager.curWeapon].ammoType;
                uint amountToReload = Math.Min(ClipSize - remainingClip, ev.Player.Ammo[ammoType]);

                if (amountToReload <= 0)
                    return;

                ev.Player.ReferenceHub.weaponManager.scp268.ServerDisable();

                ev.Player.Ammo[ammoType] -= amountToReload;
                ev.Player.Inventory.items.ModifyDuration(ev.Player.Inventory.GetItemIndex(), ev.Player.CurrentItem.durability + amountToReload);

                Log.Debug($"{ev.Player.Nickname} ({ev.Player.UserId}) [{ev.Player.Role}] reloaded a {Name} ({Id}) [{Type} ({ev.Player.CurrentItem.durability}/{ClipSize})]!", Instance.Config.Debug);
            }
        }

        private void OnInternalShooting(ShootingEventArgs ev)
        {
            if (!Check(ev.Shooter.CurrentItem))
                return;

            OnShooting(ev);
        }

        private void OnInternalShot(ShotEventArgs ev)
        {
            if (!Check(ev.Shooter.CurrentItem))
                return;

            OnShot(ev);
        }

        private void OnInternalHurting(HurtingEventArgs ev)
        {
            if (!Check(ev.Attacker.CurrentItem))
                return;

            OnHurting(ev);
        }
    }
}
