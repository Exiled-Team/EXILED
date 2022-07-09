// -----------------------------------------------------------------------
// <copyright file="Scp079.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs;
    using Exiled.Events.Features;

    /// <summary>
    /// SCP-079 related events.
    /// </summary>
    public static class Scp079
    {
        /// <summary>
        /// Invoked before SCP-079 switches cameras.
        /// </summary>
        public static readonly Event<ChangingCameraEventArgs> ChangingCamera = new();

        /// <summary>
        /// Invoked before gaining experience with SCP-079
        /// </summary>
        public static readonly Event<GainingExperienceEventArgs> GainingExperience = new();

        /// <summary>
        /// Invoked before gaining levels with SCP-079
        /// </summary>
        public static readonly Event<GainingLevelEventArgs> GainingLevel = new();

        /// <summary>
        /// Invoked before triggering a tesla with SCP-079.
        /// </summary>
        public static readonly Event<InteractingTeslaEventArgs> InteractingTesla = new();

        /// <summary>
        /// Invoked before triggering a door with SCP-079.
        /// </summary>
        public static readonly Event<TriggeringDoorEventArgs> TriggeringDoor = new();

        /// <summary>
        /// Invoked before SCP-079 teleports using an elevator.
        /// </summary>
        public static readonly Event<ElevatorTeleportingEventArgs> ElevatorTeleporting = new();

        /// <summary>
        /// Invoked before SCP-079 lockdowns a room.
        /// </summary>
        public static readonly Event<LockingDownEventArgs> LockingDown = new();

        /// <summary>
        /// Invoked before SCP-079 uses a speaker.
        /// </summary>
        public static readonly Event<StartingSpeakerEventArgs> StartingSpeaker = new();

        /// <summary>
        /// Invoked before SCP-079 finishes using a speaker.
        /// </summary>
        public static readonly Event<StoppingSpeakerEventArgs> StoppingSpeaker = new();

        /// <summary>
        /// Invoked after Scp079 recontainment.
        /// </summary>
        public static readonly Event<RecontainedEventArgs> Recontained = new();

        /// <summary>
        /// Called before SCP-079 switches cameras.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingCameraEventArgs"/> instance.</param>
        public static void OnChangingCamera(ChangingCameraEventArgs ev) => ChangingCamera.InvokeSafely(ev);

        /// <summary>
        /// Called before gaining experience with SCP-079.
        /// </summary>
        /// <param name="ev">The <see cref="GainingExperienceEventArgs"/> instance.</param>
        public static void OnGainingExperience(GainingExperienceEventArgs ev) => GainingExperience.InvokeSafely(ev);

        /// <summary>
        /// Called before gaining levels with SCP-079.
        /// </summary>
        /// <param name="ev">The <see cref="GainingLevelEventArgs"/> instance.</param>
        public static void OnGainingLevel(GainingLevelEventArgs ev) => GainingLevel.InvokeSafely(ev);

        /// <summary>
        /// Called before triggering a tesla with SCP-079.
        /// </summary>
        /// <param name="ev">The <see cref="InteractingTeslaEventArgs"/> instance.</param>
        public static void OnInteractingTesla(InteractingTeslaEventArgs ev) => InteractingTesla.InvokeSafely(ev);

        /// <summary>
        /// Called before interacting with a door with SCP-079.
        /// </summary>
        /// <param name="ev">The <see cref="TriggeringDoorEventArgs"/> instance.</param>
        public static void OnTriggeringDoor(TriggeringDoorEventArgs ev) => TriggeringDoor.InvokeSafely(ev);

        /// <summary>
        /// Called before SCP-079 teleports using an elevator.
        /// </summary>
        /// <param name="ev">The <see cref="ElevatorTeleportingEventArgs"/> instance.</param>
        public static void OnElevatorTeleporting(ElevatorTeleportingEventArgs ev) => ElevatorTeleporting.InvokeSafely(ev);

        /// <summary>
        /// Called before SCP-079 lockdowns a room.
        /// </summary>
        /// <param name="ev">The <see cref="LockingDownEventArgs"/> instance.</param>
        public static void OnLockingDown(LockingDownEventArgs ev) => LockingDown.InvokeSafely(ev);

        /// <summary>
        /// Called before interacting with a speaker with SCP-079.
        /// </summary>
        /// <param name="ev">The <see cref="StartingSpeakerEventArgs"/> instance.</param>
        public static void OnStartingSpeaker(StartingSpeakerEventArgs ev) => StartingSpeaker.InvokeSafely(ev);

        /// <summary>
        /// Called before SCP-079 finishes using a speaker.
        /// </summary>
        /// <param name="ev">The <see cref="StoppingEventArgs"/> instance.</param>
        public static void OnStoppingSpeaker(StoppingSpeakerEventArgs ev) => StoppingSpeaker.InvokeSafely(ev);

        /// <summary>
        /// Called after SCP-079 is recontained.
        /// </summary>
        /// <param name="ev">The <see cref="RecontainedEventArgs"/> instance.</param>
        public static void OnRecontained(RecontainedEventArgs ev) => Recontained.InvokeSafely(ev);
    }
}
