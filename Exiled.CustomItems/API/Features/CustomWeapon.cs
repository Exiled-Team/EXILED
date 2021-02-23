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

    /// <inheritdoc />
    public abstract class CustomWeapon : CustomItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomWeapon"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> to be used.</param>
        /// <param name="clipSize">The <see cref="uint"/> size of the clip to be used.</param>
        /// <param name="id">The <see cref="uint"/> ID to be used.</param>
        protected CustomWeapon(ItemType type, uint id, uint clipSize)
            : base(type, id) => ClipSize = clipSize;

        /// <summary>
        /// Gets the weapon modifiers.
        /// </summary>
        public virtual Modifiers Modifiers { get; }

        /// <inheritdoc/>
        public override ItemType Type
        {
            get => base.Type;
            protected set
            {
                if (!value.IsWeapon())
                    throw new ArgumentOutOfRangeException("Type", value, "Invalid weapon type.");

                base.Type = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating how big of a clip the weapon will have.
        /// </summary>
        public virtual uint ClipSize
        {
            get => (uint)Durability;
            protected set => Durability = value;
        }

        /// <inheritdoc/>
        [YamlIgnore]
        public override float Durability { get; protected set; }

        /// <inheritdoc/>
        public override void Spawn(Vector3 position)
        {
            Pickups.Add(Item.Spawn(Type, ClipSize, position, default, Modifiers.SightType, Modifiers.BarrelType, Modifiers.OtherType));
        }

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
                ShowMessage(player);

            ItemGiven(player);
        }

        /// <inheritdoc/>
        public override void Init()
        {
            SubscribeEvents();

            base.Init();
        }

        /// <inheritdoc/>
        public override void Destroy()
        {
            UnsubscribeEvents();

            base.Destroy();
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            Events.Handlers.Player.ReloadingWeapon += OnReloading;

            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            Events.Handlers.Player.ReloadingWeapon -= OnReloading;

            base.UnsubscribeEvents();
        }

        /// <summary>
        /// Handles reloading for custom weapons.
        /// </summary>
        /// <param name="ev"><see cref="ReloadingWeaponEventArgs"/>.</param>
        protected virtual void OnReloading(ReloadingWeaponEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            ev.IsAllowed = false;

            uint remainingInClip = (uint)ev.Player.CurrentItem.durability;

            if (remainingInClip >= ClipSize)
                return;

            Log.Debug($"{ev.Player.Nickname} is reloading a {Name}!", CustomItems.Instance.Config.Debug);

            if (ev.IsAnimationOnly)
            {
                ev.Player.ReloadWeapon();
            }
            else
            {
                uint currentAmmoAmount = ev.Player.Ammo[ev.Player.ReferenceHub.weaponManager.weapons[ev.Player.ReferenceHub.weaponManager.curWeapon].ammoType];
                uint amountToReload = ClipSize - remainingInClip;

                if (currentAmmoAmount < 0)
                {
                    Log.Debug($"Returning!");
                    return;
                }

                ev.Player.ReferenceHub.weaponManager.scp268.ServerDisable();

                uint amountAfterReload = currentAmmoAmount - amountToReload;

                ev.Player.Ammo[ev.Player.ReferenceHub.weaponManager.weapons[ev.Player.ReferenceHub.weaponManager.curWeapon].ammoType] = amountAfterReload < 0 ? 0 : currentAmmoAmount - amountToReload;

                Log.Debug($"{remainingInClip} - {currentAmmoAmount} - {amountToReload} - {amountAfterReload} - {ev.Player.Ammo[ev.Player.ReferenceHub.weaponManager.weapons[ev.Player.ReferenceHub.weaponManager.curWeapon].ammoType]}");

                ev.Player.Inventory.items.ModifyDuration(ev.Player.Inventory.GetItemIndex(), ClipSize);
            }

            Log.Debug($"{ev.Player.Nickname} - {ev.Player.CurrentItem.durability} - {ev.Player.Ammo[ev.Player.ReferenceHub.weaponManager.weapons[ev.Player.ReferenceHub.weaponManager.curWeapon].ammoType]}", CustomItems.Instance.Config.Debug);
        }
    }
}
