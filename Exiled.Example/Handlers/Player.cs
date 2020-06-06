// -----------------------------------------------------------------------
// <copyright file="Player.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.Handlers
{
    using Exiled.API.Features;
    using Exiled.Events.Handlers.EventArgs;

    /// <summary>
    /// Handles player events.
    /// </summary>
    public class Player
    {
        /// <inheritdoc cref="OnDied(DiedEventArgs)"/>
        public void OnDied(DiedEventArgs ev)
        {
            Log.Info($"{ev.Target.Nickname} died from {ev.HitInformations.GetDamageName()}! {ev.Killer.Nickname} killed him!");
        }

        /// <inheritdoc cref="OnChangingRole(ChangingRoleEventArgs)"/>
        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            Log.Info($"{ev.Player} is changing his role! The new role will be {ev.NewRole}!");
        }
    }
}
