// -----------------------------------------------------------------------
// <copyright file="PlayerHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.EventHandlers
{
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.CustomModules.API.Enums;
    using Exiled.CustomModules.API.Features;
    using Exiled.CustomModules.API.Features.CustomItems;
    using Exiled.Events.EventArgs.Player;

    /// <summary>
    /// Handles <see cref="Player"/> events.
    /// </summary>
    internal sealed class PlayerHandler
    {
        private ModuleInfo CustomItemsModuleInfo { get; } = ModuleInfo.Get(UUModuleType.CustomItems.Name);

        /// <inheritdoc cref="ChangingItemEventArgs"/>
        public void OnChangingItem(ChangingItemEventArgs ev)
        {
            if (!ev.IsAllowed || !CustomItemsModuleInfo.IsCurrentlyLoaded)
                return;

            if (CustomItem.TryGet(ev.Item, out CustomItem customItem) && customItem.Settings.NotifyItemToSpectators)
                SpectatorCustomNickname(ev.Player, $"{ev.Player.CustomName} (CustomItem: {customItem.Name})");
            else if (ev.Player && ev.Player.Cast(out Pawn pawn) && pawn.CurrentItem)
                SpectatorCustomNickname(ev.Player, ev.Player.HasCustomName ? ev.Player.CustomName : string.Empty);
        }

        private void SpectatorCustomNickname(Player player, string itemName)
        {
            foreach (Player target in Player.List)
                target.SendFakeSyncVar(player.NetworkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_displayName), itemName);
        }
    }
}