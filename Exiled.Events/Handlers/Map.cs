// -----------------------------------------------------------------------
// <copyright file="Map.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs;
    using Exiled.Events.Features;

    using MapGeneration.Distributors;

    /// <summary>
    /// Map related events.
    /// </summary>
    public static class Map
    {
        /// <summary>
        /// Invoked before placing decals.
        /// </summary>
        public static readonly Event<PlacingBulletHole> PlacingBulletHole = new();

        /// <summary>
        /// Invoked before placing bloods.
        /// </summary>
        public static readonly Event<PlacingBloodEventArgs> PlacingBlood = new();

        /// <summary>
        /// Invoked before announcing the light containment zone decontamination.
        /// </summary>
        public static readonly Event<AnnouncingDecontaminationEventArgs> AnnouncingDecontamination = new();

        /// <summary>
        /// Invoked before announcing an SCP termination.
        /// </summary>
        public static readonly Event<AnnouncingScpTerminationEventArgs> AnnouncingScpTermination = new();

        /// <summary>
        /// Invoked before announcing the NTF entrance.
        /// </summary>
        public static readonly Event<AnnouncingNtfEntranceEventArgs> AnnouncingNtfEntrance = new();

        /// <summary>
        /// Invoked after a <see cref="Scp079Generator"/> has been activated.
        /// </summary>
        public static readonly Event<GeneratorActivatedEventArgs> GeneratorActivated = new();

        /// <summary>
        /// Invoked before decontaminating the light containment zone.
        /// </summary>
        public static readonly Event<DecontaminatingEventArgs> Decontaminating = new();

        /// <summary>
        /// Invoked before a grenade explodes.
        /// </summary>
        public static readonly Event<ExplodingGrenadeEventArgs> ExplodingGrenade = new();

        /// <summary>
        /// Invoked before an item is spawned.
        /// </summary>
        public static readonly Event<SpawningItemEventArgs> SpawningItem = new();

        /// <summary>
        /// Invoked after the map is generated.
        /// </summary>
        public static readonly Event Generated = new();

        /// <summary>
        /// Invoked before the server changes a pickup into a grenade, when triggered by an explosion.
        /// </summary>
        public static readonly Event<ChangingIntoGrenadeEventArgs> ChangingIntoGrenade = new();

        /// <summary>
        /// Called before placing a decal.
        /// </summary>
        /// <param name="ev">The <see cref="EventArgs.PlacingBulletHole"/> instance.</param>
        public static void OnPlacingBulletHole(PlacingBulletHole ev) => PlacingBulletHole.InvokeSafely(ev);

        /// <summary>
        /// Called before placing bloods.
        /// </summary>
        /// <param name="ev">The <see cref="EventArgs.PlacingBulletHole"/> instance.</param>
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
        /// Called after the map is generated.
        /// </summary>
        public static void OnGenerated() => Generated.InvokeSafely();

        /// <summary>
        /// Called before the server changes a <see cref="Pickup"/> into a live Grenade when hit by an explosion.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingIntoGrenadeEventArgs"/> instance.</param>
        public static void OnChangingIntoGrenade(ChangingIntoGrenadeEventArgs ev) => ChangingIntoGrenade.InvokeSafely(ev);
    }
}
