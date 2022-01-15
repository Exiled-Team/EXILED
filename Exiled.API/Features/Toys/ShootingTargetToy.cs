// -----------------------------------------------------------------------
// <copyright file="ShootingTargetToy.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Toys
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Enums;

    using PlayerStatsSystem;

    using UnityEngine;

    using ShootingTarget = AdminToys.ShootingTarget;

    /// <summary>
    /// A wrapper class for <see cref="ShootingTarget"/>.
    /// </summary>
    public class ShootingTargetToy : AdminToy
    {
        private static readonly Dictionary<string, ShootingTargetType> TypeLookup = new Dictionary<string, ShootingTargetType>()
        {
            { "sportTargetPrefab", ShootingTargetType.Sport },
            { "dboyTargetPrefab", ShootingTargetType.ClassD },
            { "binaryTargetPrefab", ShootingTargetType.Binary },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="ShootingTargetToy"/> class.
        /// </summary>
        /// <param name="target"><inheritdoc cref="Base"/></param>
        public ShootingTargetToy(ShootingTarget target)
            : base(target, AdminToyType.ShootingTarget) => Base = target;

        /// <summary>
        /// Gets the base-game <see cref="ShootingTarget"/> for this target.
        /// </summary>
        public ShootingTarget Base { get; }

        /// <summary>
        /// Gets the <see cref="Interactables.Verification.IVerificationRule"/> for this target.
        /// </summary>
        public Interactables.Verification.IVerificationRule VerificationRule => Base.VerificationRule;

        /// <summary>
        /// Gets the bullseye location of the target.
        /// </summary>
        public Vector3 BullseyePosition
        {
            get => Base._bullsEye.position;
        }

        /// <summary>
        /// Gets the bullseye radius of the target.
        /// </summary>
        public float BullseyeRadius
        {
            get => Base._bullsEyeRadius;
        }

        /// <summary>
        /// Gets or sets the max health of the target.
        /// </summary>
        public int MaxHealth
        {
            get => Base._maxHp;
            set
            {
                if (!IsSynced)
                    throw new InvalidOperationException("Attempted to set MaxHealth while target was not in sync mode.");
                Base._maxHp = value;
                Base.RpcSendInfo(MaxHealth, AutoResetTime);
            }
        }

        /// <summary>
        /// Gets or sets the remaining health of the target.
        /// </summary>
        public float Health
        {
            get => Base._hp;
            set
            {
                if (!IsSynced)
                    throw new InvalidOperationException("Attempted to set Health while target was not in sync mode.");
                Base._hp = value;
            }
        }

        /// <summary>
        /// Gets or sets the remaining health of the target.
        /// </summary>
        public int AutoResetTime
        {
            get => Base._autoDestroyTime;
            set
            {
                if (!IsSynced)
                    throw new InvalidOperationException("Attempted to set AutoResetTime while target was not in sync mode.");
                Base._autoDestroyTime = Mathf.Max(0, value);
                Base.RpcSendInfo(MaxHealth, AutoResetTime);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the target is in sync mode.
        /// </summary>
        public bool IsSynced
        {
            get => Base.Network_syncMode;
            set => Base.Network_syncMode = value;
        }

        /// <summary>
        /// Gets the type of the target.
        /// </summary>
        public ShootingTargetType Type => TypeLookup.TryGetValue(Base.gameObject.name.Substring(0, Base.gameObject.name.Length - 7), out ShootingTargetType type) ? type : ShootingTargetType.Unknown;

        /// <summary>
        /// Creates a new <see cref="ShootingTargetToy"/>.
        /// </summary>
        /// <param name="type">The <see cref="ShootingTargetType"/> of the target.</param>
        /// <returns>The new <see cref="ShootingTargetToy"/>.</returns>
        public static ShootingTargetToy Create(ShootingTargetType type)
        {
            switch (type)
            {
                case ShootingTargetType.ClassD:
                    return new ShootingTargetToy(UnityEngine.Object.Instantiate(ToysHelper.DboyShootingTargetObject));
                case ShootingTargetType.Binary:
                    return new ShootingTargetToy(UnityEngine.Object.Instantiate(ToysHelper.BinaryShootingTargetObject));
                default:
                    return new ShootingTargetToy(UnityEngine.Object.Instantiate(ToysHelper.SportShootingTargetObject));
            }
        }

        /// <summary>
        /// Clears the target and resets its health.
        /// </summary>
        public void Clear() => Base.ClearTarget();

        /// <summary>
        /// Damages the target with the given damage, item, footprint, and hit location.
        /// </summary>
        /// <param name="damage">The damage to be dealt.</param>
        /// <param name="damageHandler">The <see cref="DamageHandlerBase"/> dealing the damage.</param>
        /// <param name="exactHit">The exact location of the hit.</param>
        /// <returns>Whether or not the damage was sent.</returns>
        public bool Damage(float damage, DamageHandlerBase damageHandler, Vector3 exactHit) => Base.Damage(damage, damageHandler, exactHit);
    }
}
