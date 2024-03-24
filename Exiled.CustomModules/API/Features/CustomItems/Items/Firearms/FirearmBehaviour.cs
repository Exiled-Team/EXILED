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
    using Exiled.API.Features.Core.Generic;
    using Exiled.API.Features.Items;
    using Exiled.CustomModules.API.Features.CustomItems.Items;
    using Exiled.Events.EventArgs.Item;
    using Exiled.Events.EventArgs.Player;

    using InventorySystem.Items.Firearms.Attachments;
    using InventorySystem.Items.Firearms.BasicMessages;
    using InventorySystem.Items.Firearms.Modules;
    using MEC;

    using Firearm = Exiled.API.Features.Items.Firearm;

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
        protected bool overrideReload;
        protected int chamberSize;
#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore SA1401 // Fields should be private
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <inheritdoc cref="ItemBehaviour.Settings"/>.
        public FirearmSettings FirearmSettings => Settings.Cast<FirearmSettings>();

        /// <inheritdoc cref="EBehaviour{T}.Owner"/>
        public Firearm Firearm => Owner.Cast<Firearm>();

        /// <summary>
        /// Gets or sets the replicated clip.
        /// </summary>
        public ReplicatedProperty<Firearm, byte> ReplicatedClip { get; set; }

        /// <summary>
        /// Gets or sets the replicated max ammo.
        /// </summary>
        public ReplicatedProperty<Firearm, byte> ReplicatedMaxAmmo { get; set; }

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

            overrideReload = FirearmSettings.OverrideReload;

            ItemType ammoItemType = FirearmSettings.AmmoType;
            uint customAmmoId = FirearmSettings.CustomAmmoType;

            if (!overrideReload)
            {
                chamberSize = Firearm.Base.AmmoManagerModule switch
                {
                    AutomaticAmmoManager aam => aam.ChamberedAmount,
                    TubularMagazineAmmoManager tmam => tmam.ChamberedRounds,
                    PumpAction pa => pa.ChamberedRounds,
                    _ => 0,
                };

                ammoType = ammoItemType.IsAmmo() ? ammoItemType.GetAmmoType() : Firearm.AmmoType;
            }
            else
            {
                chamberSize = FirearmSettings.ChamberSize;

                if (itemType is ItemType.None)
                {
                    if (customAmmoId > 0 && CustomItem.TryGet(customAmmoId, out CustomItem _))
                        customAmmoType = customAmmoId;
                }
                else if (ammoItemType.IsAmmo())
                {
                    ammoType = ammoItemType.GetAmmoType();
                }
                else
                {
                    itemType = ammoItemType;
                }
            }

            if (!FirearmSettings.Attachments.IsEmpty())
                Firearm.AddAttachment(FirearmSettings.Attachments);

            Firearm.Recoil = FirearmSettings.RecoilSettings;

            ReplicatedMaxAmmo = new ReplicatedProperty<Firearm, byte>(
                firearm => firearm.MaxAmmo,
                (firearm, value) => firearm.MaxAmmo = value,
                Firearm);

            ReplicatedMaxAmmo.Send(FirearmSettings.MaxAmmo);
            ReplicatedMaxAmmo.Replicate();

            if (overrideReload)
            {
                ReplicatedClip = new ReplicatedProperty<Firearm, byte>(
                    firearm => firearm.Ammo,
                    (firearm, value) => firearm.Ammo = value,
                    Firearm);

                ReplicatedClip.Send(FirearmSettings.ClipSize);

                if (ReplicatedClip.ReplicatedValue > ReplicatedMaxAmmo.ReplicatedValue)
                    ReplicatedClip.Send(ReplicatedMaxAmmo.ReplicatedValue);

                ReplicatedClip.Replicate();
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
            Exiled.Events.Handlers.Player.UnloadingWeapon += OnInternalUnloading;
            Exiled.Events.Handlers.Item.ChangingAttachments += OnInternalChangingAttachments;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            Exiled.Events.Handlers.Player.ReloadingWeapon -= OnInternalReloading;
            Exiled.Events.Handlers.Player.Shooting -= OnInternalShooting;
            Exiled.Events.Handlers.Player.Shot -= OnInternalShot;
            Exiled.Events.Handlers.Player.Hurting -= OnInternalHurting;
            Exiled.Events.Handlers.Player.UnloadingWeapon -= OnInternalUnloading;
            Exiled.Events.Handlers.Item.ChangingAttachments -= OnInternalChangingAttachments;
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

        /// <summary>
        /// Handles unloading events for custom firearms.
        /// </summary>
        /// <param name="ev">The <see cref="UnloadingWeaponEventArgs"/> containing information about the unloading event.</param>
        protected virtual void OnUnloading(UnloadingWeaponEventArgs ev)
        {
        }

        /// <summary>
        /// Handles attachments events for custom firearms.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingAttachmentsEventArgs"/> containing information about the attachments event.</param>
        protected virtual void OnChangingAttachments(ChangingAttachmentsEventArgs ev)
        {
        }

        /// <inheritdoc cref="OnReloading(ReloadingWeaponEventArgs)"/>
        private protected virtual void OnInternalReloading(ReloadingWeaponEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            OnReloading(ev);

            if (!ev.IsAllowed || (ev.IsAllowed = overrideReload))
                return;

            byte clipSize = FirearmSettings.ClipSize;
            byte remainingClip = ReplicatedClip.ReplicatedValue;

            if (remainingClip >= clipSize)
                return;

            if (remainingClip > ReplicatedMaxAmmo.ReplicatedValue)
                remainingClip = ReplicatedMaxAmmo.ReplicatedValue;

            ushort unscaledAmmoAmount = 0;

            if (ammoType is not AmmoType.None)
                unscaledAmmoAmount = ev.Player.GetAmmo(ammoType);
            else if (itemType is not ItemType.None)
                unscaledAmmoAmount = (ushort)ev.Player.Items.Where(i => !Check(i) && i.Type == itemType).Count();
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

            ReplicatedClip.Send((byte)(ReplicatedClip.ReplicatedValue + amountToReload));
            ReplicatedClip.Replicate();
        }

        /// <inheritdoc cref="OnShooting(ShootingEventArgs)"/>
        private protected virtual void OnInternalShooting(ShootingEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            if (overrideReload)
            {
                if (ReplicatedClip.ReplicatedValue <= 0)
                {
                    ReplicatedClip.Send(0);
                    ReplicatedClip.Replicate();
                    ev.IsAllowed = false;
                    return;
                }

                ReplicatedClip.Replicate();
                ReplicatedClip.Send((byte)(ReplicatedClip.ReplicatedValue - chamberSize));
            }

            OnShooting(ev);
        }

        /// <inheritdoc cref="OnUnloading(UnloadingWeaponEventArgs)"/>
        private protected virtual void OnInternalUnloading(UnloadingWeaponEventArgs ev)
        {
            if (!Check(ev.Item) || !overrideReload)
                return;

            OnUnloading(ev);

            if (!ev.IsAllowed)
                return;

            ReplicatedClip.Replicate((byte)chamberSize);

            if (ammoType is not AmmoType.None)
            {
                ItemOwner.AddAmmo(ammoType, (byte)(ReplicatedClip.ReplicatedValue - chamberSize));
            }
            else if (itemType is not ItemType.None)
            {
                for (int i = 0; i < ReplicatedClip.ReplicatedValue - chamberSize; i++)
                {
                    Item item = Item.Create(itemType);

                    if (ItemOwner.Items.Count >= 8)
                        item.CreatePickup(ItemOwner.Position);
                    else
                        item.Give(ItemOwner);
                }
            }
            else if (customAmmoType > 0)
            {
                ev.Player.Cast<Pawn>().AddAmmo(customAmmoType, (byte)(ReplicatedClip.ReplicatedValue - chamberSize));
            }

            ReplicatedClip.Send(0);
        }

        /// <inheritdoc cref="OnShot(ShotEventArgs)"/>
        private protected virtual void OnInternalShot(ShotEventArgs ev)
        {
            if (!Check(ev.Item))
                return;

            OnShot(ev);
        }

        /// <inheritdoc cref="OnChangingAttachments(ChangingAttachmentsEventArgs)"/>
        private protected virtual void OnInternalChangingAttachments(ChangingAttachmentsEventArgs ev)
        {
            if (!Check(ev.Item) || !overrideReload)
                return;

            OnChangingAttachments(ev);

            if (ev.IsAllowed)
                return;

            int magModifier = (int)Firearm.Base.AttachmentsValue(AttachmentParam.MagazineCapacityModifier);

            Timing.CallDelayed(0.5f, () =>
            {
                int curModifier = (int)Firearm.Base.AttachmentsValue(AttachmentParam.MagazineCapacityModifier);
                int amount = curModifier == magModifier ? 0 : curModifier > magModifier ? curModifier - magModifier : magModifier - curModifier;
                ReplicatedMaxAmmo.Send((byte)(ReplicatedMaxAmmo.ReplicatedValue + amount));
                ReplicatedMaxAmmo.Replicate();
            });
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