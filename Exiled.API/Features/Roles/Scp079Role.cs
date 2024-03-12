// -----------------------------------------------------------------------
// <copyright file="Scp079Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Features.Doors;
    using Interactables.Interobjects.DoorUtils;
    using MapGeneration;
    using Mirror;
    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp079;
    using PlayerRoles.PlayableScps.Scp079.Cameras;
    using PlayerRoles.PlayableScps.Scp079.Pinging;
    using PlayerRoles.PlayableScps.Scp079.Rewards;
    using PlayerRoles.Subroutines;
    using RelativePositioning;
    using Utils.NonAllocLINQ;

    using Mathf = UnityEngine.Mathf;
    using Scp079GameRole = PlayerRoles.PlayableScps.Scp079.Scp079Role;
    using Vector3 = UnityEngine.Vector3;

    /// <summary>
    /// Defines a role that represents SCP-079.
    /// </summary>
    public class Scp079Role : Role, ISubroutinedScpRole
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp079Role"/> class.
        /// </summary>
        /// <param name="baseRole">the base <see cref="Scp079GameRole"/>.</param>
        internal Scp079Role(Scp079GameRole baseRole)
            : base(baseRole)
        {
            SubroutineModule = baseRole.SubroutineModule;
            Base = baseRole;

            if (!SubroutineModule.TryGetSubroutine(out Scp079SpeakerAbility scp079SpeakerAbility))
                Log.Error("Scp079SpeakerAbility subroutine not found in Scp079Role::ctor");

            SpeakerAbility = scp079SpeakerAbility;

            if (!SubroutineModule.TryGetSubroutine(out Scp079ElevatorStateChanger scp079ElevatorStateChanger))
                Log.Error("Scp079ElevatorStateChanger subroutine not found in Scp079Role::ctor");

            ElevatorStateChanger = scp079ElevatorStateChanger;

            if (!SubroutineModule.TryGetSubroutine(out Scp079DoorStateChanger scp079DoorAbility))
                Log.Error("Scp079DoorStateChanger subroutine not found in Scp079Role::ctor");

            DoorStateChanger = scp079DoorAbility;

            if (!SubroutineModule.TryGetSubroutine(out Scp079DoorLockChanger scp079DoorLockChanger))
                Log.Error("Scp079DoorLockChanger subroutine not found in Scp079Role::ctor");
            DoorLockChanger = scp079DoorLockChanger;

            if (!SubroutineModule.TryGetSubroutine(out Scp079AuxManager scp079AuxManager))
                Log.Error("Scp079AuxManager not found in Scp079Role::ctor");

            AuxManager = scp079AuxManager;

            if (!SubroutineModule.TryGetSubroutine(out Scp079TierManager scp079TierManager))
                Log.Error("Scp079TierManager subroutine not found in Scp079Role::ctor");

            TierManager = scp079TierManager;

            if (!SubroutineModule.TryGetSubroutine(out Scp079RewardManager scp079RewardManager))
                Log.Error("Scp079RewardManager subroutine not found in Scp079Role::ctor");

            RewardManager = scp079RewardManager;

            if (!SubroutineModule.TryGetSubroutine(out Scp079LockdownRoomAbility scp079LockdownRoomAbility))
                Log.Error("Scp079LockdownRoomAbility subroutine not found in Scp079Role::ctor");

            LockdownRoomAbility = scp079LockdownRoomAbility;

            if (!SubroutineModule.TryGetSubroutine(out Scp079BlackoutRoomAbility scp079BlackoutRoomAbility))
                Log.Error("Scp079BlackoutRoomAbility subroutine not found in Scp079Role::ctor");

            BlackoutRoomAbility = scp079BlackoutRoomAbility;

            if (!SubroutineModule.TryGetSubroutine(out Scp079BlackoutZoneAbility scp079BlackoutZoneAbility))
                Log.Error("Scp079BlackoutZoneAbility subroutine not found in Scp079Role::ctor");

            BlackoutZoneAbility = scp079BlackoutZoneAbility;

            if (!SubroutineModule.TryGetSubroutine(out Scp079LostSignalHandler scp079LostSignalHandler))
                Log.Error("Scp079LostSignalHandler subroutine not found in Scp079Role::ctor");

            LostSignalHandler = scp079LostSignalHandler;

            if (!SubroutineModule.TryGetSubroutine(out Scp079CurrentCameraSync scp079CameraSync))
                Log.Error("Scp079CurrentCameraSync subroutine not found in Scp079Role::ctor");

            CurrentCameraSync = scp079CameraSync;

            if (!SubroutineModule.TryGetSubroutine(out Scp079PingAbility scp079PingAbility))
                Log.Error("Scp079PingAbility subroutine not found in Scp079Role::ctor");

            PingAbility = scp079PingAbility;

            if (!SubroutineModule.TryGetSubroutine(out Scp079TeslaAbility scp079TeslaAbility))
                Log.Error("Scp079TeslaAbility subroutine not found in Scp079Role::ctor");

            TeslaAbility = scp079TeslaAbility;

            if (!SubroutineModule.TryGetSubroutine(out Scp079ScannerTracker scp079ScannerTracker))
                Log.Error("Scp079ScannerTracker subroutine not found in Scp079Role::ctor");

            ScannerTracker = scp079ScannerTracker;

            if (!SubroutineModule.TryGetSubroutine(out Scp079ScannerZoneSelector scp079ScannerZoneSelector))
                Log.Error("Scp079ScannerZoneSelector subroutine not found in Scp079Role::ctor");

            ScannerZoneSelector = scp079ScannerZoneSelector;
        }

        /// <summary>
        /// Gets a list of players who will be turned away from SCP-079's scan.
        /// </summary>
        public static HashSet<Player> TurnedPlayers { get; } = new(20);

        /// <inheritdoc/>
        public override RoleTypeId Type { get; } = RoleTypeId.Scp079;

        /// <inheritdoc/>
        public SubroutineManagerModule SubroutineModule { get; }

        /// <summary>
        /// Gets SCP-079's <see cref="Scp079SpeakerAbility"/>.
        /// </summary>
        public Scp079SpeakerAbility SpeakerAbility { get; }

        /// <summary>
        /// Gets SCP-079's <see cref="Scp079ElevatorStateChanger"/>.
        /// </summary>
        public Scp079ElevatorStateChanger ElevatorStateChanger { get; }

        /// <summary>
        /// Gets SCP-079's <see cref="Scp079DoorAbility"/>.
        /// </summary>
        public Scp079DoorStateChanger DoorStateChanger { get; }

        /// <summary>
        /// Gets SCP-079's <see cref="Scp079DoorLockChanger"/>.
        /// </summary>
        public Scp079DoorLockChanger DoorLockChanger { get; }

        /// <summary>
        /// Gets SCP-079's <see cref="Scp079AuxManager"/>.
        /// </summary>
        public Scp079AuxManager AuxManager { get; }

        /// <summary>
        /// Gets SCP-079's <see cref="Scp079TierManager"/>.
        /// </summary>
        public Scp079TierManager TierManager { get; }

        /// <summary>
        /// Gets SCP-079's <see cref="Scp079RewardManager"/>.
        /// </summary>
        public Scp079RewardManager RewardManager { get; }

        /// <summary>
        /// Gets SCP-079's <see cref="Scp079PingAbility"/>.
        /// </summary>
        public Scp079PingAbility PingAbility { get; }

        /// <summary>
        /// Gets SCP-079's <see cref="Scp079TeslaAbility"/>.
        /// </summary>
        public Scp079TeslaAbility TeslaAbility { get; }

        /// <summary>
        /// Gets SCP-079's <see cref="Scp079LockdownRoomAbility"/>.
        /// </summary>
        public Scp079LockdownRoomAbility LockdownRoomAbility { get; }

        /// <summary>
        /// Gets SCP-079's <see cref="Scp079BlackoutRoomAbility"/>.
        /// </summary>
        public Scp079BlackoutRoomAbility BlackoutRoomAbility { get; }

        /// <summary>
        /// Gets SCP-079's <see cref="Scp079BlackoutZoneAbility"/>.
        /// </summary>
        public Scp079BlackoutZoneAbility BlackoutZoneAbility { get; }

        /// <summary>
        /// Gets SCP-079's <see cref="Scp079LostSignalHandler"/>.
        /// </summary>
        public Scp079LostSignalHandler LostSignalHandler { get; }

        /// <summary>
        /// Gets SCP-079's <see cref="Scp079CurrentCameraSync"/>.
        /// </summary>
        public Scp079CurrentCameraSync CurrentCameraSync { get; }

        /// <summary>
        /// Gets SCP-079's <see cref="Scp079ScannerTracker"/>.
        /// </summary>
        public Scp079ScannerTracker ScannerTracker { get; }

        /// <summary>
        /// Gets SCP-079's <see cref="Scp079ScannerZoneSelector"/>.
        /// </summary>
        public Scp079ScannerZoneSelector ScannerZoneSelector { get; }

        /// <summary>
        /// Gets or sets the camera SCP-079 is currently controlling.
        /// <remarks>This value will return the <c>Hcz079ContChamber</c> Camera if SCP-079's current camera cannot be detected.</remarks>
        /// </summary>
        public Camera Camera
        {
            get => Camera.Get(Base.CurrentCamera) ?? Camera.Get(CameraType.Hcz079ContChamber);
            set => Base._curCamSync.CurrentCamera = value.Base;
        }

        /// <summary>
        /// Gets or sets 079s cooldown duration for Tesla.
        /// </summary>
        public float TeslaCooldown
        {
            get => TeslaAbility._cooldown;
            set => TeslaAbility._cooldown = value;
        }

        /// <summary>
        /// Gets or sets the amount of aux power to be taken when Tesla is powered by 079.
        /// Note: Will not apply change on client.
        /// </summary>
        public int TeslaUseCost
        {
            get => TeslaAbility._cost;
            set => TeslaAbility._cost = value;
        }

        /// <summary>
        /// Gets or sets the multiplier of aux power to regen while speaking.
        /// </summary>
        public float SpeakingRegenMultiplier
        {
            get => SpeakerAbility._regenMultiplier;
            set => SpeakerAbility._regenMultiplier = value;
        }

        /// <summary>
        /// Gets or sets the duration of ghostlight signal loss.
        /// </summary>
        public float GhostlightLockoutDuration
        {
            get => LostSignalHandler._ghostlightLockoutDuration;
            set => LostSignalHandler._ghostlightLockoutDuration = value;
        }

        /// <summary>
        /// Gets or sets the cost for blackout ability.
        /// </summary>
        public int BlackoutCost
        {
            get => BlackoutZoneAbility._cost;
            set => BlackoutZoneAbility._cost = value;
        }

        /// <summary>
        /// Gets or sets the duration for blackout ability.
        /// </summary>
        public float BlackoutDuration
        {
            get => BlackoutZoneAbility._duration;
            set => BlackoutZoneAbility._duration = value;
        }

        /// <summary>
        /// Gets or sets the cooldown for blackout ability.
        /// </summary>
        public float BlackoutCooldown
        {
            get => BlackoutZoneAbility._cooldown;
            set => BlackoutZoneAbility._cooldown = value;
        }

        /// <summary>
        /// Gets or sets the cost for changing elevator state.
        /// </summary>
        public int ElevatorCost
        {
            get => ElevatorStateChanger._cost;
            set => ElevatorStateChanger._cost = value;
        }

        /// <summary>
        /// Gets a value indicating whether or not SCP-079 can transmit its voice to a speaker.
        /// </summary>
        public bool CanTransmit => SpeakerAbility.CanTransmit;

        /// <summary>
        /// Gets a list of rooms that have been marked by SCP-079. Marked rooms grant SCP-079 experience if a kill occurs in them.
        /// </summary>
        public IEnumerable<Room> MarkedRooms => RewardManager._markedRooms.Select(kvp => Room.Get(kvp.Key));

        /// <summary>
        /// Gets the speaker SCP-079 is currently using. Can be <see langword="null"/>.
        /// </summary>
        public Scp079Speaker Speaker => Scp079Speaker.TryGetSpeaker(Base.CurrentCamera, out Scp079Speaker speaker) ? speaker : null;

        /// <summary>
        /// Gets the doors SCP-079 has locked. Can be <see langword="null"/>.
        /// </summary>
        public Door LockedDoor => Door.Get(DoorLockChanger.LockedDoor);

        /// <summary>
        /// Gets or sets SCP-079's abilities. Can be <see langword="null"/>.
        /// </summary>
        public IScp079AuxRegenModifier[] Abilities
        {
            get => AuxManager._abilities;
            set => AuxManager._abilities = value;
        }

        /// <summary>
        /// Gets or sets the amount of experience SCP-079 has.
        /// </summary>
        public int Experience
        {
            get => TierManager.TotalExp;
            set => TierManager.TotalExp = value;
        }

        /// <summary>
        /// Gets the Current Camera Position.
        /// </summary>
        public Vector3 CameraPosition => Base.CameraPosition;

        /// <summary>
        /// Gets the relative experience.
        /// </summary>
        public float RelativeExperience => TierManager.RelativeExp;

        /// <summary>
        /// Gets or sets SCP-079's level.
        /// </summary>
        public int Level
        {
            get => TierManager.AccessTierLevel;
            set => Experience = value <= 1 ? 0 : TierManager.AbsoluteThresholds[Mathf.Clamp(value - 2, 0, TierManager.AbsoluteThresholds.Length - 1)];
        }

        /// <summary>
        /// Gets or sets SCP-079's level index.
        /// </summary>
        public int LevelIndex
        {
            get => TierManager.AccessTierIndex;
            set => Level = value + 1;
        }

        /// <summary>
        /// Gets SCP-079's next level threshold.
        /// </summary>
        public int NextLevelThreshold => TierManager.NextLevelThreshold;

        /// <summary>
        /// Gets or sets SCP-079's energy.
        /// </summary>
        public float Energy
        {
            get => AuxManager.CurrentAux;
            set => AuxManager.CurrentAux = value;
        }

        /// <summary>
        /// Gets or sets SCP-079's max energy.
        /// </summary>
        public float MaxEnergy
        {
            get => AuxManager.MaxAux;
            set => AuxManager._maxPerTier[LevelIndex] = value;
        }

        /// <summary>
        /// Gets or sets SCP-079's room lockdown cooldown.
        /// </summary>
        public float RoomLockdownCooldown
        {
            get => LockdownRoomAbility.RemainingCooldown;
            set
            {
                LockdownRoomAbility.RemainingCooldown = value;
                LockdownRoomAbility.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Gets the Remaining Lockdown Duration.
        /// </summary>
        public float RemainingLockdownDuration => LockdownRoomAbility.RemainingLockdownDuration;

        /// <summary>
        /// Gets the amount of rooms that SCP-079 has blacked out.
        /// </summary>
        public int BlackoutCount => BlackoutRoomAbility.RoomsOnCooldown;

        /// <summary>
        /// Gets the maximum amount of rooms that SCP-079 can black out at its current <see cref="Level"/>.
        /// </summary>
        public int BlackoutCapacity => BlackoutRoomAbility.CurrentCapacity;

        /// <summary>
        /// Gets or sets the amount of time until SCP-079 can use its blackout zone ability again.
        /// </summary>
        public float BlackoutZoneCooldown
        {
            get => BlackoutZoneAbility._cooldownTimer.Remaining;
            set
            {
                BlackoutZoneAbility._cooldownTimer.Remaining = value;
                BlackoutZoneAbility.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Gets or sets the amount of time that SCP-2176 will disable SCP-079 for.
        /// </summary>
        public float Scp2176LostTime
        {
            get => LostSignalHandler._ghostlightLockoutDuration;
            set => LostSignalHandler._ghostlightLockoutDuration = value;
        }

        /// <summary>
        /// Gets the Roll Rotation of SCP-079.
        /// </summary>
        public float RollRotation => Base.RollRotation;

        /// <summary>
        /// Gets a value indicating whether or not SCP-079's signal is lost due to SCP-2176.
        /// </summary>
        public bool IsLost => LostSignalHandler.Lost;

        /// <summary>
        /// Gets a value indicating how much more time SCP-079 will be lost.
        /// </summary>
        public float LostTime => LostSignalHandler.RemainingTime;

        /// <summary>
        /// Gets SCP-079's energy regeneration speed.
        /// </summary>
        public float EnergyRegenerationSpeed => AuxManager.RegenSpeed;

        /// <summary>
        /// Gets the game <see cref="Scp079GameRole"/>.
        /// </summary>
        public new Scp079GameRole Base { get; }

        /// <summary>
        /// Unlocks all doors that SCP-079 has locked.
        /// </summary>
        public void UnlockAllDoors() => DoorLockChanger.ServerUnlock();

        /// <summary>
        /// Forces SCP-079's signal to be lost for the specified amount of time.
        /// </summary>
        /// <param name="duration">Time to lose SCP-079's signal.</param>
        public void LoseSignal(float duration) => LostSignalHandler.ServerLoseSignal(duration);

        /// <summary>
        /// Grants SCP-079 experience.
        /// </summary>
        /// <param name="amount">The amount to grant.</param>
        /// <param name="reason">The reason to grant experience.</param>
        public void AddExperience(int amount, Scp079HudTranslation reason = Scp079HudTranslation.ExpGainAdminCommand) => TierManager.ServerGrantExperience(amount, reason);

        /// <summary>
        /// Grants SCP-079 experience.
        /// </summary>
        /// <param name="amount">The amount to grant.</param>
        /// <param name="reason">The reason to grant experience.</param>
        /// <param name="subject">The RoleType of the player that's causing it to happen.</param>
        public void AddExperience(int amount, Scp079HudTranslation reason, RoleTypeId subject) => TierManager.ServerGrantExperience(amount, reason, subject);

        /// <summary>
        /// Locks the provided <paramref name="door"/>.
        /// </summary>
        /// <param name="door">The door to lock.</param>
        /// <returns><see langword="true"/> if the door has been lock; otherwise, <see langword="false"/>.</returns>
        public bool LockDoor(Door door)
        {
            if (door is not null)
            {
                DoorLockChanger.LockedDoor = door.Base;
                DoorLockChanger._lockTime = NetworkTime.time;
                DoorLockChanger.LockedDoor.ServerChangeLock(DoorLockReason.Regular079, true);
                if (door.Room is not null)
                    MarkRoom(door.Room);
                AuxManager.CurrentAux -= DoorLockChanger.GetCostForDoor(DoorAction.Locked, DoorLockChanger.LockedDoor);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Locks the provided <paramref name="door"/>.
        /// </summary>
        /// <param name="door">The door to lock.</param>
        /// <returns><see langword="true"/> if the door has been lock; otherwise, <see langword="false"/>.</returns>
        /// <param name="consumeEnergy">Indicates if the energy cost should be consumed or not.</param>
        public bool LockDoor(Door door, bool consumeEnergy = true)
        {
            if (door is not null)
            {
                DoorLockChanger.LockedDoor = door.Base;
                DoorLockChanger._lockTime = NetworkTime.time;
                DoorLockChanger.LockedDoor.ServerChangeLock(DoorLockReason.Regular079, true);
                MarkRoom(door.Room);
                if (consumeEnergy)
                    AuxManager.CurrentAux -= GetCost(door, DoorAction.Locked);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Unlocks the <see cref="LockedDoor"/>.
        /// </summary>
        public void UnlockDoor() => LockedDoor?.Unlock();

        /// <summary>
        /// Unlocks the provided <paramref name="door"/>.
        /// </summary>
        /// <param name="door">The door to unlock.</param>
        public void UnlockDoor(Door door)
        {
            if (door is not null && Door.Get(DoorLockChanger.LockedDoor) == door)
            {
                door.Unlock();
            }
        }

        /// <summary>
        /// Marks a room as being modified by SCP-079 (granting experience if a kill happens in the room).
        /// </summary>
        /// <param name="room">The room to mark.</param>
        public void MarkRoom(Room room)
        {
            if (room is not null)
                RewardManager.MarkRoom(room.Identifier);
        }

        /// <summary>
        /// Marks a array of rooms as being modified by SCP-079 (granting experience if a kill happens in the room).
        /// </summary>
        /// <param name="rooms">The Array of Rooms to mark.</param>
        public void MarkRooms(IEnumerable<Room> rooms) => RewardManager.MarkRooms(rooms.Select(x => x.Identifier).ToArray());

        /// <summary>
        /// Removes a marked room.
        /// </summary>
        /// <param name="room">The room to remove.</param>
        public void UnmarkRoom(Room room)
        {
            if (room is not null && RewardManager._markedRooms.ContainsKey(room.Identifier))
                RewardManager._markedRooms.Remove(room.Identifier);
        }

        /// <summary>
        /// Clears the list of marked SCP-079 rooms.
        /// </summary>
        public void ClearMarkedRooms() => RewardManager._markedRooms.Clear();

        /// <summary>
        /// Gets the cost to switch from the current <see cref="Camera"/> to the provided <paramref name="camera"/>.
        /// </summary>
        /// <param name="camera">The camera to get the cost to switch to.</param>
        /// <returns>The cost to switch from the current camera to the new camera.</returns>
        public int GetSwitchCost(Camera camera) => camera is null ? 0 : CurrentCameraSync.GetSwitchCost(camera.Base);

        /// <summary>
        /// Gets the cost to modify a door.
        /// </summary>
        /// <param name="door">The door to get the cost to modify.</param>
        /// <param name="action">The action.</param>
        /// <returns>The cost to modify the door.</returns>
        public int GetCost(Door door, DoorAction action) => action is DoorAction.Locked or DoorAction.Unlocked ? DoorLockChanger.GetCostForDoor(action, door.Base) :
            DoorStateChanger.GetCostForDoor(action, door.Base);

        /// <summary>
        /// Blackout the current room.
        /// </summary>
        /// <param name="consumeEnergy">Indicates if the energy cost should be consumed or not.</param>
        public void BlackoutRoom(bool consumeEnergy = true)
        {
            if (consumeEnergy)
                BlackoutRoomAbility.AuxManager.CurrentAux -= BlackoutRoomAbility._cost;

            BlackoutRoomAbility.RewardManager.MarkRoom(BlackoutRoomAbility._roomController.Room);
            BlackoutRoomAbility._blackoutCooldowns[BlackoutRoomAbility._roomController.netId] = NetworkTime.time + BlackoutRoomAbility._cooldown;
            BlackoutRoomAbility._roomController.ServerFlickerLights(BlackoutRoomAbility._blackoutDuration);
            BlackoutRoomAbility._successfulController = BlackoutRoomAbility._roomController;
            BlackoutRoomAbility.ServerSendRpc(true);
        }

        /// <summary>
        /// Blackout the current zone.
        /// </summary>
        /// <param name="consumeEnergy">Indicates if the energy cost should be consumed or not.</param>
        public void BlackoutZone(bool consumeEnergy = true)
        {
            foreach (RoomLightController lightController in RoomLightController.Instances)
            {
                if (lightController.Room.Zone == BlackoutZoneAbility._syncZone)
                {
                    lightController.ServerFlickerLights(BlackoutZoneAbility._duration);
                }
            }

            BlackoutZoneAbility._cooldownTimer.Trigger(BlackoutZoneAbility._cooldown);

            if (consumeEnergy)
                BlackoutZoneAbility.AuxManager.CurrentAux -= BlackoutZoneAbility._cost;

            BlackoutZoneAbility.ServerSendRpc(true);
        }

        /// <summary>
        /// Trigger the Ping Ability to ping a <see cref="RelativePosition"/>.
        /// </summary>
        /// <param name="position">The SyncNormal Position.</param>
        /// <param name="pingType">The PingType to return.</param>
        /// <param name="consumeEnergy">Indicates if the energy cost should be consumed or not.</param>
        public void Ping(Vector3 position, PingType pingType = PingType.Default, bool consumeEnergy = true)
        {
            PingAbility._syncPos = new(position);
            PingAbility._syncNormal = position;
            PingAbility._syncProcessorIndex = (byte)pingType;

            PingAbility.ServerSendRpc(x => PingAbility.ServerCheckReceiver(x, PingAbility._syncPos.Position, (int)pingType));

            if (consumeEnergy)
                PingAbility.AuxManager.CurrentAux -= PingAbility._cost;

            PingAbility._rateLimiter.RegisterInput();
        }

        /// <summary>
        /// Trigger the Lockdown Room Ability to lock the current room.
        /// </summary>
        public void LockdownRoom() => LockdownRoomAbility.ServerInitLockdown();

        /// <summary>
        /// Cancels the Actual Lockdown.
        /// </summary>
        public void CancelLockdown() => LockdownRoomAbility.ServerCancelLockdown();

        /// <summary>
        /// Trigger the SCP-079's Tesla Gate Ability.
        /// </summary>
        /// <param name="consumeEnergy">Indicates if the energy cost should be consume or not.</param>
        public void ActivateTesla(bool consumeEnergy = true)
        {
            Scp079Camera cam = CurrentCameraSync.CurrentCamera;
            RewardManager.MarkRoom(cam.Room);

            if (!TeslaGateController.Singleton.TeslaGates.TryGetFirst(x => RoomIdUtils.IsTheSameRoom(cam.Position, x.transform.position), out global::TeslaGate teslaGate))
                return;

            if (consumeEnergy)
                AuxManager.CurrentAux -= TeslaAbility._cost;

            teslaGate.RpcInstantBurst();
            TeslaAbility._nextUseTime = NetworkTime.time + TeslaAbility._cooldown;
            TeslaAbility.ServerSendRpc(false);
        }

        /// <summary>
        /// Gets the spawn chance of SCP-079.
        /// </summary>
        /// <param name="alreadySpawned">The List of Roles already spawned.</param>
        /// <returns>The Spawn Chance.</returns>
        public float GetSpawnChance(List<RoleTypeId> alreadySpawned) => Base.GetSpawnChance(alreadySpawned);
    }
}
