// -----------------------------------------------------------------------
// <copyright file="Scp079.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.API.Utils;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Extensions;

    using static Exiled.Events.Events;

    /// <summary>
    /// SCP-079 related events.
    /// </summary>
    public static class Scp079
    {
        /// <summary>
        /// Invoked before SCP-079 switches cameras.
        /// </summary>
        public static event CustomEventHandler<ChangingCameraEventArgs> ChangingCamera;

        /// <summary>
        /// Invoked before gaining experience with SCP-079
        /// </summary>
        public static event CustomEventHandler<GainingExperienceEventArgs> GainingExperience;

        /// <summary>
        /// Invoked before gaining levels with SCP-079
        /// </summary>
        public static event CustomEventHandler<GainingLevelEventArgs> GainingLevel;

        /// <summary>
        /// Invoked before triggering a tesla with SCP-079.
        /// </summary>
        public static event CustomEventHandler<InteractingTeslaEventArgs> InteractingTesla;

        /// <summary>
        /// Invoked before triggering a door with SCP-079.
        /// </summary>
        public static event CustomEventHandler<TriggeringDoorEventArgs> TriggeringDoor;

        /// <summary>
        /// Invoked before SCP-079 teleports using an elevator.
        /// </summary>
        public static event CustomEventHandler<ElevatorTeleportingEventArgs> ElevatorTeleporting;

        /// <summary>
        /// Invoked before SCP-079 lockdowns a room.
        /// </summary>
        public static event CustomEventHandler<LockingDownEventArgs> LockingDown;

        /// <summary>
        /// Invoked before SCP-079 uses a speaker.
        /// </summary>
        public static event CustomEventHandler<StartingSpeakerEventArgs> StartingSpeaker;

        /// <summary>
        /// Invoked before SCP-079 finishes using a speaker.
        /// </summary>
        public static event CustomEventHandler<StoppingSpeakerEventArgs> StoppingSpeaker;

        /// <summary>
        /// Invoked after Scp079 recontainment.
        /// </summary>
        public static event CustomEventHandler<RecontainedEventArgs> Recontained;

        /// <summary>
        /// Called before SCP-079 switches cameras.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingCameraEventArgs"/> instance.</param>
        public static void OnChangingCamera(ChangingCameraEventArgs ev) => EventManager.Instance.Invoke<ChangingCameraEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnChangingCamera(ChangingCameraEventArgs ev) => ChangingCamera.InvokeSafely(ev);


        /// <summary>
        /// Called before gaining experience with SCP-079.
        /// </summary>
        /// <param name="ev">The <see cref="GainingExperienceEventArgs"/> instance.</param>
        public static void OnGainingExperience(GainingExperienceEventArgs ev) => EventManager.Instance.Invoke<GainingExperienceEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnGainingExperience(GainingExperienceEventArgs ev) => GainingExperience.InvokeSafely(ev);


        /// <summary>
        /// Called before gaining levels with SCP-079.
        /// </summary>
        /// <param name="ev">The <see cref="GainingLevelEventArgs"/> instance.</param>
        public static void OnGainingLevel(GainingLevelEventArgs ev) => EventManager.Instance.Invoke<GainingLevelEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnGainingLevel(GainingLevelEventArgs ev) => GainingLevel.InvokeSafely(ev);


        /// <summary>
        /// Called before triggering a tesla with SCP-079.
        /// </summary>
        /// <param name="ev">The <see cref="InteractingTeslaEventArgs"/> instance.</param>
        public static void OnInteractingTesla(InteractingTeslaEventArgs ev) => EventManager.Instance.Invoke<InteractingTeslaEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnInteractingTesla(InteractingTeslaEventArgs ev) => InteractingTesla.InvokeSafely(ev);


        /// <summary>
        /// Called before interacting with a door with SCP-079.
        /// </summary>
        /// <param name="ev">The <see cref="TriggeringDoorEventArgs"/> instance.</param>
        public static void OnTriggeringDoor(TriggeringDoorEventArgs ev) => EventManager.Instance.Invoke<TriggeringDoorEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnTriggeringDoor(TriggeringDoorEventArgs ev) => TriggeringDoor.InvokeSafely(ev);


        /// <summary>
        /// Called before SCP-079 teleports using an elevator.
        /// </summary>
        /// <param name="ev">The <see cref="ElevatorTeleportingEventArgs"/> instance.</param>
        public static void OnElevatorTeleporting(ElevatorTeleportingEventArgs ev) => EventManager.Instance.Invoke<ElevatorTeleportingEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnElevatorTeleporting(ElevatorTeleportingEventArgs ev) => ElevatorTeleporting.InvokeSafely(ev);


        /// <summary>
        /// Called before SCP-079 lockdowns a room.
        /// </summary>
        /// <param name="ev">The <see cref="LockingDownEventArgs"/> instance.</param>
        public static void OnLockingDown(LockingDownEventArgs ev) => EventManager.Instance.Invoke<LockingDownEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnLockingDown(LockingDownEventArgs ev) => LockingDown.InvokeSafely(ev);


        /// <summary>
        /// Called before interacting with a speaker with SCP-079.
        /// </summary>
        /// <param name="ev">The <see cref="StartingSpeakerEventArgs"/> instance.</param>
        public static void OnStartingSpeaker(StartingSpeakerEventArgs ev) => EventManager.Instance.Invoke<StartingSpeakerEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnStartingSpeaker(StartingSpeakerEventArgs ev) => StartingSpeaker.InvokeSafely(ev);


        /// <summary>
        /// Called before SCP-079 finishes using a speaker.
        /// </summary>
        /// <param name="ev">The <see cref="StoppingEventArgs"/> instance.</param>
        public static void OnStoppingSpeaker(StoppingSpeakerEventArgs ev) => EventManager.Instance.Invoke<StoppingSpeakerEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnStoppingSpeaker(StoppingSpeakerEventArgs ev) => StoppingSpeaker.InvokeSafely(ev);


        /// <summary>
        /// Called after SCP-079 is recontained.
        /// </summary>
        /// <param name="ev">The <see cref="RecontainedEventArgs"/> instance.</param>
        public static void OnRecontained(RecontainedEventArgs ev) => EventManager.Instance.Invoke<RecontainedEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnRecontained(RecontainedEventArgs ev) => Recontained.InvokeSafely(ev);

    }
}
