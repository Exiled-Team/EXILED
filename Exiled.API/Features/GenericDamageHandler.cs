// -----------------------------------------------------------------------
// <copyright file="GenericDamageHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using Exiled.API.Enums;

    using Footprinting;

    /// <summary>
    /// Allows generic damage to player.
    /// </summary>
    internal class GenericDamageHandler : PlayerStatsSystem.CustomReasonDamageHandler
    {
        private Player player;
        private Player attacker;
        private float amount;
        private DamageType damageType;

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
            : base($"You were damaged by {damageType}")
        {
            this.player = player;
            this.attacker = attacker;
            this.amount = amount;
            this.damageType = damageType;

            this.Attacker = attacker.Footprint;
            this.AllowSelfDamage = true;
            this.Damage = amount;
            this.ServerLogsText = $"You were damaged by {damageType}";
            Log.Info($"What is cassie {this.CassieDeathAnnouncement.Announcement}");
            /*
             *_deathReason = customReason;
            Damage = damage;
            _cassieAnnouncement = new CassieAnnouncement();
            _cassieAnnouncement.Announcement = customCassieAnnouncement;
            _cassieAnnouncement.SubtitleParts = new SubtitlePart[1]
            {
                new SubtitlePart(SubtitleType.Custom, new string[1]
                {
                    customCassieAnnouncement
                })
            };
            */
        }

        /// <inheritdoc />
        public override CassieAnnouncement CassieDeathAnnouncement
        {
            get
            {
                CassieAnnouncement baseAnnouncement = base.CassieDeathAnnouncement;
                baseAnnouncement.Announcement += $" utilizing {this.damageType}";
                baseAnnouncement.SubtitleParts = new Subtitles.SubtitlePart[]
                {
                    new Subtitles.SubtitlePart(Subtitles.SubtitleType.Custom, new string[]
                    {
                        baseAnnouncement.Announcement,
                    }),
                };
                return baseAnnouncement;
            }
        }

        /// <inheritdoc />
        public override Footprint Attacker { get; set; }

        /// <inheritdoc />
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
            return HandlerOutput.Death;             // return base.ApplyDamage(ply);
        }
    }
}
