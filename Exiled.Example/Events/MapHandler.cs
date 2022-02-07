// -----------------------------------------------------------------------
// <copyright file="MapHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.Events
{
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    /// <summary>
    /// Handles Map events.
    /// </summary>
    internal sealed class MapHandler
    {
        /// <inheritdoc cref="Exiled.Events.Handlers.Map.OnExplodingGrenade(ExplodingGrenadeEventArgs)"/>
        public void OnExplodingGrenade(ExplodingGrenadeEventArgs ev)
        {
            Log.Info($"A grenade thrown by {ev.Thrower.Nickname} is exploding: {ev.Grenade.name}\n[Targets]\n\n{string.Join("\n", ev.TargetsToAffect.Select(player => $"[{player.Nickname}]"))}");
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Map.OnGeneratorActivated(GeneratorActivatedEventArgs)"/>
        public void OnGeneratorActivated(GeneratorActivatedEventArgs ev)
        {
            Log.Info($"A generator has been activated in {ev.Generator.gameObject.GetComponent<Room>().Name}!");
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Map.OnAnnouncingDecontamination(AnnouncingDecontaminationEventArgs)"/>
        public void OnAnnouncingDecontamination(AnnouncingDecontaminationEventArgs ev)
        {
            Log.Info("Cassie is announcing decontamination");
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Map.OnAnnouncingNtfEntrance(AnnouncingNtfEntranceEventArgs)"/>
        public void OnAnnouncingNtfEntrance(AnnouncingNtfEntranceEventArgs ev)
        {
            Log.Info("Cassie is announcing a Ntf entrance");
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Map.OnAnnouncingScpTermination(AnnouncingScpTerminationEventArgs)"/>
        public void OnAnnouncingScpTermination(AnnouncingScpTerminationEventArgs ev)
        {
            Log.Info($"Cassie is announcing a Scp Termination with - Killer: {ev.Killer} Victim: {ev.Player} Scp Role: {ev.Role} Termination Cause: {ev.TerminationCause}");
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Map.OnDamagingWindow(DamagingWindowEventArgs)"/>
        public void OnDamagingWindow(DamagingWindowEventArgs ev)
        {
            Log.Info($"A window was damaged - Window Damage: {ev.Damage}");
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Map.OnPlacingBlood(PlacingBloodEventArgs)"/>
        public void OnPlacingBlood(PlacingBloodEventArgs ev)
        {
            Log.Info($"A blood decal was placed - Bleeding Player: {ev.Player} Position:{ev.Position} Multiplier: {ev.Multiplier}");
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Map.OnPlacingBulletHole(PlacingBulletHole)"/>
        public void OnPlacingBulletHole(PlacingBulletHole ev)
        {
            Log.Info($"A bullet hole decal was placed - Shooter Player: {ev.Owner} Position:{ev.Position} Rotation: {ev.Rotation}");
        }
    }
}
