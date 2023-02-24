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
    using Interactables.Interobjects.DoorUtils;
    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp079;
    using PlayerRoles.PlayableScps.Scp079.Cameras;
    using PlayerRoles.PlayableScps.Scp079.Rewards;
    using PlayerRoles.PlayableScps.Subroutines;

    using Mathf = UnityEngine.Mathf;
    using Scp079GameRole = PlayerRoles.PlayableScps.Scp079.Scp079Role;

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
            Internal = baseRole;

            if (!SubroutineModule.TryGetSubroutine(out Scp079SpeakerAbility scp079SpeakerAbility))
                Log.Error("Scp079SpeakerAbility subroutine not found in Scp079Role::ctor");

            SpeakerAbility = scp079SpeakerAbility;

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
        }

        /// <inheritdoc/>
        public override RoleTypeId Type { get; } = RoleTypeId.Scp079;

        /// <inheritdoc/>
        public SubroutineManagerModule SubroutineModule { get; }

        /// <summary>
        /// Gets SCP-079's <see cref="Scp079SpeakerAbility"/>.
        /// </summary>
        public Scp079SpeakerAbility SpeakerAbility { get; }

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
        /// Gets or sets the camera SCP-079 is currently controlling.
        /// <remarks>This value will return the <c>Hcz079ContChamber</c> Camera if SCP-079's current camera cannot be detected.</remarks>
        /// </summary>
        public Camera Camera
        {
            get => Camera.Get(Internal.CurrentCamera) ?? Camera.Get(CameraType.Hcz079ContChamber);
            set => Internal._curCamSync.CurrentCamera = value.Base;
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
        public Scp079Speaker Speaker => Scp079Speaker.TryGetSpeaker(Internal.CurrentCamera, out Scp079Speaker speaker) ? speaker : null;

        /// <summary>
        /// Gets the doors SCP-079 has locked. Can be <see langword="null"/>.
        /// </summary>
        public IEnumerable<Door> LockedDoors => DoorLockChanger._lockedDoors.Select(x => Door.Get(x));

        /// <summary>
        /// Gets or sets SCP-079's abilities. Can be <see langword="null"/>.
        /// </summary>
        public Scp079AbilityBase[] Abilities
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
        protected Scp079GameRole Internal { get; }

        /// <summary>
        /// Unlocks all doors that SCP-079 has locked.
        /// </summary>
        public void UnlockAllDoors() => DoorLockChanger.ServerUnlockAll();

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
        /// Locks the provided <paramref name="door"/>.
        /// </summary>
        /// <param name="door">The door to lock.</param>
        /// <returns>.</returns>
        public bool LockDoor(Door door) => DoorLockChanger.SetDoorLock(door.Base, true);

        /// <summary>
        /// Unlocks the provided <paramref name="door"/>.
        /// </summary>
        /// <param name="door">The door to unlock.</param>
        public void UnlockDoor(Door door) => DoorLockChanger.SetDoorLock(door.Base, false);

        /// <summary>
        /// Marks a room as being modified by SCP-079 (granting experience if a kill happens in the room).
        /// </summary>
        /// <param name="room">The room to mark.</param>
        public void MarkRoom(Room room) => RewardManager.MarkRoom(room.Identifier);

        /// <summary>
        /// Removes a marked room.
        /// </summary>
        /// <param name="room">The room to remove.</param>
        public void UnmarkRoom(Room room)
        {
            if (RewardManager._markedRooms.ContainsKey(room.Identifier))
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
        public int GetSwitchCost(Camera camera) => CurrentCameraSync.GetSwitchCost(camera.Base);

        /// <summary>
        /// Gets the cost to modify a door.
        /// </summary>
        /// <param name="door">The door to get the cost to modify.</param>
        /// <param name="action">The action.</param>
        /// <returns>The cost to modify the door.</returns>
        public int GetCost(Door door, DoorAction action)
        {
            if (action is DoorAction.Locked or DoorAction.Unlocked)
                return DoorLockChanger.GetCostForDoor(action, door.Base);
            else
                return DoorStateChanger.GetCostForDoor(action, door.Base);
        }
    }
}