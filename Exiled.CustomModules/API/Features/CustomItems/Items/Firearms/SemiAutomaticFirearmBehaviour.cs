// -----------------------------------------------------------------------
// <copyright file="SemiAutomaticFirearmBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomItems.Items.Firearms
{
    using System.Collections.Generic;

    using Exiled.API.Features.Core.Generic;
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
        /// Gets or sets the firearm's fire rate.
        /// <para/>
        /// Automatic firearms won't be affected.
        /// </summary>
        public virtual byte FireRate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Enums.FiringMode"/>.
        /// </summary>
        public virtual FiringMode FiringMode { get; set; }

        /// <summary>
        /// Gets a value indicating whether the firearm can shoot.
        /// </summary>
        public bool CanShoot => FiringMode is not FiringMode.None && ReplicatedClip.ReplicatedValue > 0;

        /// <inheritdoc/>
        protected override void PostInitialize()
        {
            base.PostInitialize();

            FiringMode = FirearmSettings.FiringMode;

            ReplicatedClip = new ReplicatedProperty<Firearm, int>(
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
                int burstLength = ReplicatedClip.ReplicatedValue >= FirearmSettings.BurstLength ? FirearmSettings.BurstLength : ReplicatedClip.Value;
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