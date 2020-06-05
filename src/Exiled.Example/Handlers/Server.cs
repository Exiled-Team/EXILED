// -----------------------------------------------------------------------
// <copyright file="Server.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.Handlers
{
    using Exiled.API.Features;
    using Exiled.Events.Handlers.EventArgs;

    /// <summary>
    /// Handles server-related events.
    /// </summary>
    internal class Server
    {
        /// <inheritdoc cref="Events.Handlers.Server.OnWaitingForPlayers"/>
        public void OnWaitingForPlayers()
        {
            Log.Warn("I'm waiting for players!");
        }

        /// <inheritdoc cref="Events.Handlers.Server.OnEndingRound(EndingRoundEventArgs)"/>
        public void OnEndingRound(EndingRoundEventArgs ev)
        {
            Log.Warn($"The actual leading team is: {ev.LeadingTeam}");

            if (ev.LeadingTeam == RoundSummary.LeadingTeam.Draw)
                ev.IsAllowed = false;
        }
    }
}
