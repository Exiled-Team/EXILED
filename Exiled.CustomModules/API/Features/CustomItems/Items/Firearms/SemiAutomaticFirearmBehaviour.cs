// -----------------------------------------------------------------------
// <copyright file="SemiAutomaticFirearmBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomItems.Items.Firearms
{
    using System.Collections.Generic;

    using Exiled.API.Enums;
    using Exiled.API.Features.Core.Generics;
    using Exiled.API.Features.Items;
    using Exiled.CustomModules.API.Enums;
    using Exiled.Events.EventArgs.Player;

    using MEC;

    /// <summary>
    /// Represents the base class for custom semi automatic firearm behaviors.
    /// </summary>
    /// <remarks>
    /// This class extends <see cref="FirearmBehaviour"/>.
    /// <br/>It provides a foundation for creating custom behaviors associated with in-game firearms.
    /// </remarks>
    public abstract class SemiAutomaticFirearmBehaviour : FirearmBehaviour
    {
        /// <summary>
        /// Gets or sets the replicated clip.
        /// </summary>
        public ReplicatedProperty<Firearm, byte> ReplicatedClip { get; set; }

        /// <summary>
        /// Gets or sets the firearm's fire rate.
        /// <para/>
        /// Automatic firearms won't be affected.
        /// </summary>
        public byte FireRate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Enums.FiringMode"/>.
        /// </summary>
        public FiringMode FiringMode { get; set; }

        /// <summary>
        /// Gets a value indicating whether the firearm can shoot.
        /// </summary>
        public bool CanShoot => FiringMode is not FiringMode.None && ReplicatedClip.ReplicatedValue > 0;

        /// <inheritdoc/>
        protected override void PostInitialize()
        {
            base.PostInitialize();

            FiringMode = FirearmSettings.FiringMode;

            ReplicatedClip = new ReplicatedProperty<Firearm, byte>(
                firearm => firearm.Ammo,
                (firearm, value) => firearm.Ammo = value,
                Firearm);
        }

        /// <inheritdoc/>
        protected override void OnEndPlay()
        {
            base.OnEndPlay();

            ReplicatedClip.Destroy();
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            Exiled.Events.Handlers.Player.UnloadingWeapon += OnInternalUnloading;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            Exiled.Events.Handlers.Player.UnloadingWeapon -= OnInternalUnloading;
        }

        /// <summary>
        /// Handles unloading events for semi automatic firearms.
        /// </summary>
        /// <param name="ev">The <see cref="UnloadingWeaponEventArgs"/> containing information about the unloading event.</param>
        private protected virtual void OnInternalUnloading(UnloadingWeaponEventArgs ev)
        {
            ReplicatedClip.Replicate(1);

            if (ammoType is not AmmoType.None)
            {
                ItemOwner.AddAmmo(ammoType, (byte)(ReplicatedClip.ReplicatedValue - 1));
            }
            else if (itemType is not ItemType.None)
            {
                for (int i = 0; i < ReplicatedClip.ReplicatedValue - 1; i++)
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
                ev.Player.Cast<Pawn>().AddAmmo(customAmmoType, (byte)(ReplicatedClip.ReplicatedValue - 1));
            }

            ReplicatedClip.Send(0);
        }

        /// <inheritdoc/>
        private protected override void OnInternalShooting(ShootingEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            if (ReplicatedClip.ReplicatedValue <= 0)
            {
                ReplicatedClip.Send(0);
                ReplicatedClip.Replicate();
                ev.IsAllowed = false;
                return;
            }

            if (FiringMode is FiringMode.Automatic)
            {
                ReplicatedClip.Replicate();
                ReplicatedClip.Send((byte)(ReplicatedClip.ReplicatedValue - 1));
            }
            else if (FiringMode is FiringMode.Burst)
            {
                byte burstLength = ReplicatedClip.ReplicatedValue >= FirearmSettings.BurstLength ? FirearmSettings.BurstLength : ReplicatedClip.Value;
                ReplicatedClip.Send((byte)(ReplicatedClip.ReplicatedValue - burstLength));
                ReplicatedClip.Replicate(burstLength);
            }
            else if (FiringMode is FiringMode.SemiAutomatic)
            {
                ReplicatedClip.Send((byte)(ReplicatedClip.ReplicatedValue - 1));
                ReplicatedClip.Replicate(1);
            }
            else
            {
                ReplicatedClip.Replicate(0);
                ev.IsAllowed = false;
                return;
            }

            OnShooting(ev);
        }

        /// <inheritdoc/>
        private protected override void OnInternalShot(ShotEventArgs ev)
        {
            if (!Check(ev.Item))
                return;

            Timing.RunCoroutine(FireRateCooldown());

            OnShot(ev);
        }

        private IEnumerator<float> FireRateCooldown()
        {
            FiringMode previousFiringMode = FiringMode;
            FiringMode = FiringMode.None;

            yield return Timing.WaitForSeconds(FireRate);

            if (FiringMode is not FiringMode.None)
                yield break;

            FiringMode = previousFiringMode;
            ReplicatedClip.Replicate(1);
        }
    }
}