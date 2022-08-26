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

    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Loader;
    using Exiled.Loader.Features;

    using InventorySystem;

    using MEC;

    using Item = Exiled.API.Features.Items.Item;
    using Log = Exiled.API.Features.Log;
    using Player = Exiled.API.Features.Player;
    using PlayerHandler = Exiled.Events.Handlers.Player;
    using Scp173 = Exiled.API.Features.Scp173;

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

            if (Events.Instance.Config.ShouldReloadConfigsAtRoundRestart)
                ConfigManager.Reload();

            if (Events.Instance.Config.ShouldReloadTranslationsAtRoundRestart)
                TranslationManager.Reload();

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

        /// <inheritdoc cref="PlayerHandler.OnChangingRole(ChangingRoleEventArgs)"/>
        /// <inheritdoc cref="Handlers.Player.OnChangingRole(ChangingRoleEventArgs)" />
        public static void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (ev.Player?.IsHost != false || string.IsNullOrEmpty(ev.Player.UserId))
                return;

            if (ev.NewRole == RoleType.Spectator && Events.Instance.Config.ShouldDropInventory)
                ev.Player.Inventory.ServerDropEverything();

            if (ev.NewRole is RoleType.Scp173)
                Scp173.TurnedPlayers.Remove(ev.Player);

            ev.Player.Role = API.Features.Roles.Role.Create(ev.NewRole, ev.Player);

            ev.Player.MaxHealth = CharacterClassManager._staticClasses.SafeGet(ev.NewRole).maxHP;
        }

        /// <inheritdoc cref="PlayerHandler.Verified"/>
        public static void OnVerified(VerifiedEventArgs ev)
        {
#if DEBUG
            Log.Debug($"{ev.Player.Nickname} has verified!");
#endif
            API.Features.Player.Dictionary.Add(ev.Player.GameObject, ev.Player);
            ev.Player.IsVerified = true;
            ev.Player.RawUserId = ev.Player.UserId.GetRawUserId();
            Log.SendRaw($"Player {ev.Player.Nickname} ({ev.Player.UserId}) ({ev.Player.Id}) connected with the IP: {ev.Player.IPAddress}", ConsoleColor.Green);
        }

        /// <inheritdoc cref="PlayerHandler.Joined"/>
        public static void OnJoined(JoinedEventArgs ev)
        {
#if DEBUG
            Log.Debug($"Object exists {ev.Player is not null}");
            Log.Debug($"Created player object for {ev.Player.ReferenceHub.nicknameSync.Network_displayName}", true);
#endif
            API.Features.Player.UnverifiedPlayers.Add(ev.Player.ReferenceHub, ev.Player);
            Timing.CallDelayed(0.25f, () =>
            {
                if (ev.Player.IsMuted)
                    ev.Player.ReferenceHub.characterClassManager.SetDirtyBit(2UL);
            });
        }

        /// <inheritdoc cref="PlayerHandler.Left"/>
        public static void OnLeft(LeftEventArgs ev)
        {
            Log.SendRaw($"Player {ev.Player.Nickname} has disconnected", ConsoleColor.Green);
        }

        /// <inheritdoc cref="PlayerHandler.Destroying"/>
        public static void OnDestroying(DestroyingEventArgs ev)
        {
            Player.Dictionary.Remove(ev.Player.GameObject);
            Player.UnverifiedPlayers.Remove(ev.Player.ReferenceHub);
            Player.IdsCache.Remove(ev.Player.Id);

            if (ev.Player.UserId is not null)
                Player.UserIdsCache.Remove(ev.Player.UserId);
        }
    }
}
