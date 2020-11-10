// -----------------------------------------------------------------------
// <copyright file="Scp079.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs;
    using Exiled.Events.Extensions;

    using static Exiled.Events.Events;

    /// <summary>
    /// SCP-079 related events.
    /// </summary>
    public static class Scp079
    {
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
        public static event CustomEventHandler<InteractingDoorEventArgs> InteractingDoor;

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
        /// <param name="ev">The <see cref="InteractingDoorEventArgs"/> instance.</param>
        public static void OnInteractingDoor(InteractingDoorEventArgs ev) => InteractingDoor.InvokeSafely(ev);

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
