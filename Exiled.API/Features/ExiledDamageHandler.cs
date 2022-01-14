// -----------------------------------------------------------------------
// <copyright file="ExiledDamageHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using Footprinting;

    using PlayerStatsSystem;

    /// <summary>
    /// A special <see cref="AttackerDamageHandler"/> that allows you to define the attacker.
    /// </summary>
    public class ExiledDamageHandler : AttackerDamageHandler
    {
        private Footprint attacker;
        private float damage;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExiledDamageHandler"/> class.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> who is dealing the damage.</param>
        /// <param name="amount">The amount of damage to deal.</param>
        /// <param name="cassieAnnouncement">The announcement cassie will make if this damage kills the victim.</param>
        /// <param name="logText">The text to appear in server logs.</param>
        /// <param name="allowSelfDamage"><see langword="true"/> if the damage can be dealt to the attacker. Otherwise <see langwod="false"/>.</param>
        public ExiledDamageHandler(Player player, float amount, string cassieAnnouncement = "default", string logText = "default", bool allowSelfDamage = true)
        {
            attacker = player?.Footprint ?? Server.Host.Footprint;
            damage = amount;
            AllowSelfDamage = allowSelfDamage;
            ServerLogsText = logText;
        }

        /// <summary>
        /// Gets or sets the <see cref="Footprint"/> of the attacker.
        /// </summary>
        public override Footprint Attacker
        {
            get => attacker;
            set => attacker = value;
        }

        /// <summary>
        /// Gets the text to show in the server logs.
        /// </summary>
        public override string ServerLogsText { get; }

        /// <summary>
        /// Gets or sets the amount of damage to be done.
        /// </summary>
        public override float Damage
        {
            get => damage;
            set => damage = value;
        }

        /// <summary>
        /// Gets a value indicating whether damage can be dealt to themselves.
        /// </summary>
        public override bool AllowSelfDamage { get; }
    }
}
