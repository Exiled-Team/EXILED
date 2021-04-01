// -----------------------------------------------------------------------
// <copyright file="MapHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Sexiled.API.Features;
using Sexiled.Events.EventArgs;

namespace Sexiled.Example.Events
{
    using System.Linq;

    using Sexiled.API.Features;
    using Sexiled.Events.EventArgs;

    /// <summary>
    /// Handles Map events.
    /// </summary>
    internal sealed class MapHandler
    {
        /// <inheritdoc cref="Sexiled.Events.Handlers.Map.OnExplodingGrenade(ExplodingGrenadeEventArgs)"/>
        public void OnExplodingGrenade(ExplodingGrenadeEventArgs ev)
        {
            Log.Info($"A frag grenade thrown by {ev.Thrower.Nickname} is exploding: {ev.Grenade.name}\n[Targets]\n\n{string.Join("\n", ev.TargetToDamages.Select(player => $"[{player.Key.Nickname} ({player.Value} HP)]"))}");
        }

        /// <inheritdoc cref="Sexiled.Events.Handlers.Map.OnGeneratorActivated(GeneratorActivatedEventArgs)"/>
        public void OnGeneratorActivated(GeneratorActivatedEventArgs ev)
        {
            Log.Info($"A generator has been activated in {ev.Generator.CurRoom}!");
        }
    }
}
