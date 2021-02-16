// -----------------------------------------------------------------------
// <copyright file="CustomWeapon.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using UnityEngine;

    /// <inheritdoc />
    public abstract class CustomWeapon : CustomItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomWeapon"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> to be used.</param>
        /// <param name="clipSize">The <see cref="int"/> size of the clip to be used.</param>
        /// <param name="itemId">The <see cref="int"/> ID to be used.</param>
        protected CustomWeapon(ItemType type, int clipSize, int itemId)
            : base(type, itemId) => ClipSize = clipSize;

        /// <inheritdoc/>
        public abstract override string Name { get; set; }

        /// <inheritdoc/>
        public abstract override string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating how big of a clip the weapon will have.
        /// </summary>
        protected virtual int ClipSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating what <see cref="ModBarrel"/> the weapon will have.
        /// </summary>
        protected virtual int ModBarrel { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating what <see cref="ModSight"/> the weapon will have.
        /// </summary>
        protected virtual int ModSight { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating what <see cref="ModOther"/> the weapon will have.
        /// </summary>
        protected virtual int ModOther { get; set; } = 0;

        /// <inheritdoc/>
        public override void Init()
        {
            Exiled.Events.Handlers.Player.ReloadingWeapon += OnReloadingWeapon;
            base.Init();
        }

        /// <inheritdoc/>
        public override void Destroy()
        {
            Exiled.Events.Handlers.Player.ReloadingWeapon -= OnReloadingWeapon;
            base.Destroy();
        }

        /// <inheritdoc/>
        public override void SpawnItem(Vector3 position) => ItemPickups.Add(Exiled.API.Extensions.Item.Spawn(ItemType, ClipSize, position, default, ModSight, ModBarrel, ModOther));

        /// <inheritdoc/>
        public override void GiveItem(Player player)
        {
            ++Inventory._uniqId;
            Inventory.SyncItemInfo syncItemInfo = new Inventory.SyncItemInfo()
            {
                durability = 1,
                id = ItemType,
                uniq = Inventory._uniqId,
            };
            player.Inventory.items.Add(syncItemInfo);
            ItemIds.Add(syncItemInfo.uniq);

            ShowMessage(player);

            ItemGiven(player);
        }

        /// <inheritdoc/>
        public override void GiveItem(Player player, bool displayMessage)
        {
            ++Inventory._uniqId;
            Inventory.SyncItemInfo syncItemInfo = new Inventory.SyncItemInfo()
            {
                durability = ClipSize,
                id = ItemType,
                uniq = Inventory._uniqId,
                modBarrel = ModBarrel,
                modSight = ModSight,
                modOther = ModOther,
            };
            player.Inventory.items.Add(syncItemInfo);
            ItemIds.Add(syncItemInfo.uniq);

            if (displayMessage)
                ShowMessage(player);

            ItemGiven(player);
        }

        /// <summary>
        /// Reloads the current weapon for the player.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> who's weapon should be reloaded.</param>
        protected static void Reload(Player player) => player.ReferenceHub.weaponManager.RpcReload(player.ReferenceHub.weaponManager.curWeapon);

        /// <summary>
        /// Handles reloading for custom weapons.
        /// </summary>
        /// <param name="ev"><see cref="ReloadingWeaponEventArgs"/>.</param>
        protected virtual void OnReloadingWeapon(ReloadingWeaponEventArgs ev)
        {
            if (!CheckItem(ev.Player.CurrentItem))
                return;

            ev.IsAllowed = false;

            int remainingInClip = (int)ev.Player.CurrentItem.durability;
            if (remainingInClip >= ClipSize)
                return;

            Log.Debug($"{ev.Player.Nickname} is reloading a {Name}!", CustomItems.Singleton.Config.Debug);
            if (ev.IsAnimationOnly)
            {
                Reload(ev.Player);
            }
            else
            {
                int currentAmmoAmount =
                    (int)ev.Player.Ammo[
                        ev.Player.ReferenceHub.weaponManager.weapons[ev.Player.ReferenceHub.weaponManager.curWeapon]
                            .ammoType];
                int amountToReload = ClipSize - remainingInClip;
                if (currentAmmoAmount < 0)
                {
                    Log.Debug($"Returning!");
                    return;
                }

                ev.Player.ReferenceHub.weaponManager.scp268.ServerDisable();

                int amountAfterReload = currentAmmoAmount - amountToReload;
                ev.Player.Ammo[ev.Player.ReferenceHub.weaponManager.weapons[ev.Player.ReferenceHub.weaponManager.curWeapon].ammoType] = amountAfterReload < 0 ? 0 : (uint)(currentAmmoAmount - amountToReload);

                Log.Debug($"{remainingInClip} - {currentAmmoAmount} - {amountToReload} - {amountAfterReload} - {ev.Player.Ammo[ev.Player.ReferenceHub.weaponManager.weapons[ev.Player.ReferenceHub.weaponManager.curWeapon].ammoType]}");
                ev.Player.Inventory.items.ModifyDuration(ev.Player.Inventory.GetItemIndex(), ClipSize);
            }

            Log.Debug($"{ev.Player.Nickname} - {ev.Player.CurrentItem.durability} - {ev.Player.Ammo[ev.Player.ReferenceHub.weaponManager.weapons[ev.Player.ReferenceHub.weaponManager.curWeapon].ammoType]}", CustomItems.Singleton.Config.Debug);
        }
    }
}
