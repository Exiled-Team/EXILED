// -----------------------------------------------------------------------
// <copyright file="PlayerHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.Events
{
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs.Player;

    using PlayerRoles;

    /// <summary>
    /// Handles Player events for the CustomItem API.
    /// </summary>
    internal sealed class PlayerHandler
    {
        /// <inheritdoc cref="ChangingSpectatedPlayerEventArgs"/>
        public void OnChangingSpectatedPlayer(ChangingSpectatedPlayerEventArgs ev)
        {
            if (ev.NewTarget == null || !ev.Player.IsGlobalModerator)
                return;

            if (CustomItem.TryGet(ev.NewTarget, out CustomItem item))
            {
                if (item.ShouldMessageOnGban)
                {
                    ev.Player.SendFakeSyncVar(ev.NewTarget.NetworkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_displayName), $"{ev.NewTarget.CustomName} (CustomItem: {item.Name})");
                }
            }
            else if (ev.OldTarget != null)
            {
                ev.Player.SendFakeSyncVar(ev.OldTarget.NetworkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_displayName), ev.OldTarget.HasCustomName ? ev.OldTarget.CustomName : null);
            }
        }
    }
}