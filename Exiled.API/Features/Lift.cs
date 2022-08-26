// -----------------------------------------------------------------------
// <copyright file="Lift.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Structs;

    using UnityEngine;

    using BaseLift = global::Lift;

    /// <summary>
    /// The in-game lift.
    /// </summary>
    public class Lift
    {
        /// <summary>
        /// A <see cref="List{T}"/> of <see cref="Lift"/>s on the map.
        /// </summary>
        internal static readonly List<Lift> LiftsValue = new(10);

        private readonly List<Elevator> elevators = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Lift"/> class.
        /// </summary>
        /// <param name="baseLift">The <see cref="BaseLift"/> to wrap.</param>
        internal Lift(BaseLift baseLift)
        {
            Base = baseLift;

            foreach (BaseLift.Elevator elevator in baseLift.elevators)
                elevators.Add(new Elevator(elevator));
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Lift"/> which contains all the <see cref="Lift"/> instances.
        /// </summary>
        public static IEnumerable<Lift> List
        {
            get => LiftsValue;
        }

        /// <summary>
        /// Gets a random <see cref="Lift"/>.
        /// </summary>
        /// <returns><see cref="Lift"/> object.</returns>
        public static Lift Random
        {
            get => LiftsValue[UnityEngine.Random.Range(0, LiftsValue.Count)];
        }

        /// <summary>
        /// Gets the base <see cref="BaseLift"/>.
        /// </summary>
        public BaseLift Base { get; }

        /// <summary>
        /// Gets the lift's name.
        /// </summary>
        public string Name
        {
            get => Base.elevatorName;
        }

        /// <summary>
        /// Gets the <see cref="UnityEngine.GameObject"/> of the lift.
        /// </summary>
        public GameObject GameObject
        {
            get => Base.gameObject;
        }

        /// <summary>
        /// Gets the lift's <see cref="UnityEngine.Transform"/>.
        /// </summary>
        public Transform Transform
        {
            get => GameObject.transform;
        }

        /// <summary>
        /// Gets the lift's position.
        /// </summary>
        public Vector3 Position
        {
            get => Base.transform.position;
        }

        /// <summary>
        /// Gets the lift's rotation.
        /// </summary>
        public Quaternion Rotation
        {
            get => Base.transform.rotation;
        }

        /// <summary>
        /// Gets or sets the lift's <see cref="BaseLift.Status"/>.
        /// </summary>
        public BaseLift.Status Status
        {
            get => (BaseLift.Status)Base.NetworkstatusID;
            set => Base.SetStatus((byte)value);
        }

        /// <summary>
        /// Gets the lift's <see cref="ElevatorType"/>.
        /// </summary>
#pragma warning disable SA1122
        public ElevatorType Type => Name switch
        {
            "SCP-049" => ElevatorType.Scp049,
            "GateA" => ElevatorType.GateA,
            "GateB" => ElevatorType.GateB,
            "ElA" or "ElA2" => ElevatorType.LczA,
            "ElB" or "ElB2" => ElevatorType.LczB,
            "" => ElevatorType.Nuke,
            _ => ElevatorType.Unknown,
        };
#pragma warning restore SA1122

        /// <summary>
        /// Gets a value indicating whether the lift is operative.
        /// </summary>
        public bool IsOperative
        {
            get => Base.operative;
        }

        /// <summary>
        /// Gets a value indicating whether the lift is currently moving.
        /// </summary>
        public bool IsMoving
        {
            get => Base.status == BaseLift.Status.Moving;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the lift is locked.
        /// </summary>
        public bool IsLocked
        {
            get => Base.Network_locked;
            set => Base.Network_locked = value;
        }

        /// <summary>
        /// Gets a value indicating whether the lift is lockable.
        /// </summary>
        public bool IsLockable
        {
            get => Base.lockable;
        }

        /// <summary>
        /// Gets or sets the lift's moving speed.
        /// </summary>
        public float MovingSpeed
        {
            get => Base.movingSpeed;
            set => Base.movingSpeed = value;
        }

        /// <summary>
        /// Gets or sets the maximum distance from which an object should be considered inside the lift's range.
        /// </summary>
        public float MaxDistance
        {
            get => Base.maxDistance;
            set => Base.maxDistance = value;
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Vector3"/> which contains all the lift's cached items' position.
        /// </summary>
        public IEnumerable<Vector3> CachedItemPositions
        {
            get => Base._cachedItemTransforms.Select(item => item.position);
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Vector3"/> which contains all the lift's cached items' rotation.
        /// </summary>
        public IEnumerable<Quaternion> CachedItemRotations
        {
            get => Base._cachedItemTransforms.Select(item => item.rotation);
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Vector3"/> which contains all the lift's cached items' <see cref="Transform"/>.
        /// </summary>
        public IEnumerable<Transform> CachedItemTransformss
        {
            get => Base._cachedItemTransforms;
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Elevator"/> which contains all the lift's elevators.
        /// </summary>
        public IEnumerable<Elevator> Elevators
        {
            get => elevators;
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Lift"/> which contains all the <see cref="Lift"/> instances from the specified <see cref="BaseLift.Status"/>.
        /// </summary>
        /// <param name="status">The specified <see cref="BaseLift.Status"/>.</param>
        /// <returns>A <see cref="Lift"/> or <see langword="null"/> if not found.</returns>
        public static IEnumerable<Lift> Get(BaseLift.Status status) => Get(lift => lift.Status == status);

        /// <summary>
        /// Gets the <see cref="Lift"/> belonging to the <see cref="BaseLift"/>, if any.
        /// </summary>
        /// <param name="baseLift">The <see cref="BaseLift"/> instance.</param>
        /// <returns>A <see cref="Lift"/> or <see langword="null"/> if not found.</returns>
        public static Lift Get(BaseLift baseLift) => Get(lift => lift.Base == baseLift).FirstOrDefault();

        /// <summary>
        /// Gets the <see cref="Lift"/> corresponding to the specified <see cref="ElevatorType"/>, if any.
        /// </summary>
        /// <param name="type">The <see cref="ElevatorType"/>.</param>
        /// <returns>A <see cref="Lift"/> or <see langword="null"/> if not found.</returns>
        public static Lift Get(ElevatorType type) => Get(lift => lift.Type == type).FirstOrDefault();

        /// <summary>
        /// Gets the <see cref="Lift"/> corresponding to the specified name, if any.
        /// </summary>
        /// <param name="name">The lift's name.</param>
        /// <returns>A <see cref="Lift"/> or <see langword="null"/> if not found.</returns>
        public static Lift Get(string name) => Get(lift => lift.Name == name).FirstOrDefault();

        /// <summary>
        /// Gets the <see cref="Lift"/> belonging to the <see cref="UnityEngine.GameObject"/>, if any.
        /// </summary>
        /// <param name="gameObject">The <see cref="UnityEngine.GameObject"/>.</param>
        /// <returns>A <see cref="Lift"/> or <see langword="null"/> if not found.</returns>
        public static Lift Get(GameObject gameObject) => Get(lift => lift.GameObject == gameObject).FirstOrDefault();

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Lift"/> filtered based on a predicate.
        /// </summary>
        /// <param name="predicate">The condition to satify.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Lift"/> which contains elements that satify the condition.</returns>
        public static IEnumerable<Lift> Get(Func<Lift, bool> predicate) => List.Where(predicate);

        /// <summary>
        /// Tries to melt a <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to melt.</param>
        /// <returns><see langword="true"/> if the player was melted successfully; otherwise, <see langword="false"/>.</returns>
        /// <seealso cref="Player.EnableEffect(EffectType, float, bool)"/>
        public static bool TryMeltPlayer(Player player)
        {
            if (player.Position.y is >= 200 or <= -200)
                return false;

            player.EnableEffect(EffectType.Decontaminating);

            return true;
        }

        /// <summary>
        /// Plays the lift's music.
        /// </summary>
        public void PlayMusic() => Base.RpcPlayMusic();

        /// <summary>
        /// Tries to start the lift.
        /// </summary>
        /// <returns><see langword="true"/> if the lift was started successfully; otherwise, <see langword="false"/>.</returns>
        public bool TryStart() => Base.UseLift();

        /// <summary>
        /// Returns the Lift in a human-readable format.
        /// </summary>
        /// <returns>A string containing Lift-related data.</returns>
        public override string ToString() => $"{Type} {Status} [{MovingSpeed}] *{IsLocked}* |{IsLockable}|";
    }
}