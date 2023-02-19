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
        /// <inheritdoc cref="ChangingItemEventArgs"/>
        public void OnChangingItem(ChangingItemEventArgs ev)
        {
            if (!ev.IsAllowed)
                return;
            if (CustomItem.TryGet(ev.NewItem, out CustomItem newItem))
            {
                SpectatorCustomNickname(ev.Player, $"{ev.Player.CustomName} (CustomItem: {newItem.Name})");
            }
            else if (ev.Player != null && CustomItem.TryGet(ev.Player.CurrentItem, out CustomItem _))
            {
                SpectatorCustomNickname(ev.Player, ev.Player.HasCustomName ? ev.Player.CustomName : null);
            }
        }

        private void SpectatorCustomNickname(Player player, string itemName)
        {
            foreach (Player spetator in Player.List)
            {
                spetator.SendFakeSyncVar(player.NetworkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_displayName), itemName);
            }
        }
    }
}