// -----------------------------------------------------------------------
// <copyright file="Map.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.Handlers
{
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    /// <summary>
    /// Handles Map events.
    /// </summary>
    internal sealed class Map
    {
        /// <inheritdoc cref="Events.Handlers.Map.OnExplodingGrenade(ExplodingGrenadeEventArgs)"/>
        public void OnExplodingGrenade(ExplodingGrenadeEventArgs ev)
        {
            Log.Info($"A frag grenade thrown by {ev.Thrower.Nickname} is exploding: {ev.Grenade.name}\n[Targets]\n\n{string.Join("\n", ev.TargetToDamages.Select(player => $"[{player.Key.Nickname} ({player.Value} HP)]"))}");
        }
    }
}
