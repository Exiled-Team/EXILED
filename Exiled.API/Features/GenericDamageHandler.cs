// -----------------------------------------------------------------------
// <copyright file="GenericDamageHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Features.Items;

    using Footprinting;

    using PlayerStatsSystem;

    using Subtitles;

    /// <summary>
    /// Allows generic damage to player.
    /// </summary>
    internal class GenericDamageHandler : PlayerStatsSystem.AttackerDamageHandler
    {
        private Player player;
        private Player attacker;
        private float amount;
        private DamageType damageType;
        private DamageHandlers.DamageHandlerBase.CassieAnnouncement customCassieAnnouncement;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericDamageHandler"/> class.
        /// Transform input data to custom generic handler.
        /// </summary>
        /// <param name="player"> Current player (Target). </param>
        /// <param name="attacker"> Attacker. </param>
        /// <param name="amount"> Damage quantity. </param>
        /// <param name="damageType"> Damage type. </param>
        /// <param name="cassieAnnouncement"> Custom cassie announcment. </param>
        public GenericDamageHandler(Player player, Player attacker, float amount, DamageType damageType, DamageHandlers.DamageHandlerBase.CassieAnnouncement cassieAnnouncement)
            : base()
        {
            this.player = player;
            this.attacker = attacker;
            this.amount = amount;
            this.damageType = damageType;
            this.customCassieAnnouncement = cassieAnnouncement;

            this.Attacker = attacker.Footprint;
            this.AllowSelfDamage = true;
            this.Damage = amount;
            this.ServerLogsText = $"You were damaged by {damageType}";

            // Base = new CustomReasonDamageHandler($"You were damaged by {damageType}", amount, string.IsNullOrEmpty(cassieAnnouncement?.Announcement) ? $"{player.Nickname} killed by {attacker.Nickname} utilizing {damageType}" : cassieAnnouncement.Announcement);
        }

        /// <summary>
        /// Gets or sets custom base.
        /// </summary>
        public DamageHandlerBase Base { get; set; }

        /// <summary>
        /// Gets or sets current attacker.
        /// </summary>
        public override Footprint Attacker { get; set; }

        /// <summary>
        /// Gets a value indicating whether allow self damage.
        /// </summary>
        public override bool AllowSelfDamage { get; }

        /// <inheritdoc />
        public override float Damage { get; set; }

        /// <inheritdoc />
        public override string ServerLogsText { get; }

        /// <summary>
        /// Process damage for this custom damage source.
        /// </summary>
        /// <param name="ply"> Current player reference hub. </param>
        public override void ProcessDamage(ReferenceHub ply)
        {
            base.ProcessDamage(ply);
        }

        /// <summary>
        /// Custom Exiled process damage.
        /// </summary>
        /// <param name="ply"> Current player hub. </param>
        /// <returns> Handlers for processing. </returns>
        public override HandlerOutput ApplyDamage(ReferenceHub ply)
        {
            HandlerOutput output = base.ApplyDamage(ply);
            if(output == HandlerOutput.Death)
            {
                Cassie.Message(this.customCassieAnnouncement?.Announcement ?? $" {this.player} KILLED BY UNKNOWN CAUSE ");
            }

            return output;
        }
    }
}
