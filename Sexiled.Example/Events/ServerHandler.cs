// -----------------------------------------------------------------------
// <copyright file="ServerHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Sexiled.API.Enums;
using Sexiled.API.Features;
using Sexiled.Events.EventArgs;

namespace Sexiled.Example.Events
{
    using Sexiled.API.Enums;
    using Sexiled.API.Features;
    using Sexiled.Events.EventArgs;

    /// <summary>
    /// Handles server-related events.
    /// </summary>
    internal sealed class ServerHandler
    {
        /// <inheritdoc cref="Sexiled.Events.Handlers.Server.OnWaitingForPlayers"/>
        public void OnWaitingForPlayers()
        {
            Log.Info("I'm waiting for players!"); // This is an example of information messages sent to your console!
        }

        /// <inheritdoc cref="Sexiled.Events.Handlers.Server.OnEndingRound(EndingRoundEventArgs)"/>
        public void OnEndingRound(EndingRoundEventArgs ev)
        {
            Log.Debug($"The round is ending, fetching leading team...");

            if (ev.LeadingTeam == LeadingTeam.Draw)
                Log.Error($"The round has ended in a draw!");
            else
                Log.Info($"The leading team is actually: {ev.LeadingTeam}.");
        }
    }
}
