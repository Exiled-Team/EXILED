// -----------------------------------------------------------------------
// <copyright file="Server.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.Handlers
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    /// <summary>
    /// Handles server-related events.
    /// </summary>
    internal class Server
    {
        /// <inheritdoc cref="Events.Handlers.Server.OnWaitingForPlayers"/>
        public void OnWaitingForPlayers()
        {
            Log.Info("I'm waiting for players!");
        }

        /// <inheritdoc cref="Events.Handlers.Server.OnEndingRound(EndingRoundEventArgs)"/>
        public void OnRoundEnd(RoundEndedEventArgs ev)
        {
            if (ev.LeadingTeam == RoundSummary.LeadingTeam.Draw)
            {
                Log.Debug($"The round has ended in a draw!");
                return;
            }

            Log.Warn($"The actual leading team is: {ev.LeadingTeam}");
        }
    }
}
