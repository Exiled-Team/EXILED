// -----------------------------------------------------------------------
// <copyright file="FirearmDamageHandler.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.API.Features.DamageHandlers
{
    using System.Collections.Generic;

    using SEXiled.API.Enums;
    using SEXiled.API.Features.Items;

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
            : base(target, baseHandler) => Item = item;

        /// <inheritdoc/>
        public override DamageType Type
        {
            get
            {
                switch (Item)
                {
                    case Firearm _ when ItemConversion.ContainsKey(Item.Type):
                        return ItemConversion[Item.Type];
                    case MicroHid _:
                        return DamageType.MicroHid;
                    default:
                        return DamageType.Firearm;
                }
            }
        }

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

        /// <summary>
        /// Gets conversion information between <see cref="ItemType"/>s and <see cref="DamageType"/>s.
        /// </summary>
        internal static Dictionary<ItemType, DamageType> ItemConversion { get; } = new Dictionary<ItemType, DamageType>
        {
            { ItemType.GunCrossvec, DamageType.Crossvec },
            { ItemType.GunLogicer, DamageType.Logicer },
            { ItemType.GunRevolver, DamageType.Revolver },
            { ItemType.GunShotgun, DamageType.Shotgun },
            { ItemType.GunAK, DamageType.AK },
            { ItemType.GunCOM15, DamageType.Com15 },
            { ItemType.GunCOM18, DamageType.Com18 },
            { ItemType.GunFSP9, DamageType.Fsp9 },
            { ItemType.GunE11SR, DamageType.E11Sr },
        };

        /// <inheritdoc/>
        public override void ProcessDamage(Player player)
        {
            if (Is(out BaseFirearmHandler firearmHandler))
                firearmHandler.ProcessDamage(player.ReferenceHub);
            else if (Is(out MicroHidDamageHandler microHidHandler))
                microHidHandler.ProcessDamage(player.ReferenceHub);
        }

        /// <inheritdoc/>
        public override string ToString() => $"{Target} {Damage} ({Type}) {(Attacker != null ? Attacker.Nickname : "No one")} {(Item != null ? Item.ToString() : "No item")}";
    }
}
