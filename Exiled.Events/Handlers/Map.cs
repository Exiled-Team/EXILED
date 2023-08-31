// -----------------------------------------------------------------------
// <copyright file="Map.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.API.Features.Pickups;
    using Exiled.Events.EventArgs.Map;

    using Extensions;

    using MapGeneration.Distributors;

    using static Events;

    /// <summary>
    /// Map related events.
    /// </summary>
    public static class Map
    {
        /// <summary>
        /// Invoked before placing decals.
        /// </summary>
        public static event CustomEventHandler<PlacingBulletHole> PlacingBulletHole;

        /// <summary>
        /// Invoked before placing bloods.
        /// </summary>
        public static event CustomEventHandler<PlacingBloodEventArgs> PlacingBlood;

        /// <summary>
        /// Invoked before announcing the light containment zone decontamination.
        /// </summary>
        public static event CustomEventHandler<AnnouncingDecontaminationEventArgs> AnnouncingDecontamination;

        /// <summary>
        /// Invoked before announcing an SCP termination.
        /// </summary>
        public static event CustomEventHandler<AnnouncingScpTerminationEventArgs> AnnouncingScpTermination;

        /// <summary>
        /// Invoked before announcing the NTF entrance.
        /// </summary>
        public static event CustomEventHandler<AnnouncingNtfEntranceEventArgs> AnnouncingNtfEntrance;

        /// <summary>
        /// Invoked after a <see cref="Scp079Generator"/> has been activated.
        /// </summary>
        public static event CustomEventHandler<GeneratorActivatedEventArgs> GeneratorActivated;

        /// <summary>
        /// Invoked before decontaminating the light containment zone.
        /// </summary>
        public static event CustomEventHandler<DecontaminatingEventArgs> Decontaminating;

        /// <summary>
        /// Invoked before a grenade explodes.
        /// </summary>
        public static event CustomEventHandler<ExplodingGrenadeEventArgs> ExplodingGrenade;

        /// <summary>
        /// Invoked before an item is spawned.
        /// </summary>
        public static event CustomEventHandler<SpawningItemEventArgs> SpawningItem;

        /// <summary>
        /// Invoked before an item is spawned in a locker.
        /// </summary>
        public static event CustomEventHandler<SpawningItemInLockerEventArgs> SpawningItemInLocker;

        /// <summary>
        /// Invoked after the map is generated.
        /// </summary>
        public static event CustomEventHandler Generated;

        /// <summary>
        /// Invoked before the server changes a pickup into a grenade, when triggered by an explosion.
        /// </summary>
        public static event CustomEventHandler<ChangingIntoGrenadeEventArgs> ChangingIntoGrenade;

        /// <summary>
        /// Invoked after the server changes a pickup into a grenade, when triggered by an explosion.
        /// </summary>
        public static event CustomEventHandler<ChangedIntoGrenadeEventArgs> ChangedIntoGrenade;

        /// <summary>
        /// Invoked before turning off lights.
        /// </summary>
        public static event CustomEventHandler<TurningOffLightsEventArgs> TurningOffLights;

        /// <summary>
        /// Invoked after an pickup is spawned.
        /// </summary>
        public static event CustomEventHandler<PickupAddedEventArgs> PickupAdded;

        /// <summary>
        /// Invoked after an pickup is destroyed.
        /// </summary>
        public static event CustomEventHandler<PickupDestroyedEventArgs> PickupDestroyed;

        /// <summary>
        /// Called before placing a decal.
        /// </summary>
        /// <param name="ev">The <see cref="EventArgs.Map.PlacingBulletHole"/> instance.</param>
        public static void OnPlacingBulletHole(PlacingBulletHole ev) => PlacingBulletHole.InvokeSafely(ev);

        /// <summary>
        /// Called before placing bloods.
        /// </summary>
        /// <param name="ev">The <see cref="PlacingBloodEventArgs"/> instance.</param>
        public static void OnPlacingBlood(PlacingBloodEventArgs ev) => PlacingBlood.InvokeSafely(ev);

        /// <summary>
        /// Called before announcing the light containment zone decontamination.
        /// </summary>
        /// <param name="ev">The <see cref="AnnouncingDecontaminationEventArgs"/> instance.</param>
        public static void OnAnnouncingDecontamination(AnnouncingDecontaminationEventArgs ev) => AnnouncingDecontamination.InvokeSafely(ev);

        /// <summary>
        /// Called before announcing an SCP termination.
        /// </summary>
        /// <param name="ev">The <see cref="AnnouncingScpTerminationEventArgs"/> instance.</param>
        public static void OnAnnouncingScpTermination(AnnouncingScpTerminationEventArgs ev) => AnnouncingScpTermination.InvokeSafely(ev);

        /// <summary>
        /// Called before announcing the NTF entrance.
        /// </summary>
        /// <param name="ev">The <see cref="AnnouncingNtfEntranceEventArgs"/> instance.</param>
        public static void OnAnnouncingNtfEntrance(AnnouncingNtfEntranceEventArgs ev) => AnnouncingNtfEntrance.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="Scp079Generator"/> has been activated.
        /// </summary>
        /// <param name="ev">The <see cref="GeneratorActivatedEventArgs"/> instance.</param>
        public static void OnGeneratorActivated(GeneratorActivatedEventArgs ev) => GeneratorActivated.InvokeSafely(ev);

        /// <summary>
        /// Called before decontaminating the light containment zone.
        /// </summary>
        /// <param name="ev">The <see cref="DecontaminatingEventArgs"/> instance.</param>
        public static void OnDecontaminating(DecontaminatingEventArgs ev) => Decontaminating.InvokeSafely(ev);

        /// <summary>
        /// Called before a grenade explodes.
        /// </summary>
        /// <param name="ev">The <see cref="ExplodingGrenadeEventArgs"/> instance.</param>
        public static void OnExplodingGrenade(ExplodingGrenadeEventArgs ev) => ExplodingGrenade.InvokeSafely(ev);

        /// <summary>
        /// Called before an item is spawned.
        /// </summary>
        /// <param name="ev">The <see cref="SpawningItemEventArgs"/> instance.</param>
        public static void OnSpawningItem(SpawningItemEventArgs ev) => SpawningItem.InvokeSafely(ev);

        /// <summary>
        /// Called before an item is spawned in a locker.
        /// </summary>
        /// <param name="ev">The <see cref="SpawningItemInLockerEventArgs"/> instance.</param>
        public static void OnSpawningItemInLocker(SpawningItemInLockerEventArgs ev) => SpawningItemInLocker.InvokeSafely(ev);

        /// <summary>
        /// Called after the map is generated.
        /// </summary>
        public static void OnGenerated() => Generated.InvokeSafely();

        /// <summary>
        /// Called before the server changes a <see cref="Pickup"/> into a live Grenade when hit by an explosion.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingIntoGrenadeEventArgs"/> instance.</param>
        public static void OnChangingIntoGrenade(ChangingIntoGrenadeEventArgs ev) => ChangingIntoGrenade.InvokeSafely(ev);

        /// <summary>
        /// Called after the server changes a <see cref="Pickup"/> into a live Grenade when hit by an explosion.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingIntoGrenadeEventArgs"/> instance.</param>
        public static void OnChangedIntoGrenade(ChangedIntoGrenadeEventArgs ev) => ChangedIntoGrenade.InvokeSafely(ev);

        /// <summary>
        /// Called before turning off lights.
        /// </summary>
        /// <param name="ev">The <see cref="TurningOffLightsEventArgs"/> instance.</param>
        public static void OnTurningOffLights(TurningOffLightsEventArgs ev) => TurningOffLights.InvokeSafely(ev);

        /// <summary>
        /// Called after an pickup is spawned.
        /// </summary>
        /// <param name="ev">The <see cref="PickupAddedEventArgs"/> instance.</param>
        public static void OnPickupAdded(PickupAddedEventArgs ev) => PickupAdded.InvokeSafely(ev);

        /// <summary>
        /// Called after an pickup is destroyed.
        /// </summary>
        /// <param name="ev">The <see cref="PickupDestroyedEventArgs"/> instance.</param>
        public static void OnPickupDestroyed(PickupDestroyedEventArgs ev) => PickupDestroyed.InvokeSafely(ev);
    }
}