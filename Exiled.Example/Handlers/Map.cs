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
    /// Asd.
    /// </summary>
    internal class Map
    {
        /// <inheritdoc cref="Events.Handlers.Map.OnExplodingGrenade(ExplodingGrenadeEventArgs)"/>
        public void OnExplodingGrenade(ExplodingGrenadeEventArgs ev)
        {
            Log.Info($"A frag grenade is exploding: {ev.Grenade.name}, targets: {string.Join(", ", ev.Targets.Select(player => player.Nickname))}");
        }
    }
}
