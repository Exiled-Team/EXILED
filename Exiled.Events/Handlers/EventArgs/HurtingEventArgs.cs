// -----------------------------------------------------------------------
// <copyright file="HurtingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.EventArgs
{
    using System;
    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before a player gets hurt.
    /// </summary>
    public class HurtingEventArgs : EventArgs
    {
        private PlayerStats.HitInfo hitInformations;
        private DamageTypes.DamageType damageType = DamageTypes.None;

        /// <summary>
        /// Initializes a new instance of the <see cref="HurtingEventArgs"/> class.
        /// </summary>
        /// <param name="attacker"><inheritdoc cref="Attacker"/></param>
        /// <param name="target"><inheritdoc cref="Target"/></param>
        /// <param name="hitInformations"><inheritdoc cref="HitInformations"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public HurtingEventArgs(Player attacker, Player target, PlayerStats.HitInfo hitInformations, bool isAllowed = true)
        {
            Attacker = attacker;
            Target = target;
            HitInformations = hitInformations;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the attacker player.
        /// </summary>
        public Player Attacker { get; private set; }

        /// <summary>
        /// Gets the target player, who is going to be hurt.
        /// </summary>
        public Player Target { get; private set; }

        /// <summary>
        /// Gets the hit informations.
        /// </summary>
        public PlayerStats.HitInfo HitInformations
        {
            get => hitInformations;
            private set => hitInformations = value;
        }

        /// <summary>
        /// Gets the time at which the player was hurt.
        /// </summary>
        public int Time => hitInformations.Time;

        /// <summary>
        /// Gets the damage type.
        /// </summary>
        public DamageTypes.DamageType DamageType
        {
            get
            {
                if (damageType == DamageTypes.None)
                    damageType = DamageTypes.FromIndex(hitInformations.Tool);

                return damageType;
            }
        }

        /// <summary>
        /// Gets the tool that damaged the player.
        /// </summary>
        public int Tool => hitInformations.Tool;

        /// <summary>
        /// Gets or sets the amount of inflicted damage.
        /// </summary>
        public float Amount
        {
            get => hitInformations.Amount;
            set => hitInformations.Amount = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
