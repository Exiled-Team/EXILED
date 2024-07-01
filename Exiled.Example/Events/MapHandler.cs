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
    using Exiled.Events.EventArgs.Map;

    /// <summary>
    /// Handles Map events.
    /// </summary>
    internal sealed class MapHandler
    {
        /// <inheritdoc cref="Exiled.Events.Handlers.Map.OnExplodingGrenade(ExplodingGrenadeEventArgs)"/>
        public void OnExplodingGrenade(ExplodingGrenadeEventArgs ev)
        {
            Log.Info($"A grenade thrown by {ev.Player.Nickname} is exploding: {ev.Projectile.Type}\n[Targets]\n\n{string.Join("\n", ev.TargetsToAffect.Select(player => $"[{player.Nickname}]"))}");
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Map.OnGeneratorActivating"/>
        public void OnGeneratorActivated(GeneratorActivatingEventArgs ev)
        {
            Log.Info($"A generator has been activated in {ev.Generator.Room.Type}!");
        }
    }
}