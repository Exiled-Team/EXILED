// -----------------------------------------------------------------------
// <copyright file="Map.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using System;
    using Exiled.API.Extensions;
    using Exiled.Events.Handlers.EventArgs;

    /// <summary>
    /// Map related events.
    /// </summary>
    public class Map
    {
        /// <summary>
        /// Invoked before placing decals.
        /// </summary>
        public static event EventHandler<PlacingDecalEventArgs> PlacingDecal;

        /// <summary>
        /// Invoked before placing bloods.
        /// </summary>
        public static event EventHandler<PlacingBloodEventArgs> PlacingBlood;

        /// <summary>
        /// Invoked before announcing the light containment zone decontamination.
        /// </summary>
        public static event EventHandler<AnnouncingDecontaminationEventArgs> AnnouncingDecontamination;

        /// <summary>
        /// Invoked before announcing an SCP termination.
        /// </summary>
        public static event EventHandler<AnnouncingScpTerminationEventArgs> AnnouncingScpTermination;

        /// <summary>
        /// Invoked before announcing the NTF entrance.
        /// </summary>
        public static event EventHandler<AnnouncingNtfEntranceEventArgs> AnnouncingNtfEntrance;

        /// <summary>
        /// Invoked before interacting with a door.
        /// </summary>
        public static event EventHandler<InteractingDoorEventArgs> InteractingDoor;

        /// <summary>
        /// Invoked before interacting with an elevator.
        /// </summary>
        public static event EventHandler<InteractingElevatorEventArgs> InteractingElevator;

        /// <summary>
        /// Invoked before interacting with a locker.
        /// </summary>
        public static event EventHandler<InteractingLockerEventArgs> InteractingLocker;

        /// <summary>
        /// Invoked before triggering a tesla.
        /// </summary>
        public static event EventHandler<TriggeringTeslaEventArgs> TriggeringTesla;

        /// <summary>
        /// Invoked before upgrading items in the SCP-914 machine.
        /// </summary>
        public static event EventHandler<UpgradingScp914ItemsEventArgs> UpgradingScp914Items;

        /// <summary>
        /// Invoked before unlocking a generator.
        /// </summary>
        public static event EventHandler<UnlockingGeneratorEventArgs> UnlockingGenerator;

        /// <summary>
        /// Invoked before opening a generator.
        /// </summary>
        public static event EventHandler<OpeningGeneratorEventArgs> OpeningGenerator;

        /// <summary>
        /// Invoked befroe closing a generator.
        /// </summary>
        public static event EventHandler<ClosingGeneratorEventArgs> ClosingGenerator;

        /// <summary>
        /// Invoked before inserting a generator.
        /// </summary>
        public static event EventHandler<InsertingGeneratorTabletEventArgs> InsertingGeneratorTablet;

        /// <summary>
        /// Invoked before ejecting a generator.
        /// </summary>
        public static event EventHandler<EjectingGeneratorTabletEventArgs> EjectingGeneratorTablet;

        /// <summary>
        /// Invoked after a generator has been activated.
        /// </summary>
        public static event EventHandler<GeneratorActivatedEventArgs> GeneratorActivated;

        /// <summary>
        /// Invoked before decontaminating the light containment zone.
        /// </summary>
        public static event EventHandler<DecontaminatingEventArgs> Decontaminating;

        /// <summary>
        /// Invoked before stopping the warhead.
        /// </summary>
        public static event EventHandler<StoppingWarheadEventArgs> StoppingWarhead;

        /// <summary>
        /// Invoked before starting the warhead.
        /// </summary>
        public static event EventHandler<StartingWarheadEventArgs> StartingWarhead;

        /// <summary>
        /// Invoked after the warhead has been detonated.
        /// </summary>
        public static event EventHandler WarheadDetonated;

        /// <summary>
        /// Called before placing a decal.
        /// </summary>
        /// <param name="ev">The <see cref="PlacingDecalEventArgs"/> instance.</param>
        public static void OnPlacingDecal(PlacingDecalEventArgs ev) => PlacingDecal.InvokeSafely(null, ev);

        /// <summary>
        /// Called before placing bloods.
        /// </summary>
        /// <param name="ev">The <see cref="PlacingDecalEventArgs"/> instance.</param>
        public static void OnPlacingBlood(PlacingBloodEventArgs ev) => PlacingBlood.InvokeSafely(null, ev);

        /// <summary>
        /// Called before announcing the light containment zone decontamination.
        /// </summary>
        /// <param name="ev">The <see cref="PlacingDecalEventArgs"/> instance.</param>
        public static void OnAnnouncingDecontamination(AnnouncingDecontaminationEventArgs ev) => AnnouncingDecontamination.InvokeSafely(null, ev);

        /// <summary>
        /// Called before announcing an SCP termination.
        /// </summary>
        /// <param name="ev">The <see cref="PlacingDecalEventArgs"/> instance.</param>
        public static void OnAnnouncingScpTermination(AnnouncingScpTerminationEventArgs ev) => AnnouncingScpTermination.InvokeSafely(null, ev);

        /// <summary>
        /// Called before announcing the NTF entrance.
        /// </summary>
        /// <param name="ev">The <see cref="PlacingDecalEventArgs"/> instance.</param>
        public static void OnAnnouncingNtfEntrance(AnnouncingNtfEntranceEventArgs ev) => AnnouncingNtfEntrance.InvokeSafely(null, ev);

        /// <summary>
        /// Called before interacting with a door.
        /// </summary>
        /// <param name="ev">The <see cref="PlacingDecalEventArgs"/> instance.</param>
        public static void OnInteractingDoor(InteractingDoorEventArgs ev) => InteractingDoor.InvokeSafely(null, ev);

        /// <summary>
        /// Called before interacting with an elevator.
        /// </summary>
        /// <param name="ev">The <see cref="PlacingDecalEventArgs"/> instance.</param>
        public static void OnInteractingElevator(InteractingElevatorEventArgs ev) => InteractingElevator.InvokeSafely(null, ev);

        /// <summary>
        /// Called before interacting with a locker.
        /// </summary>
        /// <param name="ev">The <see cref="PlacingDecalEventArgs"/> instance.</param>
        public static void OnInteractingLocker(InteractingLockerEventArgs ev) => InteractingLocker.InvokeSafely(null, ev);

        /// <summary>
        /// Called before triggering a tesla.
        /// </summary>
        /// <param name="ev">The <see cref="TriggeringTeslaEventArgs"/> instance.</param>
        public static void OnTriggeringTesla(TriggeringTeslaEventArgs ev) => TriggeringTesla.InvokeSafely(null, ev);

        /// <summary>
        /// Called before upgrading items in the SCP-914 machine.
        /// </summary>
        /// <param name="ev">The <see cref="UpgradingScp914ItemsEventArgs"/> instance.</param>
        public static void OnUpgradingScp914Items(UpgradingScp914ItemsEventArgs ev) => UpgradingScp914Items.InvokeSafely(null, ev);

        /// <summary>
        /// Called before unlocking a generator.
        /// </summary>
        /// <param name="ev">The <see cref="UnlockingGeneratorEventArgs"/> instance.</param>
        public static void OnUnlockingGenerator(UnlockingGeneratorEventArgs ev) => UnlockingGenerator.InvokeSafely(null, ev);

        /// <summary>
        /// Called before opening a generator.
        /// </summary>
        /// <param name="ev">The <see cref="OpeningGeneratorEventArgs"/> instance.</param>
        public static void OnOpeningGenerator(OpeningGeneratorEventArgs ev) => OpeningGenerator.InvokeSafely(null, ev);

        /// <summary>
        /// Called before closing a generator.
        /// </summary>
        /// <param name="ev">The <see cref="ClosingGeneratorEventArgs"/> instance.</param>
        public static void OnClosingGenerator(ClosingGeneratorEventArgs ev) => ClosingGenerator.InvokeSafely(null, ev);

        /// <summary>
        /// Called before inserting a generator.
        /// </summary>
        /// <param name="ev">The <see cref="InsertingGeneratorTabletEventArgs"/> instance.</param>
        public static void OnInsertingGeneratorTablet(InsertingGeneratorTabletEventArgs ev) => InsertingGeneratorTablet.InvokeSafely(null, ev);

        /// <summary>
        /// Called before ejecting a generator.
        /// </summary>
        /// <param name="ev">The <see cref="EjectingGeneratorTabletEventArgs"/> instance.</param>
        public static void OnEjectingGeneratorTablet(EjectingGeneratorTabletEventArgs ev) => EjectingGeneratorTablet.InvokeSafely(null, ev);

        /// <summary>
        /// Called after a generator has been activated.
        /// </summary>
        /// <param name="ev">The <see cref="GeneratorActivatedEventArgs"/> instance.</param>
        public static void OnGeneratorActivated(GeneratorActivatedEventArgs ev) => GeneratorActivated.InvokeSafely(null, ev);

        /// <summary>
        /// Called before decontaminating the light containment zone.
        /// </summary>
        /// <param name="ev">The <see cref="DecontaminatingEventArgs"/> instance.</param>
        public static void OnDecontaminating(DecontaminatingEventArgs ev) => Decontaminating.InvokeSafely(null, ev);

        /// <summary>
        /// Called before stopping the warhead.
        /// </summary>
        /// <param name="ev">The <see cref="StoppingWarheadEventArgs"/> instance.</param>
        public static void OnStoppingWarhead(StoppingWarheadEventArgs ev) => StoppingWarhead.InvokeSafely(null, ev);

        /// <summary>
        /// Called before starting the warhead.
        /// </summary>
        /// <param name="ev">The <see cref="StartingWarheadEventArgs"/> instance.</param>
        public static void OnStartingWarhead(StartingWarheadEventArgs ev) => StartingWarhead.InvokeSafely(null, ev);

        /// <summary>
        /// Called after the warhead has been detonated.
        /// </summary>
        public static void OnWarheadDetonated() => WarheadDetonated.InvokeSafely(null, System.EventArgs.Empty);
    }
}