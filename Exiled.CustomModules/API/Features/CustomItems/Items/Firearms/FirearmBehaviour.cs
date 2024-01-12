// -----------------------------------------------------------------------
// <copyright file="FirearmBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomItems.Items.Firearms
{
    using System;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Core.Generics;
    using Exiled.API.Features.Items;
    using Exiled.CustomModules.API.Features.CustomItems.Items;
    using Exiled.Events.EventArgs.Player;
    using InventorySystem.Items.Firearms.BasicMessages;

    /// <summary>
    /// Represents the base class for custom firearm behaviors.
    /// </summary>
    /// <remarks>
    /// This class extends <see cref="ItemBehaviour"/>.
    /// <br/>It provides a foundation for creating custom behaviors associated with in-game firearms.
    /// </remarks>
    public abstract class FirearmBehaviour : ItemBehaviour
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1401 // Fields should be private
#pragma warning disable SA1600 // Elements should be documented
        protected ItemType itemType;
        protected AmmoType ammoType;
        protected uint customAmmoType;
#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore SA1401 // Fields should be private
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <inheritdoc cref="ItemBehaviour.Settings"/>.
        public FirearmSettings FirearmSettings => Settings.Cast<FirearmSettings>();

        /// <inheritdoc cref="EBehaviour{T}.Owner"/>
        public Firearm Firearm => Owner.Cast<Firearm>();

        /// <inheritdoc/>
        protected override void PostInitialize()
        {
            base.PostInitialize();

            if (Owner is not Firearm _)
            {
                Log.Debug($"{CustomItem.Name} is not a Firearm", true);
                Destroy();
            }

            if (!Settings.Cast(out FirearmSettings _))
            {
                Log.Debug($"{CustomItem.Name}'s settings are not suitable for a Firearm", true);
                Destroy();
            }

            if (!FirearmSettings.Attachments.IsEmpty())
                Firearm.AddAttachment(FirearmSettings.Attachments);

            Firearm.Ammo = FirearmSettings.ClipSize;
            Firearm.Recoil = FirearmSettings.RecoilSettings;

            ItemType ammoItemType = FirearmSettings.AmmoType;
            uint customAmmoId = FirearmSettings.CustomAmmoType;

            if (itemType is ItemType.None)
            {
                if (customAmmoId > 0 && CustomItem.TryGet(customAmmoId, out CustomItem _))
                    customAmmoType = customAmmoId;
            }
            else
            {
                if (ammoItemType.IsAmmo())
                    ammoType = ammoItemType.GetAmmoType();
                else
                    itemType = ammoItemType;
            }
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            Exiled.Events.Handlers.Player.ReloadingWeapon += OnInternalReloading;
            Exiled.Events.Handlers.Player.Shooting += OnInternalShooting;
            Exiled.Events.Handlers.Player.Shot += OnInternalShot;
            Exiled.Events.Handlers.Player.Hurting += OnInternalHurting;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            Exiled.Events.Handlers.Player.ReloadingWeapon -= OnInternalReloading;
            Exiled.Events.Handlers.Player.Shooting -= OnInternalShooting;
            Exiled.Events.Handlers.Player.Shot -= OnInternalShot;
            Exiled.Events.Handlers.Player.Hurting -= OnInternalHurting;
        }

        /// <summary>
        /// Handles reloading events for firearms.
        /// </summary>
        /// <param name="ev">The <see cref="ReloadingWeaponEventArgs"/> containing information about the reloading event.</param>
        protected virtual void OnReloading(ReloadingWeaponEventArgs ev)
        {
        }

        /// <summary>
        /// Handles shooting events for firearms.
        /// </summary>
        /// <param name="ev">The <see cref="ShootingEventArgs"/> containing information about the shooting event.</param>
        protected virtual void OnShooting(ShootingEventArgs ev)
        {
        }

        /// <summary>
        /// Handles shot events for firearms.
        /// </summary>
        /// <param name="ev">The <see cref="ShotEventArgs"/> containing information about the shot event.</param>
        protected virtual void OnShot(ShotEventArgs ev)
        {
        }

        /// <summary>
        /// Handles hurting events for firearms.
        /// </summary>
        /// <param name="ev">The <see cref="HurtingEventArgs"/> containing information about the hurting event.</param>
        protected virtual void OnHurting(HurtingEventArgs ev)
        {
        }

        /// <inheritdoc cref="OnReloading(ReloadingWeaponEventArgs)"/>
        private protected virtual void OnInternalReloading(ReloadingWeaponEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            OnReloading(ev);

            if (!ev.IsAllowed)
                return;

            ev.IsAllowed = false;

            byte clipSize = FirearmSettings.ClipSize;
            byte remainingClip = Firearm.Ammo;

            if (remainingClip >= clipSize)
                return;

            ushort unscaledAmmoAmount = 0;

            if (ammoType is not AmmoType.None)
                unscaledAmmoAmount = (ushort)ev.Player.Items.Where(i => !Check(i) && i.Type == itemType).Count();
            else if (itemType is not ItemType.None)
                unscaledAmmoAmount = ev.Player.GetAmmo(ammoType);
            else if (customAmmoType > 0)
                unscaledAmmoAmount = ev.Player.Cast<Pawn>().GetAmmo(customAmmoType);

            if (unscaledAmmoAmount == 0)
                return;

            ev.Player.Connection.Send(new RequestMessage(ev.Firearm.Serial, RequestType.Reload));

            byte amountToReload = (byte)Math.Min(clipSize - remainingClip, unscaledAmmoAmount);

            if (amountToReload <= 0)
                return;

            ev.Player.GetEffect(EffectType.Invisible).Intensity = 0;

            if (ammoType is not AmmoType.None)
                ev.Player.RemoveAmmo(ammoType, amountToReload);
            else if (itemType is not ItemType.None)
                ev.Player.RemoveItem(itemType);
            else if (customAmmoType > 0)
                ev.Player.Cast<Pawn>().RemoveAmmo(customAmmoType, amountToReload);

            Firearm.Ammo = (byte)(Firearm.Ammo + amountToReload);
        }

        /// <inheritdoc cref="OnShooting(ShootingEventArgs)"/>
        private protected virtual void OnInternalShooting(ShootingEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            OnShooting(ev);
        }

        /// <inheritdoc cref="OnShot(ShotEventArgs)"/>
        private protected virtual void OnInternalShot(ShotEventArgs ev)
        {
            if (!Check(ev.Item))
                return;

            OnShot(ev);
        }

        /// <inheritdoc cref="OnHurting(HurtingEventArgs)"/>
        private protected virtual void OnInternalHurting(HurtingEventArgs ev)
        {
            if (!ev.Attacker || !Check(ev.Attacker))
                return;

            ev.Amount = FirearmSettings.Damage;

            OnHurting(ev);
        }
    }
}