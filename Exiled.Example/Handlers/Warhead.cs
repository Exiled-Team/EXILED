// -----------------------------------------------------------------------
// <copyright file="Warhead.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.Handlers
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    /// <summary>
    /// Handles warhead events.
    /// </summary>
    internal sealed class Warhead
    {
        /// <inheritdoc cref="Events.Handlers.Warhead.OnStopping(StoppingEventArgs)"/>
        public void OnStopping(StoppingEventArgs ev)
        {
            Log.Info($"{ev.Player?.Nickname} stopped the warhead!");
        }
    }
}
