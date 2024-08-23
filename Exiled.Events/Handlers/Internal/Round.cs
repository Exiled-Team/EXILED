// -----------------------------------------------------------------------
// <copyright file="Round.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CentralAuth;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Scp049;
    using Exiled.Events.EventArgs.Server;

    using Exiled.Loader;
    using Exiled.Loader.Features;
    using InventorySystem;
    using InventorySystem.Items.Usables;
    using MEC;

    using PlayerRoles;
    using PlayerRoles.RoleAssign;

    /// <summary>
    /// Handles some round clean-up events and some others related to players.
    /// </summary>
    internal static class Round
    {
#pragma warning disable SA1600
#pragma warning disable SA1401
        internal static CoroutineHandle EndOfSupportHandle;
#pragma warning restore SA1401
#pragma warning restore SA1600

        /// <inheritdoc cref="Handlers.Player.OnUsedItem" />
        public static void OnServerOnUsingCompleted(ReferenceHub hub, UsableItem usable) => Handlers.Player.OnUsedItem(new (hub, usable));

        /// <inheritdoc cref="Handlers.Server.OnWaitingForPlayers" />
        public static void OnWaitingForPlayers()
        {
            MultiAdminFeatures.CallEvent(MultiAdminFeatures.EventType.WAITING_FOR_PLAYERS);

            EndOfSupportHandle = Timing.RunCoroutine(EndOfSupportNotification());

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

        /// <inheritdoc cref="Handlers.Server.OnRoundEnded" />
        public static void OnRoundEnded(RoundEndedEventArgs ev) => Timing.KillCoroutines(EndOfSupportHandle);

        /// <inheritdoc cref="Handlers.Player.OnChangingRole(ChangingRoleEventArgs)" />
        public static void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (!ev.Player.IsHost && ev.NewRole == RoleTypeId.Spectator && ev.Reason != API.Enums.SpawnReason.Destroyed && Events.Instance.Config.ShouldDropInventory)
                ev.Player.Inventory.ServerDropEverything();
        }

        /// <inheritdoc cref="Scp049.OnActivatingSense(ActivatingSenseEventArgs)" />
        public static void OnActivatingSense(ActivatingSenseEventArgs ev)
        {
            if (ev.Target is null)
                return;
            if ((Events.Instance.Config.CanScp049SenseTutorial || ev.Target.Role.Type is not RoleTypeId.Tutorial) && !Scp049Role.TurnedPlayers.Contains(ev.Target))
                return;
            ev.Target = ev.Scp049.SenseAbility.CanFindTarget(out ReferenceHub hub) ? Player.Get(hub) : null;
        }

        /// <inheritdoc cref="Handlers.Player.OnVerified(VerifiedEventArgs)" />
        public static void OnVerified(VerifiedEventArgs ev)
        {
            RoleAssigner.CheckLateJoin(ev.Player.ReferenceHub, ClientInstanceMode.ReadyClient);

            // TODO: Remove if this has been fixed for https://trello.com/c/CzPD304L/5983-networking-blackout-is-not-synchronized-for-the-new-players
            foreach (Room room in Room.List.Where(current => current.AreLightsOff))
            {
                ev.Player.SendFakeSyncVar(room.RoomLightControllerNetIdentity, typeof(RoomLightController), nameof(RoomLightController.NetworkLightsEnabled), true);
                ev.Player.SendFakeSyncVar(room.RoomLightControllerNetIdentity, typeof(RoomLightController), nameof(RoomLightController.NetworkLightsEnabled), false);
            }
        }

        private static IEnumerator<float> EndOfSupportNotification()
        {
            if (LoaderPlugin.Config.Reboot)
                yield break;

            for (; ;)
            {
                ServerConsole.AddLog("Exiled support has ended. For updates and new releases, join us at https://exiled.to/discord!", ConsoleColor.DarkRed);
                ServerConsole.AddLog("Exiled Reboot will not load plugins until you acknowledge this message by setting the 'Reboot' Loader config to true.", ConsoleColor.DarkRed);
                ServerConsole.AddLog("Run the commands 'exiled reboot', 'exiled ack' or 'exiled exboot' to temporary suppress the logs.", ConsoleColor.DarkRed);

                yield return Timing.WaitForSeconds(20f);
            }
        }
    }
}
