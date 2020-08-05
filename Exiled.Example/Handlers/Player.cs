// -----------------------------------------------------------------------
// <copyright file="Player.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.Handlers
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using static Example;

    /// <summary>
    /// Handles player events.
    /// </summary>
    internal sealed class Player
    {
        /// <inheritdoc cref="Events.Handlers.Player.OnDied(DiedEventArgs)"/>
        public void OnDied(DiedEventArgs ev)
        {
            Log.Info($"{ev.Target?.Nickname} ({ev.Target?.Role}) died from {ev.HitInformations.GetDamageName()}! {ev.Killer?.Nickname} ({ev.Killer?.Role}) killed him!");
        }

        /// <inheritdoc cref="Events.Handlers.Player.OnChangingRole(ChangingRoleEventArgs)"/>
        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            Log.Info($"{ev.Player?.Nickname} ({ev.Player?.Role}) is changing his role! The new role will be {ev?.NewRole}!");
        }

        /// <inheritdoc cref="Events.Handlers.Player.OnChangingItem(ChangingItemEventArgs)"/>
        public void OnChangingItem(ChangingItemEventArgs ev)
        {
            Log.Info($"{ev.Player?.Nickname} is changing his {ev.OldItem.id} item to {ev.NewItem.id}!");
        }

        /// <inheritdoc cref="Events.Handlers.Scp106.OnTeleporting(TeleportingEventArgs)"/>
        public void OnTeleporting(TeleportingEventArgs ev)
        {
            Log.Info($"{ev.Player?.Nickname} is teleporting to {ev.PortalPosition} as SCP-106!");
        }

        /// <inheritdoc cref="Events.Handlers.Scp106.OnContaining(ContainingEventArgs)"/>
        public void OnContaining(ContainingEventArgs ev)
        {
            Log.Info($"{ev.Player?.Nickname} is being contained as SCP-106!");
        }

        /// <inheritdoc cref="Events.Handlers.Scp106.OnCreatingPortal(CreatingPortalEventArgs)"/>
        public void OnCreatingPortal(CreatingPortalEventArgs ev)
        {
            Log.Info($"{ev.Player?.Nickname} is creating a portal as SCP-106, in the position: {ev.Position}");
        }

        /// <inheritdoc cref="Events.Handlers.Scp914.OnActivating(ActivatingEventArgs)"/>
        public void OnActivating(ActivatingEventArgs ev)
        {
            Log.Info($"{ev.Player?.Nickname} is activating SCP-914!");
        }

        /// <inheritdoc cref="Events.Handlers.Player.OnFailingEscapePocketDimension(FailingEscapePocketDimensionEventArgs)"/>
        public void OnFailingEscapePocketDimension(FailingEscapePocketDimensionEventArgs ev)
        {
            Log.Info($"{ev.Player?.Nickname} is failing to escape from the pocket dimension!");
        }

        /// <inheritdoc cref="Events.Handlers.Player.OnEscapingPocketDimension(EscapingPocketDimensionEventArgs)"/>
        public void OnEscapingPocketDimension(EscapingPocketDimensionEventArgs ev)
        {
            Log.Info($"{ev.Player?.Nickname} is escaping from the pocket dimension and will be teleported at {ev.TeleportPosition}");
        }

        /// <inheritdoc cref="Events.Handlers.Scp914.OnChangingKnobSetting(ChangingKnobSettingEventArgs)"/>
        public void OnChangingKnobSetting(ChangingKnobSettingEventArgs ev)
        {
            Log.Info($"{ev.Player?.Nickname} is changing the knob setting of SCP-914 to {ev.KnobSetting}");
        }

        /// <inheritdoc cref="Events.Handlers.Player.OnJoined(JoinedEventArgs)"/>
        public void OnJoined(JoinedEventArgs ev)
        {
            if (!Instance.Config.JoinedBroadcast.Show)
                return;

            ev.Player.Broadcast(Instance.Config.JoinedBroadcast.Duration, Instance.Config.JoinedBroadcast.Content, Instance.Config.JoinedBroadcast.Type);
        }
    }
}
