// -----------------------------------------------------------------------
// <copyright file="Round.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using System.Linq;

    using Exiled.Events.Handlers.EventArgs;

    using MEC;

    /// <summary>
    /// Handles some round clean-up events and some others related to players.
    /// </summary>
    public class Round
    {
        /// <inheritdoc cref="Server.OnWaitingForPlayers"/>
        public void OnWaitingForPlayers()
        {
            Core.API.Features.Map.Rooms.Clear();
            Core.API.Features.Map.Doors.Clear();
            Core.API.Features.Map.Lifts.Clear();
            Core.API.Features.Map.TeslaGates.Clear();

            Core.API.Features.Player.IdsCache.Clear();
            Core.API.Features.Player.UserIdsCache.Clear();
        }

        /// <inheritdoc cref="Server.OnRoundStarted"/>
        public void OnRoundStarted()
        {
            foreach (Core.API.Features.Player player in Core.API.Features.Player.List)
            {
                if (player.IsOverwatchEnabled)
                {
                    player.IsOverwatchEnabled = false;
                    Timing.CallDelayed(2f, () => player.IsOverwatchEnabled = true);
                }
            }
        }

        /// <inheritdoc cref="Player.OnLeft(LeftEventArgs)"/>
        public void OnPlayerLeft(LeftEventArgs ev)
        {
            if (Core.API.Features.Player.IdsCache.ContainsKey(ev.Player.Id))
                Core.API.Features.Player.IdsCache.Remove(ev.Player.Id);

            foreach (var entry in Core.API.Features.Player.UserIdsCache.Where(userId => userId.Value == ev.Player).ToList())
                Core.API.Features.Player.UserIdsCache.Remove(entry.Key);

            if (!Core.API.Features.Player.Dictionary.ContainsKey(ev.Player.GameObject))
                Core.API.Features.Player.Dictionary.Remove(ev.Player.GameObject);
        }

        /// <inheritdoc cref="Player.OnJoined(JoinedEventArgs)"/>
        public void OnPlayerJoined(JoinedEventArgs ev)
        {
            if (ev.Player == null || ev.Player.IsHost || string.IsNullOrEmpty(ev.Player.UserId))
                return;

            if (!Core.API.Features.Player.Dictionary.ContainsKey(ev.Player.GameObject))
                Core.API.Features.Player.Dictionary.Add(ev.Player.GameObject, ev.Player);
        }

        /// <inheritdoc cref="Player.OnChangingRole(ChangingRoleEventArgs)"/>
        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (ev.Player == null || ev.Player.IsHost || string.IsNullOrEmpty(ev.Player.UserId))
                return;

            if (ev.NewRole == RoleType.Spectator && Config.ShouldDropInventory)
                ev.Player.Inventory.ServerDropAll();
        }
    }
}
