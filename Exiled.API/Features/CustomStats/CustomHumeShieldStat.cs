// -----------------------------------------------------------------------
// <copyright file="CustomHumeShieldStat.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.CustomStats
{
    using Mirror;
    using PlayerRoles.PlayableScps.HumeShield;
    using PlayerStatsSystem;
    using UnityEngine;
    using Utils.Networking;

    /// <summary>
    /// A custom version of <see cref="HumeShieldStat"/> which allows the player's max amount of HumeShield to be changed.
    /// </summary>
    public class CustomHumeShieldStat : HumeShieldStat
    {
        /// <inheritdoc/>
        public override float MaxValue => CustomMaxValue == default ? base.MaxValue : CustomMaxValue;

        /// <summary>
        /// Gets or sets the maximum amount of health the player will have.
        /// </summary>
        public float CustomMaxValue { get; set; }

        private float ShieldRegeneration => TryGetHsModule(out HumeShieldModuleBase controller) ? controller.HsRegeneration : 0;

        /// <inheritdoc/>
        public override void Update()
        {
            if (MaxValue == default)
            {
                base.Update();
                return;
            }

            if (!NetworkServer.active)
                return;

            if (_valueDirty)
            {
                new SyncedStatMessages.StatMessage()
                {
                    Stat = this,
                    SyncedValue = CurValue,
                }.SendToHubsConditionally(CanReceive);
                _lastSent = CurValue;
                _valueDirty = false;
            }

            if (ShieldRegeneration == 0)
                return;

            float delta = ShieldRegeneration * Time.deltaTime;

            if (delta > 0)
            {
                if (CurValue >= MaxValue)
                    return;
                CurValue = Mathf.MoveTowards(CurValue, MaxValue, delta);
                return;
            }

            if (CurValue <= 0)
                return;

            CurValue += delta;
        }
    }
}