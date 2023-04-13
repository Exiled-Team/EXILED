// -----------------------------------------------------------------------
// <copyright file="AttackerDamage.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Damage.Attacker
{
    using Footprinting;
    using PlayerStatsSystem;

    /// <summary>
    /// A wrapper class for AttackerDamageHandler.
    /// </summary>
    public class AttackerDamage : StandardDamage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttackerDamage"/> class.
        /// </summary>
        /// <param name="damageHandler">The base <see cref="AttackerDamageHandler"/> class.</param>
        internal AttackerDamage(AttackerDamageHandler damageHandler)
            : base(damageHandler)
        {
            Base = damageHandler;
        }

        /// <summary>
        /// Gets the <see cref="AttackerDamageHandler"/> that this class is encapsulating.
        /// </summary>
        public new AttackerDamageHandler Base { get; }

        /// <summary>
        /// Gets a value indicating whether .
        /// </summary>
        public bool IgnoreFriendlyFireDetector => Base.IgnoreFriendlyFireDetector;

        /// <summary>
        /// Gets or sets a value indicating whether .
        /// </summary>
        public bool IsFriendlyFire
        {
            get => Base.IsFriendlyFire;
            set => Base.IsFriendlyFire = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether .
        /// </summary>
        public bool IsSuicide
        {
            get => Base.IsSuicide;
            set => Base.IsSuicide = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether .
        /// </summary>
        public Footprint AttackerFootprint
        {
            get => Base.Attacker;
            set => Base.Attacker = value;
        }

        /// <summary>
        /// Gets or sets .
        /// </summary>
        public Player Attacker
        {
            get => Player.Get(AttackerFootprint);
            set => Base.Attacker = value.Footprint;
        }

        /// <inheritdoc/>
        public override string ToString() => $"{Damage} ({Type}) [{Hitbox}] -{AttackerFootprint.Nickname}-";
    }
}
