// -----------------------------------------------------------------------
// <copyright file="MapHandler.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Example.Events
{
    using System.Linq;

    using SEXiled.API.Features;
    using SEXiled.Events.EventArgs;

    /// <summary>
    /// Handles Map events.
    /// </summary>
    internal sealed class MapHandler
    {
        /// <inheritdoc cref="SEXiled.Events.Handlers.Map.OnExplodingGrenade(ExplodingGrenadeEventArgs)"/>
        public void OnExplodingGrenade(ExplodingGrenadeEventArgs ev)
        {
            Log.Info($"A grenade thrown by {ev.Thrower.Nickname} is exploding: {ev.Grenade.name}\n[Targets]\n\n{string.Join("\n", ev.TargetsToAffect.Select(player => $"[{player.Nickname}]"))}");
        }

        /// <inheritdoc cref="SEXiled.Events.Handlers.Map.OnGeneratorActivated(GeneratorActivatedEventArgs)"/>
        public void OnGeneratorActivated(GeneratorActivatedEventArgs ev)
        {
            Log.Info($"A generator has been activated in {ev.Generator.gameObject.GetComponent<Room>().Name}!");
        }
    }
}
