// -----------------------------------------------------------------------
// <copyright file="Round.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.Internal
{
    using System.Collections.Generic;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Loader;
    using Exiled.Loader.Features;

    using InventorySystem;

    /// <summary>
    ///     Handles some round clean-up events and some others related to players.
    /// </summary>
    internal static class Round
    {
        /// <inheritdoc cref="Handlers.Server.OnWaitingForPlayers" />
        public static void OnWaitingForPlayers()
        {
            MultiAdminFeatures.CallEvent(MultiAdminFeatures.EventType.WAITING_FOR_PLAYERS);
            Item.BaseToItem.Clear();
            Pickup.BaseToItem.Clear();
            ExplosiveGrenade.GrenadeToItem.Clear();
            FlashGrenade.GrenadeToItem.Clear();

            if (Events.Instance.Config.ShouldReloadConfigsAtRoundRestart) ConfigManager.Reload();

            if (Events.Instance.Config.ShouldReloadTranslationsAtRoundRestart) TranslationManager.Reload();

            RoundSummary.RoundLock = false;
        }

        /// <inheritdoc cref="Handlers.Server.OnRestartingRound" />
        public static void OnRestartingRound()
        {
            MultiAdminFeatures.CallEvent(MultiAdminFeatures.EventType.ROUND_END);

            Scp173.TurnedPlayers.Clear();
            Scp096.TurnedPlayers.Clear();
            TeslaGate.IgnoredPlayers.Clear();
            TeslaGate.IgnoredRoles.Clear();
            TeslaGate.IgnoredTeams.Clear();
            Scp106Container.IgnoredPlayers.Clear();
            Scp106Container.IgnoredRoles = new List<RoleType> { RoleType.Spectator };
            Scp106Container.IgnoredTeams = new List<Team> { Team.SCP };
        }

        /// <inheritdoc cref="Handlers.Server.OnRoundStarted" />
        public static void OnRoundStarted()
        {
            MultiAdminFeatures.CallEvent(MultiAdminFeatures.EventType.ROUND_START);
        }

        /// <inheritdoc cref="Handlers.Player.OnChangingRole(ChangingRoleEventArgs)" />
        public static void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (ev.Player?.IsHost != false || string.IsNullOrEmpty(ev.Player.UserId))
                return;

            if (ev.NewRole == RoleType.Spectator && Events.Instance.Config.ShouldDropInventory)
                ev.Player.Inventory.ServerDropEverything();
        }
    }
}
