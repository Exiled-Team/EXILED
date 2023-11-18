// -----------------------------------------------------------------------
// <copyright file="Round.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.Internal
{
    using CentralAuth;
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Scp049;
    using Exiled.Loader;
    using Exiled.Loader.Features;

    using InventorySystem;

    using PlayerRoles;
    using PlayerRoles.RoleAssign;

    /// <summary>
    ///     Handles some round clean-up events and some others related to players.
    /// </summary>
    internal static class Round
    {
        /// <inheritdoc cref="Handlers.Server.OnWaitingForPlayers" />
        public static void OnWaitingForPlayers()
        {
            MultiAdminFeatures.CallEvent(MultiAdminFeatures.EventType.WAITING_FOR_PLAYERS);

            if (Events.Instance.Config.ShouldReloadConfigsAtRoundRestart)
                ConfigManager.Reload();

            if (Events.Instance.Config.ShouldReloadTranslationsAtRoundRestart)
                TranslationManager.Reload();

            RoundSummary.RoundLock = false;
        }

        /// <inheritdoc cref="Handlers.Server.OnRestartingRound" />
        public static void OnRestartingRound()
        {
            Scp049Role.TurnedPlayers.Clear();
            Scp173Role.TurnedPlayers.Clear();
            Scp096Role.TurnedPlayers.Clear();
            Scp079Role.TurnedPlayers.Clear();

            MultiAdminFeatures.CallEvent(MultiAdminFeatures.EventType.ROUND_END);

            TeslaGate.IgnoredPlayers.Clear();
            TeslaGate.IgnoredRoles.Clear();
            TeslaGate.IgnoredTeams.Clear();

            API.Features.Round.IgnoredPlayers.Clear();
        }

        /// <inheritdoc cref="Handlers.Server.OnRoundStarted" />
        public static void OnRoundStarted() => MultiAdminFeatures.CallEvent(MultiAdminFeatures.EventType.ROUND_START);

        /// <inheritdoc cref="Handlers.Player.OnChangingRole(ChangingRoleEventArgs)" />
        public static void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (!ev.Player.IsHost && ev.NewRole == RoleTypeId.Spectator && ev.Reason != API.Enums.SpawnReason.Destroyed && Events.Instance.Config.ShouldDropInventory)
                ev.Player.Inventory.ServerDropEverything();
        }

        /// <inheritdoc cref="Scp049.OnActivatingSense(ActivatingSenseEventArgs)" />
        public static void OnActivatingSense(ActivatingSenseEventArgs ev)
        {
            if (Events.Instance.Config.CanScp049SenseTutorial || ev.Target is null || ev.Target.Role.Type is not RoleTypeId.Tutorial)
                return;
            ev.Target = ev.Scp049.SenseAbility.CanFindTarget(out ReferenceHub hub) ? Player.Get(hub) : null;
        }

        /// <inheritdoc cref="Handlers.Player.OnVerified(VerifiedEventArgs)" />
        public static void OnVerified(VerifiedEventArgs ev)
        {
            RoleAssigner.CheckLateJoin(ev.Player.ReferenceHub, ClientInstanceMode.ReadyClient);
        }
    }
}
