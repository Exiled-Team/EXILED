// -----------------------------------------------------------------------
// <copyright file="FirearmDamageHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.DamageHandlers
{
    using Enums;

    using Extensions;

    using Items;

    using PlayerStatsSystem;

    using BaseFirearmHandler = PlayerStatsSystem.FirearmDamageHandler;
    using BaseHandler = PlayerStatsSystem.DamageHandlerBase;

    /// <summary>
    /// A wrapper to easily manipulate the behavior of <see cref="BaseHandler"/>.
    /// </summary>
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
            get => As<BaseFirearmHandler>().Hitbox;
            set => As<BaseFirearmHandler>().Hitbox = value;
        }

        /// <summary>
        /// Gets the penetration.
        /// </summary>
        public float Penetration => As<BaseFirearmHandler>()._penetration;

        /// <summary>
        /// Gets a value indicating whether the human hitboxes should be used.
        /// </summary>
        public bool UseHumanHitboxes => As<BaseFirearmHandler>()._useHumanHitboxes;

        /// <inheritdoc/>
        public override void ProcessDamage(Player player)
        {
            if (Is(out BaseFirearmHandler firearmHandler))
                firearmHandler.ProcessDamage(player.ReferenceHub);
            else if (Is(out MicroHidDamageHandler microHidHandler))
                microHidHandler.ProcessDamage(player.ReferenceHub);
        }

        /// <inheritdoc/>
        public override string ToString() => $"{Target} {Damage} ({Type}) {(Attacker is not null ? Attacker.Nickname : "No one")} {(Item is not null ? Item.ToString() : "No item")}";
    }
}