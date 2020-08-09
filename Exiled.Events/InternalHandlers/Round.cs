// -----------------------------------------------------------------------
// <copyright file="Round.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.InternalHandlers
{
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;
    using Exiled.Loader;
    using Exiled.Loader.Features;

    using MEC;

    /// <summary>
    /// Handles some round clean-up events and some others related to players.
    /// </summary>
    public sealed class Round
    {
        /// <inheritdoc cref="Server.OnWaitingForPlayers"/>
        public void OnWaitingForPlayers()
        {
            MultiAdminFeatures.CallEvent(MultiAdminFeatures.EventType.WAITING_FOR_PLAYERS);

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
            MultiAdminFeatures.CallEvent(MultiAdminFeatures.EventType.ROUND_END);

            API.Features.Map.ClearCache();
        }

        /// <inheritdoc cref="Server.OnRoundStarted"/>
        public void OnRoundStarted()
        {
            MultiAdminFeatures.CallEvent(MultiAdminFeatures.EventType.ROUND_START);

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
