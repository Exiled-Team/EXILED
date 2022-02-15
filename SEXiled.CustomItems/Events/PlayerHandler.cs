// -----------------------------------------------------------------------
// <copyright file="PlayerHandler.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.CustomItems
{
    using SEXiled.API.Extensions;
    using SEXiled.API.Features;
    using SEXiled.CustomItems.API.Features;
    using SEXiled.Events.EventArgs;

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
                    {
                        continue;
                    }

                    if (CustomItem.TryGet(player, out CustomItem item))
                    {
                        if (item.ShouldMessageOnGban)
                        {
                            ev.Player.SendFakeSyncVar(player.ReferenceHub.networkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_displayName), $"{player.Nickname} (CustomItem: {item.Name})");
                        }
                    }
                }
            }
            else
            {
                foreach (Player player in Player.List)
                {
                    if (player == ev.Player || player.ReferenceHub.nicknameSync.Network_displayName == null)
                        continue;

                    ev.Player.SendFakeSyncVar(player.ReferenceHub.networkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_displayName), player.DisplayNickname);
                }
            }
        }
    }
}
