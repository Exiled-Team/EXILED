// -----------------------------------------------------------------------
// <copyright file="WarheadHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.Events
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    /// <summary>
    /// Handles warhead events.
    /// </summary>
    internal sealed class WarheadHandler
    {
        /// <inheritdoc cref="Exiled.Events.Handlers.Warhead.OnStopping(StoppingWarheadEventArgs)"/>
        public void OnStopping(StoppingWarheadEventArgs ev)
        {
            Log.Info($"{ev.Player.Nickname} stopped the warhead!");
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Warhead.OnStarting(StartingWarheadEventArgs)"/>
        public void OnStarting(StartingWarheadEventArgs ev)
        {
            Log.Info($"{ev.Player.Nickname} started the warhead!");
        }
    }
}
