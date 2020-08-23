// -----------------------------------------------------------------------
// <copyright file="Round.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs;
    using Exiled.Loader;

    using MEC;

    /// <summary>
    /// Handles some round clean-up events and some others related to players.
    /// </summary>
    public sealed class Round
    {
        /// <inheritdoc cref="Server.OnWaitingForPlayers"/>
        public void OnWaitingForPlayers()
        {
            API.Features.Player.IdsCache.Clear();
            API.Features.Player.UserIdsCache.Clear();
            API.Features.Player.Dictionary.Clear();

            if (Events.Instance.Config.ShouldReloadConfigsAtRoundRestart)
                ConfigManager.Reload();

            RoundSummary.RoundLock = false;
        }

        /// <inheritdoc cref="Server.OnRestartingRound"/>
        public void OnRestartingRound()
        {
            API.Features.Map.ClearCache();
            API.Features.Scp173.TurnedPlayers.Clear();
            API.Features.Scp096.TurnedPlayers.Clear();
        }

        /// <inheritdoc cref="Server.OnRoundStarted"/>
        public void OnRoundStarted()
        {
            foreach (API.Features.Player player in API.Features.Player.List)
            {
                if (player.IsOverwatchEnabled)
                {
                    player.IsOverwatchEnabled = false;
                    Timing.CallDelayed(2f, () => player.IsOverwatchEnabled = true);
                }
            }
        }

        /// <inheritdoc cref="Player.OnChangingRole(ChangingRoleEventArgs)"/>
        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (ev.Player?.IsHost != false || string.IsNullOrEmpty(ev.Player.UserId))
                return;

            if (ev.NewRole == RoleType.Spectator && Events.Instance.Config.ShouldDropInventory)
                ev.Player.Inventory.ServerDropAll();
        }
    }
}
