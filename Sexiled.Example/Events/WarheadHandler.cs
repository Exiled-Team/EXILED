// -----------------------------------------------------------------------
// <copyright file="WarheadHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Sexiled.API.Features;
using Sexiled.Events.EventArgs;

namespace Sexiled.Example.Events
{
    using Sexiled.API.Features;
    using Sexiled.Events.EventArgs;

    /// <summary>
    /// Handles warhead events.
    /// </summary>
    internal sealed class WarheadHandler
    {
        /// <inheritdoc cref="Sexiled.Events.Handlers.Warhead.OnStopping(StoppingEventArgs)"/>
        public void OnStopping(StoppingEventArgs ev)
        {
            Log.Info($"{ev.Player?.Nickname} stopped the warhead!");
        }

        /// <inheritdoc cref="Sexiled.Events.Handlers.Warhead.OnStarting(StartingEventArgs)"/>
        public void OnStarting(StartingEventArgs ev)
        {
            Log.Info($"{ev.Player?.Nickname} started the warhead!");
        }
    }
}
