// -----------------------------------------------------------------------
// <copyright file="Scp079.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs.Scp079;
    using Exiled.Events.Features;

    /// <summary>
    ///     SCP-079 related events.
    /// </summary>
    public static class Scp079
    {
        /// <summary>
        ///     Gets or sets invoked before SCP-079 switches cameras.
        /// </summary>
        public static Event<ChangingCameraEventArgs> ChangingCamera { get; set; } = new();

        /// <summary>
        ///     Gets or sets invoked before gaining experience with SCP-079.
        /// </summary>
        public static Event<GainingExperienceEventArgs> GainingExperience { get; set; } = new();

        /// <summary>
        ///     Gets or sets invoked before gaining levels with SCP-079.
        /// </summary>
        public static Event<GainingLevelEventArgs> GainingLevel { get; set; } = new();

        /// <summary>
        ///     Gets or sets invoked before triggering a tesla with SCP-079.
        /// </summary>
        public static Event<InteractingTeslaEventArgs> InteractingTesla { get; set; } = new();

        /// <summary>
        ///     Gets or sets invoked before triggering a door with SCP-079.
        /// </summary>
        public static Event<TriggeringDoorEventArgs> TriggeringDoor { get; set; } = new();

        /// <summary>
        ///     Gets or sets invoked before SCP-079 teleports using an elevator.
        /// </summary>
        public static Event<ElevatorTeleportingEventArgs> ElevatorTeleporting { get; set; } = new();

        /// <summary>
        ///     Gets or sets invoked before SCP-079 lockdowns a room.
        /// </summary>
        public static Event<LockingDownEventArgs> LockingDown { get; set; } = new();

        /// <summary>
        ///     Gets or sets invoked before SCP-079 changes a speaker status.
        /// </summary>
        public static Event<ChangingSpeakerStatusEventArgs> ChangingSpeakerStatus { get; set; } = new();

        /// <summary>
        ///     Gets or sets invoked after SCP-079 recontainment.
        /// </summary>
        public static Event<RecontainedEventArgs> Recontained { get; set; } = new();

        /// <summary>
        ///     Gets or sets invoked before SCP-079 sends a ping.
        /// </summary>
        public static Event<PingingEventArgs> Pinging { get; set; } = new();

        /// <summary>
        ///     Gets or sets invoked before SCP-079 turns off the lights in a room.
        /// </summary>
        public static Event<RoomBlackoutEventArgs> RoomBlackout { get; set; } = new();

        /// <summary>
        ///     Gets or sets invoked before SCP-079 turns off the lights in a zone.
        /// </summary>
        public static Event<ZoneBlackoutEventArgs> ZoneBlackout { get; set; } = new();

        /// <summary>
        ///     Called before SCP-079 switches cameras.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingCameraEventArgs" /> instance.</param>
        public static void OnChangingCamera(ChangingCameraEventArgs ev) => ChangingCamera.InvokeSafely(ev);

        /// <summary>
        ///     Called before gaining experience with SCP-079.
        /// </summary>
        /// <param name="ev">The <see cref="GainingExperienceEventArgs" /> instance.</param>
        public static void OnGainingExperience(GainingExperienceEventArgs ev) => GainingExperience.InvokeSafely(ev);

        /// <summary>
        ///     Called before gaining levels with SCP-079.
        /// </summary>
        /// <param name="ev">The <see cref="GainingLevelEventArgs" /> instance.</param>
        public static void OnGainingLevel(GainingLevelEventArgs ev) => GainingLevel.InvokeSafely(ev);

        /// <summary>
        ///     Called before triggering a tesla with SCP-079.
        /// </summary>
        /// <param name="ev">The <see cref="InteractingTeslaEventArgs" /> instance.</param>
        public static void OnInteractingTesla(InteractingTeslaEventArgs ev) => InteractingTesla.InvokeSafely(ev);

        /// <summary>
        ///     Called before interacting with a door with SCP-079.
        /// </summary>
        /// <param name="ev">The <see cref="TriggeringDoorEventArgs" /> instance.</param>
        public static void OnTriggeringDoor(TriggeringDoorEventArgs ev) => TriggeringDoor.InvokeSafely(ev);

        /// <summary>
        ///     Called before SCP-079 teleports using an elevator.
        /// </summary>
        /// <param name="ev">The <see cref="ElevatorTeleportingEventArgs" /> instance.</param>
        public static void OnElevatorTeleporting(ElevatorTeleportingEventArgs ev) => ElevatorTeleporting.InvokeSafely(ev);

        /// <summary>
        ///     Called before SCP-079 lockdowns a room.
        /// </summary>
        /// <param name="ev">The <see cref="LockingDownEventArgs" /> instance.</param>
        public static void OnLockingDown(LockingDownEventArgs ev) => LockingDown.InvokeSafely(ev);

        /// <summary>
        ///     Called while interacting with a speaker with SCP-079.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingSpeakerStatusEventArgs" /> instance.</param>
        public static void OnChangingSpeakerStatus(ChangingSpeakerStatusEventArgs ev) => ChangingSpeakerStatus.InvokeSafely(ev);

        /// <summary>
        ///     Called after SCP-079 is recontained.
        /// </summary>
        /// <param name="ev">The <see cref="RecontainedEventArgs" /> instance.</param>
        public static void OnRecontained(RecontainedEventArgs ev) => Recontained.InvokeSafely(ev);

        /// <summary>
        ///     Called before SCP-079 sends a ping.
        /// </summary>
        /// <param name="ev">The <see cref="PingingEventArgs" /> instance.</param>
        public static void OnPinging(PingingEventArgs ev) => Pinging.InvokeSafely(ev);

        /// <summary>
        ///     Called before SCP-079 turns off the lights in a room.
        /// </summary>
        /// <param name="ev">The <see cref="PingingEventArgs" /> instance.</param>
        public static void OnRoomBlackout(RoomBlackoutEventArgs ev) => RoomBlackout.InvokeSafely(ev);

        /// <summary>
        ///     Called before SCP-079 turns off the lights in a zone.
        /// </summary>
        /// <param name="ev">The <see cref="PingingEventArgs" /> instance.</param>
        public static void OnZoneBlackout(ZoneBlackoutEventArgs ev) => ZoneBlackout.InvokeSafely(ev);
    }
}