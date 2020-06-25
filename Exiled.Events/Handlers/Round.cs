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
    public class Round
    {
        /// <inheritdoc cref="Server.OnWaitingForPlayers"/>
        public void OnWaitingForPlayers()
        {
            if (Events.Instance.Config.ShouldReloadConfigsAtRoundRestart)
                ConfigManager.Reload();

            API.Features.Map.Rooms.Clear();
            API.Features.Map.Doors.Clear();
            API.Features.Map.Lifts.Clear();
            API.Features.Map.TeslaGates.Clear();

            API.Features.Player.IdsCache.Clear();
            API.Features.Player.UserIdsCache.Clear();
            API.Features.Player.Dictionary.Clear();
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
            if (ev.Player == null || ev.Player.IsHost || string.IsNullOrEmpty(ev.Player.UserId))
                return;

            if (ev.NewRole == RoleType.Spectator && Events.Instance.Config.ShouldDropInventory)
                ev.Player.Inventory.ServerDropAll();
        }
    }
}
