// -----------------------------------------------------------------------
// <copyright file="Player.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.Handlers
{
    using System;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    /// <summary>
    /// Handles player events.
    /// </summary>
    internal sealed class Player
    {
        /// <inheritdoc cref="Events.Handlers.Player.OnDied(DiedEventArgs)"/>
        public void OnDied(DiedEventArgs ev)
        {
            Log.Info($"{ev.Target?.Nickname} died from {ev.HitInformations.GetDamageName()}! {ev.Killer?.Nickname} killed him!");
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
            Log.Info($"{ev.Player?.Nickname} has teleported to {ev.PortalPosition} with SCP-106!");
        }

        /// <inheritdoc cref="Events.Handlers.Scp106.OnContaining(ContainingEventArgs)"/>
        public void OnContaining(ContainingEventArgs ev)
        {
            Log.Info($"{ev.Player?.Nickname} is being contained as SCP-106!");
        }
    }
}
