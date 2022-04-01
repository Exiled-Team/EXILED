// -----------------------------------------------------------------------
// <copyright file="ShootingTarget.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features {
    using System;
    using System.Collections.Generic;

    using Exiled.API.Enums;

    using InventorySystem.Items;

    using Mirror;

    using PlayerStatsSystem;

    using UnityEngine;

    using BaseTarget = AdminToys.ShootingTarget;

    /// <summary>
    /// A wrapper class for <see cref="BaseTarget"/>.
    /// </summary>
    public class ShootingTarget {
        private static readonly Dictionary<string, ShootingTargetType> TypeLookup = new Dictionary<string, ShootingTargetType>()
        {
            { "sportTargetPrefab", ShootingTargetType.Sport },
            { "dboyTargetPrefab", ShootingTargetType.ClassD },
            { "binaryTargetPrefab", ShootingTargetType.Binary },
        };

        private static readonly Dictionary<BaseTarget, ShootingTarget> BaseToShootingTarget = new Dictionary<BaseTarget, ShootingTarget>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ShootingTarget"/> class.
        /// </summary>
        /// <param name="target"><inheritdoc cref="Base"/></param>
        public ShootingTarget(BaseTarget target) {
            Base = target;
            BaseToShootingTarget.Add(Base, this);
        }

        /// <summary>
        /// Gets the base-game <see cref="BaseTarget"/> for this target.
        /// </summary>
        public BaseTarget Base { get; }

        /// <summary>
        /// Gets the <see cref="Interactables.Verification.IVerificationRule"/> for this target.
        /// </summary>
        public Interactables.Verification.IVerificationRule VerificationRule => Base.VerificationRule;

        /// <summary>
        /// Gets the position of the target.
        /// </summary>
        public Vector3 Position {
            get => Base.transform.position;
        }

        /// <summary>
        /// Gets the bullseye location of the target.
        /// </summary>
        public Vector3 BullseyePosition {
            get => Base._bullsEye.position;
        }

        /// <summary>
        /// Gets the bullseye radius of the target.
        /// </summary>
        public float BullseyeRadius {
            get => Base._bullsEyeRadius;
        }

        /// <summary>
        /// Gets or sets the max health of the target.
        /// </summary>
        public int MaxHealth {
            get => Base._maxHp;
            set {
                if (!IsSynced)
                    throw new InvalidOperationException("Attempted to set MaxHealth while target was not in sync mode.");
                Base._maxHp = value;
                Base.RpcSendInfo(MaxHealth, AutoResetTime);
            }
        }

        /// <summary>
        /// Gets or sets the remaining health of the target.
        /// </summary>
        public float Health {
            get => Base._hp;
            set {
                if (!IsSynced)
                    throw new InvalidOperationException("Attempted to set Health while target was not in sync mode.");
                Base._hp = value;
            }
        }

        /// <summary>
        /// Gets or sets the remaining health of the target.
        /// </summary>
        public int AutoResetTime {
            get => Base._autoDestroyTime;
            set {
                if (!IsSynced)
                    throw new InvalidOperationException("Attempted to set AutoResetTime while target was not in sync mode.");
                Base._autoDestroyTime = Mathf.Max(0, value);
                Base.RpcSendInfo(MaxHealth, AutoResetTime);
            }
        }

        /// <summary>
        /// Gets or sets the size scale of the target.
        /// </summary>
        public Vector3 Scale {
            get => Base.gameObject.transform.localScale;
            set {
                GameObject gameObject = Base.gameObject;
                NetworkServer.UnSpawn(gameObject);
                gameObject.transform.localScale = value;
                NetworkServer.Spawn(gameObject);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the target is in sync mode.
        /// </summary>
        public bool IsSynced {
            get => Base.Network_syncMode;
            set => Base.Network_syncMode = value;
        }

        /// <summary>
        /// Gets the type of the target.
        /// </summary>
        public ShootingTargetType Type {
            get {
                return TypeLookup.TryGetValue(Base.gameObject.name.Substring(0, Base.gameObject.name.Length - 7), out ShootingTargetType type) ? type : ShootingTargetType.Unknown;
            }
        }

        /// <summary>
        /// Gets the ShootingTarget object associated with a specific <see cref="BaseTarget"/>, or creates a new one if there isn't one.
        /// </summary>
        /// <param name="shootingTarget"><inheritdoc cref="Base"/></param>
        /// <returns><inheritdoc cref="ShootingTarget"/></returns>
        public static ShootingTarget Get(BaseTarget shootingTarget) => BaseToShootingTarget.ContainsKey(shootingTarget)
            ? BaseToShootingTarget[shootingTarget]
            : new ShootingTarget(shootingTarget);

        /// <summary>
        /// Spawns a new shooting target of the given type at the given position and rotation.
        /// </summary>
        /// <param name="type">The <see cref="ShootingTargetType"/> of the target.</param>
        /// <param name="position">The position of the target.</param>
        /// <param name="rotation">The rotation of the target.</param>
        /// <returns>The <see cref="ShootingTarget"/> object associated with the <see cref="BaseTarget"/>.</returns>
        public static ShootingTarget Spawn(ShootingTargetType type, Vector3 position, Quaternion rotation = default) {
            foreach (GameObject gameObject in NetworkClient.prefabs.Values) {
                if (TypeLookup.TryGetValue(gameObject.name, out ShootingTargetType targetType) && targetType == type) {
                    BaseTarget target = gameObject.GetComponent<BaseTarget>();
                    GameObject targetGo = UnityEngine.Object.Instantiate(target.gameObject, position, rotation);
                    NetworkServer.Spawn(targetGo, ownerConnection: null);
                    return Get(target);
                }
            }

            return null;
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
        public bool Damage(float damage, DamageHandlerBase damageHandler, Vector3 exactHit) =>
            Base.Damage(damage, damageHandler, exactHit);
    }
}
