// -----------------------------------------------------------------------
// <copyright file="FirearmDamageHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.DamageHandlers
{
    using System.Diagnostics;

    using Enums;

    using Extensions;

    using Items;

    using PlayerStatsSystem;

    using BaseFirearmHandler = PlayerStatsSystem.FirearmDamageHandler;
    using BaseHandler = PlayerStatsSystem.DamageHandlerBase;

    /// <summary>
    /// A wrapper to easily manipulate the behavior of <see cref="BaseHandler"/>.
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public sealed class FirearmDamageHandler : AttackerDamageHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FirearmDamageHandler"/> class.
        /// </summary>
        /// <param name="baseHandler"><inheritdoc cref="DamageHandlerBase.Base"/></param>
        /// <param name="item">The <see cref="Items.Item"/> to be set.</param>
        /// <param name="target">The target to be set.</param>
        public FirearmDamageHandler(Item item, Player target, BaseHandler baseHandler)
            : base(target, baseHandler)
        {
            Item = item;
        }

        /// <inheritdoc/>
        public override DamageType Type => Item switch
        {
            Firearm _ when DamageTypeExtensions.ItemConversion.ContainsKey(Item.Type) => DamageTypeExtensions.ItemConversion[Item.Type],
            MicroHid _ => DamageType.MicroHid,
            _ => DamageType.Firearm,
        };

        /// <summary>
        /// Gets or sets the <see cref="Items.Item"/> used by the damage handler.
        /// </summary>
        public Item Item { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="HitboxType"/>.
        /// </summary>
        public HitboxType Hitbox
        {
            get => (Base as BaseFirearmHandler).Hitbox;
            set => (Base as BaseFirearmHandler).Hitbox = value;
        }

        /// <summary>
        /// Gets the penetration.
        /// </summary>
        public float Penetration => (Base as BaseFirearmHandler)._penetration;

        /// <summary>
        /// Gets a value indicating whether the human hitboxes should be used.
        /// </summary>
        public bool UseHumanHitboxes => (Base as BaseFirearmHandler)._useHumanHitboxes;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get
            {
                string debugView = $"FirearmDamageHandler Damage = {Damage}";
                if (Attacker is not null)
                    debugView += $" Attacker = {Attacker.Nickname}";
                if (Target is not null)
                    debugView += $" Target = {Target.Nickname}";
                return debugView;
            }
        }

        /// <inheritdoc/>
        public override void ProcessDamage(Player player)
        {
            switch (Base)
            {
                case BaseFirearmHandler firearmDamageHandler:
                    firearmDamageHandler.ProcessDamage(player.ReferenceHub);
                    break;
                case MicroHidDamageHandler microHidHandler:
                    microHidHandler.ProcessDamage(player.ReferenceHub);
                    break;
            }
        }

        /// <inheritdoc/>
        public override string ToString() => $"{Target} {Damage} ({Type}) {(Attacker is not null ? Attacker.Nickname : "No one")} {(Item is not null ? Item.ToString() : "No item")}";
    }
}