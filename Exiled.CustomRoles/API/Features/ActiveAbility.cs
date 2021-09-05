// -----------------------------------------------------------------------
// <copyright file="ActiveAbility.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.API.Features
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Features;

    using UnityEngine;

    using YamlDotNet.Serialization;

    /// <summary>
    /// The base class for active (on-use) abilities.
    /// </summary>
    public abstract class ActiveAbility : CustomAbility
    {
        /// <summary>
        /// Gets or sets how long the ability lasts.
        /// </summary>
        public abstract float Duration { get; set; }

        /// <summary>
        /// Gets or sets how long must go between ability uses.
        /// </summary>
        public abstract float Cooldown { get; set; }

        /// <summary>
        /// Gets the last time this ability was used.
        /// </summary>
        [YamlIgnore]
        public Dictionary<Player, DateTime> LastUsed { get; } = new Dictionary<Player, DateTime>();

        /// <summary>
        /// Checks to see if the ability is usable by the player.
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <param name="response">The response to send to the player.</param>
        /// <returns>True if the ability is usable.</returns>
        public virtual bool CanUseAbility(Player player, out string response)
        {
            if (!LastUsed.ContainsKey(player))
            {
                response = string.Empty;
                return true;
            }

            DateTime usableTime = LastUsed[player] + TimeSpan.FromSeconds(Cooldown);
            if (DateTime.Now > usableTime)
            {
                response = string.Empty;

                return true;
            }

            response =
                $"You must wait another {Math.Round((usableTime - DateTime.Now).TotalSeconds, 2)} seconds to use {Name}";

            return false;
        }

        /// <inheritdoc/>
        protected override void AbilityUsed(Player player)
        {
            LastUsed[player] = DateTime.Now;
            base.AbilityUsed(player);
        }
    }
}
