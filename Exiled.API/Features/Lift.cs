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
    using Exiled.API.Extensions;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.API.Features.Doors;
    using Exiled.API.Interfaces;
    using Interactables.Interobjects;
    using Interactables.Interobjects.DoorUtils;
    using MEC;
    using UnityEngine;

    using static Interactables.Interobjects.ElevatorChamber;
    using static Interactables.Interobjects.ElevatorManager;

    using Elevator = Interactables.Interobjects.ElevatorDoor;

    /// <summary>
    /// The in-game lift.
    /// </summary>
    public class Lift : GameEntity, IWrapper<ElevatorChamber>, IWorldSpace
    {
        /// <summary>
        /// A <see cref="Dictionary{TKey,TValue}"/> containing all known <see cref="ElevatorChamber"/>s and their corresponding <see cref="Lift"/>.
        /// </summary>
        internal static readonly Dictionary<ElevatorChamber, Lift> ElevatorChamberToLift = new(8);

        /// <summary>
        /// Internal list that contains all ElevatorDoor for current group.
        /// </summary>
        private readonly List<Elevator> internalDoorsList = ListPool<Elevator>.Pool.Get();

        /// <summary>
        /// Initializes a new instance of the <see cref="Lift"/> class.
        /// </summary>
        /// <param name="elevator">The <see cref="ElevatorChamber"/> to wrap.</param>
        internal Lift(ElevatorChamber elevator)
            : base()
        {
            Base = elevator;
            ElevatorChamberToLift.Add(elevator, this);

            internalDoorsList.AddRange(Elevator.AllElevatorDoors[Group]);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Lift"/> class.
        /// </summary>
        ~Lift() => ListPool<Elevator>.Pool.Return(internalDoorsList);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Lift"/> which contains all the <see cref="Lift"/> instances.
        /// </summary>
        public static new IReadOnlyCollection<Lift> List => ElevatorChamberToLift.Values;

        /// <summary>
        /// Gets a random <see cref="Lift"/>.
        /// </summary>
        /// <returns><see cref="Lift"/> object.</returns>
        public static Lift Random => List.Random();

        /// <summary>
        /// Gets a value of the internal doors list.
        /// </summary>
        public IReadOnlyCollection<Doors.ElevatorDoor> Doors => internalDoorsList.Select(x => Door.Get(x).As<Doors.ElevatorDoor>()).ToList();

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Player"/> in the <see cref="Room"/>.
        /// </summary>
        public IEnumerable<Player> Players => Player.List.Where(x => Bounds.Contains(x.Position));

        /// <summary>
        /// Gets the lift's name.
        /// </summary>
        public string Name => Group.ToString();

        /// <summary>
        /// Gets the <see cref="UnityEngine.GameObject"/> of the lift.
        /// </summary>
        public override GameObject GameObject => Base.gameObject;

        /// <summary>
        /// Gets or sets the lift's <see cref="ElevatorChamber"/> status.
        /// </summary>
        public ElevatorSequence Status
        {
            get => Base._curSequence;
            set => Base._curSequence = value;
        }

        /// <summary>
        /// Gets the <see cref="UnityEngine.Bounds"/> representing the space inside the lift.
        /// </summary>
        public Bounds Bounds => Base.WorldspaceBounds;

        /// <summary>
        /// Gets the lift's <see cref="ElevatorType"/>.
        /// </summary>
        public ElevatorType Type => Group switch
        {
            ElevatorGroup.Scp049 => ElevatorType.Scp049,
            ElevatorGroup.GateA => ElevatorType.GateA,
            ElevatorGroup.GateB => ElevatorType.GateB,
            ElevatorGroup.LczA01 or ElevatorGroup.LczA02 => ElevatorType.LczA,
            ElevatorGroup.LczB01 or ElevatorGroup.LczB02 => ElevatorType.LczB,
            ElevatorGroup.Nuke => ElevatorType.Nuke,
            _ => ElevatorType.Unknown,
        };

        /// <summary>
        /// Gets the <see cref="ElevatorGroup"/>.
        /// </summary>
        public ElevatorGroup Group => Base.AssignedGroup;

        /// <summary>
        /// Gets a value indicating whether the lift is operative.
        /// </summary>
        public bool IsOperative => Base.IsReady;

        /// <summary>
        /// Gets a value indicating whether the lift is currently moving.
        /// </summary>
        public bool IsMoving => Status is ElevatorSequence.MovingAway or ElevatorSequence.Arriving;

        /// <summary>
        /// Gets a value indicating whether the lift is locked.
        /// </summary>
        public bool IsLocked => Base.ActiveLocks > 0;

        /// <summary>
        /// Gets or sets the <see cref="AnimationTime"/>.
        /// </summary>
        public float AnimationTime
        {
            get => Base._animationTime;
            set => Base._animationTime = value;
        }

        /// <summary>
        /// Gets the <see cref="RotationTime"/>.
        /// </summary>
        public float RotationTime => Base._rotationTime;

        /// <summary>
        /// Gets the <see cref="DoorOpenTime"/>.
        /// </summary>
        public float DoorOpenTime => Base._doorOpenTime;

        /// <summary>
        /// Gets the <see cref="DoorCloseTime"/>.
        /// </summary>
        public float DoorCloseTime => Base._doorCloseTime;

        /// <summary>
        /// Gets the total <see cref="MoveTime"/>.
        /// </summary>
        public float MoveTime => AnimationTime + RotationTime + DoorOpenTime + DoorCloseTime;

        /// <summary>
        /// Gets the <see cref="CurrentLevel"/>.
        /// </summary>
        public int CurrentLevel => Base.CurrentLevel;

        /// <summary>
        /// Gets the <see cref="CurrentDestination"/>.
        /// </summary>
        public Doors.ElevatorDoor CurrentDestination => Door.Get(Base.CurrentDestination).As<Doors.ElevatorDoor>();

        /// <summary>
        /// Gets or sets the lift's position.
        /// </summary>
        public override Vector3 Position
        {
            get => Transform.position;
            set => Transform.position = value;
        }

        /// <summary>
        /// Gets or sets the lift's rotation.
        /// </summary>
        public override Quaternion Rotation
        {
            get => Transform.rotation;
            set => Transform.rotation = value;
        }

        /// <summary>
        /// Gets the base <see cref="ElevatorChamber"/>.
        /// </summary>
        public ElevatorChamber Base { get; }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Lift"/> which contains all the <see cref="Lift"/> instances from the specified <see cref="Status"/>.
        /// </summary>
        /// <param name="status">The specified <see cref="ElevatorChamber"/>.</param>
        /// <returns>A <see cref="Lift"/> or <see langword="null"/> if not found.</returns>
        public static IEnumerable<Lift> Get(ElevatorSequence status) => Get(lift => lift.Status == status);

        /// <summary>
        /// Gets the <see cref="Lift"/> belonging to the <see cref="ElevatorChamber"/>, if any.
        /// </summary>
        /// <param name="elevator">The <see cref="ElevatorChamber"/> instance.</param>
        /// <returns>A <see cref="Lift"/> or <see langword="null"/> if not found.</returns>
        public static Lift Get(ElevatorChamber elevator) => ElevatorChamberToLift.TryGetValue(elevator, out Lift lift) ? lift : new(elevator);

        /// <summary>
        /// Gets the <see cref="Lift"/> corresponding to the specified <see cref="ElevatorType"/>, if any.
        /// </summary>
        /// <param name="type">The <see cref="ElevatorType"/>.</param>
        /// <returns>A <see cref="Lift"/> or <see langword="null"/> if not found.</returns>
        public static Lift Get(ElevatorType type) => List.FirstOrDefault(lift => lift.Type == type);

        /// <summary>
        /// Gets all lifts corresponding to the specified types, if any.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <returns>All corresponding lifts.</returns>
        public static IEnumerable<Lift> Get(params ElevatorType[] types) => Get(lift => types.Contains(lift.Type));

        /// <summary>
        /// Gets all lifts corresponding to the specified types, if any.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <returns>All corresponding lifts.</returns>
        public static IEnumerable<Lift> Get(IEnumerable<ElevatorType> types) => Get(lift => types.Contains(lift.Type));

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
        /// Gets the <see cref="Lift"/> belonging to the <see cref="Vector3"/>, if any.
        /// </summary>
        /// <param name="position">The <see cref="Vector3"/>.</param>
        /// <returns>A <see cref="Lift"/> or <see langword="null"/> if not found.</returns>
        public static Lift Get(Vector3 position) => Get(lift => lift.Bounds.Contains(position)).FirstOrDefault();

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Lift"/> filtered based on a predicate.
        /// </summary>
        /// <param name="predicate">The condition to satisfy.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Lift"/> which contains elements that satisfy the condition.</returns>
        public static IEnumerable<Lift> Get(Func<Lift, bool> predicate) => List.Where(predicate);

        /// <summary>
        /// Permanently locks an elevator corresponding to the given type.
        /// </summary>
        /// <param name="type">The elevator to affect.</param>
        /// <param name="lockReason">The specified <see cref="DoorLockReason"/>.</param>
        public static void Lock(ElevatorType type, DoorLockReason lockReason = DoorLockReason.Isolation) => Get(type)?.Lock(lockReason);

        /// <summary>
        /// Temporary locks an elevator corresponding to the given type.
        /// </summary>
        /// <param name="type">The elevator to affect.</param>
        /// <param name="duration">The duration of the lockdown.</param>
        /// <param name="lockReason">The specified <see cref="DoorLockReason"/>.</param>
        public static void Lock(ElevatorType type, float duration, DoorLockReason lockReason = DoorLockReason.Isolation) => Get(type)?.Lock(duration, lockReason);

        /// <summary>
        /// Unlocks a lift corresponding to the specified type.
        /// </summary>
        /// <param name="type">The <see cref="ElevatorType"/>.</param>
        public static void Unlock(ElevatorType type) => Get(type)?.Unlock();

        /// <summary>
        /// Permanently locks all elevators in the facility.
        /// </summary>
        /// <param name="lockReason">The specified <see cref="DoorLockReason"/>.</param>
        public static void LockAll(DoorLockReason lockReason = DoorLockReason.Isolation) => List.ForEach(lift => lift.Lock(lockReason));

        /// <summary>
        /// Temporary locks all elevators in the facility.
        /// </summary>
        /// <param name="duration">The duration of the lockdown.</param>
        /// <param name="lockReason">The specified <see cref="DoorLockReason"/>.</param>
        public static void LockAll(float duration, DoorLockReason lockReason = DoorLockReason.Isolation) => List.ForEach(lift => lift.Lock(duration, lockReason));

        /// <summary>
        /// Permanently locks all elevators corresponding to the given types.
        /// </summary>
        /// <param name="types">The doors to affect.</param>
        /// <param name="lockReason">The specified <see cref="DoorLockReason"/>.</param>
        public static void LockAll(IEnumerable<ElevatorType> types, DoorLockReason lockReason = DoorLockReason.Isolation) => types.ForEach(t => Lock(t, lockReason));

        /// <summary>
        /// Temporary locks all elevators corresponding to the given types.
        /// </summary>
        /// <param name="types">The doors to affect.</param>
        /// <param name="duration">The duration of the lockdown.</param>
        /// <param name="lockReason">The specified <see cref="DoorLockReason"/>.</param>
        public static void LockAll(IEnumerable<ElevatorType> types, float duration, DoorLockReason lockReason = DoorLockReason.Isolation) => types.ForEach(t => Lock(t, duration, lockReason));

        /// <summary>
        /// Unlocks all lifts in the facility.
        /// </summary>
        public static void UnlockAll() => List.ForEach(lift => lift.Unlock());

        /// <summary>
        /// Unlocks all lifts in the facility.
        /// </summary>
        /// <param name="type">The zones to affect.</param>
        public static void UnlockAll(ZoneType type) => List.ForEach(lift => lift.Doors.Where(door => door.Zone == type).ForEach(door => door.Unlock()));

        /// <summary>
        /// Unlocks all lifts in the facility.
        /// </summary>
        /// <param name="types">The zones to affect.</param>
        public static void UnlockAll(params ZoneType[] types) => List.ForEach(lift => lift.Doors.Where(door => types.Contains(door.Zone)).ForEach(door => door.Unlock()));

        /// <summary>
        /// Unlocks all lifts in the facility.
        /// </summary>
        /// <param name="types">The zones to affect.</param>
        public static void UnlockAll(IEnumerable<ZoneType> types) => List.ForEach(lift => lift.Doors.Where(door => types.Contains(door.Zone)).ForEach(door => door.Unlock()));

        /// <summary>
        /// Unlocks all lifts in the facility.
        /// </summary>
        /// <param name="types">The types to affect.</param>
        public static void UnlockAll(params ElevatorType[] types) => Get(types).ForEach(lift => lift.Unlock());

        /// <summary>
        /// Unlocks all lifts in the facility.
        /// </summary>
        /// <param name="types">The types to affect.</param>
        public static void UnlockAll(IEnumerable<ElevatorType> types) => Get(types).ForEach(lift => lift.Unlock());

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
        /// Tries to start the lift.
        /// </summary>
        /// <param name="level">The destination level.</param>
        /// <param name="isForced">Indicates whether the start will be forced or not.</param>
        /// <returns><see langword="true"/> if the lift was started successfully; otherwise, <see langword="false"/>.</returns>
        public bool TryStart(int level, bool isForced = false) => TrySetDestination(Group, level, isForced);

        /// <summary>
        /// Locks the lift.
        /// </summary>
        /// <param name="lockReason">The <see cref="DoorLockReason"/>.</param>
        public void Lock(DoorLockReason lockReason = DoorLockReason.Isolation)
        {
            Status = ElevatorSequence.DoorClosing;
            ChangeLock(lockReason);
        }

        /// <summary>
        /// Locks the lift.
        /// </summary>
        /// <param name="duration">The duration of the lockdown.</param>
        /// <param name="lockReason">The <see cref="DoorLockReason"/>.</param>
        public void Lock(float duration, DoorLockReason lockReason = DoorLockReason.Isolation)
        {
            Status = ElevatorSequence.DoorClosing;
            Lock(duration, lockReason);
        }

        /// <summary>
        /// Unlocks the lift.
        /// </summary>
        public void Unlock() => ChangeLock(DoorLockReason.None);

        /// <summary>
        /// Unlocks the lift.
        /// </summary>
        /// <param name="delay">The delay after which the lift should be unlocked.</param>
        public void Unlock(float delay) => Timing.CallDelayed(delay, () => ChangeLock(DoorLockReason.None));

        /// <summary>
        /// Changes lock of the lift.
        /// </summary>
        /// <param name="lockReason">The <see cref="DoorLockReason"/>.</param>
        public void ChangeLock(DoorLockReason lockReason)
        {
            bool forceLock = lockReason != DoorLockReason.None;

            foreach (Doors.ElevatorDoor door in Doors)
            {
                if (!forceLock)
                {
                    door.DoorLockType = 0;

                    door.ChangeLock(DoorLockType.None);
                }
                else
                {
                    door.ChangeLock((DoorLockType)lockReason);

                    if (CurrentLevel != 1)
                        TrySetDestination(Group, 1, true);
                }

                Base.RefreshLocks(Group, door.Base);
            }
        }

        /// <summary>
        /// Returns whether or not the provided <see cref="Vector3">position</see> is inside the lift.
        /// </summary>
        /// <param name="point">The position.</param>
        /// <returns><see langword="true"/> if the point is inside the elevator. Otherwise, <see langword="false"/>.</returns>
        public bool IsInElevator(Vector3 point) => Bounds.Contains(point);

        /// <summary>
        /// Returns the Lift in a human-readable format.
        /// </summary>
        /// <returns>A string containing Lift-related data.</returns>
        public override string ToString() => $"{Type} {Status} [{CurrentLevel}] *{IsLocked}*";
    }
}