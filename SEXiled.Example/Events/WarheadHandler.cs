// -----------------------------------------------------------------------
// <copyright file="WarheadHandler.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Example.Events
{
    using SEXiled.API.Features;
    using SEXiled.Events.EventArgs;

    /// <summary>
    /// Handles warhead events.
    /// </summary>
    internal sealed class WarheadHandler
    {
        /// <inheritdoc cref="SEXiled.Events.Handlers.Warhead.OnStopping(StoppingEventArgs)"/>
        public void OnStopping(StoppingEventArgs ev)
        {
            Log.Info($"{ev.Player.Nickname} stopped the warhead!");
        }

        /// <inheritdoc cref="SEXiled.Events.Handlers.Warhead.OnStarting(StartingEventArgs)"/>
        public void OnStarting(StartingEventArgs ev)
        {
            Log.Info($"{ev.Player.Nickname} started the warhead!");
        }
    }
}
