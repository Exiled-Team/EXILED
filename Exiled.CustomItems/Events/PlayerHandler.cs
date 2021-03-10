// -----------------------------------------------------------------------
// <copyright file="PlayerHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems
{
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs;

    /// <summary>
    /// Handles Player events for the CustomItem API.
    /// </summary>
    internal sealed class PlayerHandler
    {
        /// <inheritdoc cref="ChangingRoleEventArgs"/>
        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (ev.NewRole == RoleType.Spectator)
            {
                foreach (Player player in Player.List)
                {
                    if (player == ev.Player)
                        continue;

                    if (CustomItem.TryGet(player, out CustomItem item))
                    {
                        if (item.ShouldMessageOnGban)
                            ev.Player.SendFakeSyncVar(player.ReferenceHub.networkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_myNickSync), $"{ev.Player.Nickname} (CustomItem: {item.Name})");
                    }
                }
            }
            else
            {
                foreach (Player player in Player.List)
                {
                    if (player == ev.Player)
                        continue;

                    ev.Player.SendFakeSyncVar(player.ReferenceHub.networkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_myNickSync), player.Nickname);
                }
            }
        }
    }
}
