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
    using Exiled.Events.EventArgs;

    using InventorySystem.Items.Firearms;
    using InventorySystem.Items.Firearms.Attachments;
    using InventorySystem.Items.Firearms.Attachments.Components;
    using InventorySystem.Items.Firearms.BasicMessages;

    using MEC;

    using UnityEngine;

    using static CustomItems;

    using Firearm = Exiled.API.Features.Items.Firearm;
    using Player = Exiled.API.Features.Player;

    /// <inheritdoc />
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
                if (!value.IsWeapon() && value != ItemType.None)
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

        /// <inheritdoc/>
        [Obsolete("Use Spawn(Vector3, Player) instead.", true)]
        public override Pickup Spawn(Vector3 position)
        {
            Item item = Item.Create(Type);

            if (item is null)
            {
                Log.Debug($"{nameof(Spawn)}: Item is null.", Instance.Config.Debug);
                return null;
            }

            if (item is Firearm firearm && Attachments is not null && !Attachments.IsEmpty())
                firearm.AddAttachment(Attachments);

            Pickup pickup = item.Spawn(position);
            if (pickup is null)
            {
                Log.Debug($"{nameof(Spawn)}: Pickup is null.");
                return null;
            }

            pickup.Weight = Weight;

            TrackedSerials.Add(pickup.Serial);
            return pickup;
        }

        /// <inheritdoc />
        public override Pickup Spawn(Vector3 position, Player previousOwner = null)
        {
            Item item = Item.Create(Type);

            if (item is null)
            {
                Log.Debug($"{nameof(Spawn)}: Item is null.", Instance.Config.Debug);
                return null;
            }

            if (item is Firearm firearm && Attachments is not null && !Attachments.IsEmpty())
                firearm.AddAttachment(Attachments);

            Pickup pickup = item.Spawn(position);
            if (pickup is null)
            {
                Log.Debug($"{nameof(Spawn)}: Pickup is null.", Instance.Config.Debug);
                return null;
            }

            pickup.Weight = Weight;
            if (previousOwner is not null)
                pickup.PreviousOwner = previousOwner;

            TrackedSerials.Add(pickup.Serial);
            return pickup;
        }

        /// <inheritdoc/>
        [Obsolete("Use Spawn(Vector3, Item, Player) instead.", true)]
        public override Pickup Spawn(Vector3 position, Item item)
        {
            if (item is Firearm firearm)
            {
                if (!Attachments.IsEmpty())
                    firearm.AddAttachment(Attachments);
                byte ammo = firearm.Ammo;
                Log.Debug($"{nameof(Name)}.{nameof(Spawn)}: Spawning weapon with {ammo} ammo.", Instance.Config.Debug);
                Pickup pickup = firearm.Spawn(position);

                TrackedSerials.Add(pickup.Serial);

                Timing.CallDelayed(1f, () =>
                {
                    if (pickup.Base is FirearmPickup firearmPickup)
                    {
                        firearmPickup.Status = new FirearmStatus(ammo, firearmPickup.Status.Flags, firearmPickup.Status.Attachments);
                        firearmPickup.NetworkStatus = firearmPickup.Status;
                        Log.Debug($"{nameof(Name)}.{nameof(Spawn)}: Spawned item has: {firearmPickup.Status.Ammo}", Instance.Config.Debug);
                    }
                });

                return pickup;
            }
            else
            {
                return base.Spawn(position, item);
            }
        }

        /// <inheritdoc />
        public override Pickup Spawn(Vector3 position, Item item, Player previousOwner = null)
        {
            if (item is Firearm firearm)
            {
                if (!Attachments.IsEmpty())
                    firearm.AddAttachment(Attachments);
                byte ammo = firearm.Ammo;
                Log.Debug($"{nameof(Name)}.{nameof(Spawn)}: Spawning weapon with {ammo} ammo.", Instance.Config.Debug);
                Pickup pickup = firearm.Spawn(position);

                if (previousOwner is not null)
                    pickup.PreviousOwner = previousOwner;

                TrackedSerials.Add(pickup.Serial);

                Timing.CallDelayed(1f, () =>
                {
                    if (pickup.Base is FirearmPickup firearmPickup)
                    {
                        firearmPickup.Status = new FirearmStatus(ammo, firearmPickup.Status.Flags, firearmPickup.Status.Attachments);
                        firearmPickup.NetworkStatus = firearmPickup.Status;
                        Log.Debug($"{nameof(Name)}.{nameof(Spawn)}: Spawned item has: {firearmPickup.Status.Ammo}", Instance.Config.Debug);
                    }
                });

                return pickup;
            }
            else
            {
                return base.Spawn(position, item, previousOwner);
            }
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

            Log.Debug($"{nameof(Give)}: Adding {item.Serial} to tracker.", Instance.Config.Debug);
            TrackedSerials.Add(item.Serial);

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
            if (ev.IsAllowed)
                ev.Amount = ev.Target.Role == RoleType.Scp106 ? Damage * 0.1f : Damage;
        }

        private void OnInternalReloading(ReloadingWeaponEventArgs ev)
        {
            if (!Check(ev.Firearm))
                return;

            Log.Debug($"{nameof(Name)}.{nameof(OnInternalReloading)}: Reloading weapon. Calling external reload event..", Instance.Config.Debug);
            OnReloading(ev);

            Log.Debug($"{nameof(Name)}.{nameof(OnInternalReloading)}: External event ended. {ev.IsAllowed}", Instance.Config.Debug);
            if (!ev.IsAllowed)
            {
                Log.Debug($"{nameof(Name)}.{nameof(OnInternalReloading)}: External event turned is allowed to false, returning.", Instance.Config.Debug);
                return;
            }

            Log.Debug($"{nameof(Name)}.{nameof(OnInternalReloading)}: Continuing with internal reload..", Instance.Config.Debug);
            ev.IsAllowed = false;

            byte remainingClip = ((Firearm)ev.Player.CurrentItem).Ammo;

            if (remainingClip >= ClipSize)
                return;

            Log.Debug($"{ev.Player.Nickname} ({ev.Player.UserId}) [{ev.Player.Role}] is reloading a {Name} ({Id}) [{Type} ({remainingClip}/{ClipSize})]!", Instance.Config.Debug);

            AmmoType ammoType = ((Firearm)ev.Player.CurrentItem).AmmoType;

            if (!ev.Player.Ammo.ContainsKey(ammoType.GetItemType()))
            {
                Log.Debug($"{nameof(Name)}.{nameof(OnInternalReloading)}: {ev.Player.Nickname} does not have ammo to reload this weapon.", Instance.Config.Debug);
                return;
            }

            ev.Player.Connection.Send(new RequestMessage(ev.Firearm.Serial, RequestType.Reload));

            byte amountToReload = (byte)Math.Min(ClipSize - remainingClip, ev.Player.Ammo[ammoType.GetItemType()]);

            if (amountToReload <= 0)
                return;

            ev.Player.ReferenceHub.playerEffectsController.GetEffect<CustomPlayerEffects.Invisible>().Intensity = 0;

            ev.Player.Ammo[ammoType.GetItemType()] -= amountToReload;
            ((Firearm)ev.Player.CurrentItem).Ammo = (byte)(((Firearm)ev.Player.CurrentItem).Ammo + amountToReload);

            Log.Debug($"{ev.Player.Nickname} ({ev.Player.UserId}) [{ev.Player.Role}] reloaded a {Name} ({Id}) [{Type} ({((Firearm)ev.Player.CurrentItem).Ammo}/{ClipSize})]!", Instance.Config.Debug);
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
            if (ev.Attacker is null)
            {
                Log.Debug($"{Name}: {nameof(OnInternalHurting)}: Attacker null", Instance.Config.Debug);
                return;
            }

            if (ev.Target is null)
            {
                Log.Debug($"{Name}: {nameof(OnInternalHurting)}: target null", Instance.Config.Debug);
                return;
            }

            if (!Check(ev.Attacker.CurrentItem))
            {
                Log.Debug($"{Name}: {nameof(OnInternalHurting)}: !Check()", Instance.Config.Debug);
                return;
            }

            if (ev.Attacker == ev.Target)
            {
                Log.Debug($"{Name}: {nameof(OnInternalHurting)}: attacker == target", Instance.Config.Debug);
                return;
            }

            if (ev.Handler is null)
            {
                Log.Debug($"{Name}: {nameof(OnInternalHurting)}: Handler null", Instance.Config.Debug);
                return;
            }

            if (!ev.Handler.CustomBase.BaseIs(out FirearmDamageHandler firearmDamageHandler))
            {
                Log.Debug($"{Name}: {nameof(OnInternalHurting)}: Handler not firearm", Instance.Config.Debug);
                return;
            }

            if (!Check(firearmDamageHandler.Item))
            {
                Log.Debug($"{Name}: {nameof(OnInternalHurting)}: type != type", Instance.Config.Debug);
                return;
            }

            if (!FriendlyFire && ev.Attacker.Role.Team == ev.Target.Role.Team)
            {
                Log.Debug($"{Name}: {nameof(OnInternalHurting)}: FF is disabled for this weapon!", Instance.Config.Debug);
                return;
            }

            OnHurting(ev);
        }
    }
}
