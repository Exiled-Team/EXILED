// -----------------------------------------------------------------------
// <copyright file="Scp096Handler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.Events
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Scp096;

    /// <summary>
    /// Handles SCP-096 events.
    /// </summary>
    internal sealed class Scp096Handler
    {
        /// <inheritdoc cref="Exiled.Events.Handlers.Scp096.OnAddingTarget(AddingTargetEventArgs)"/>
        public void OnAddingTarget(AddingTargetEventArgs ev)
        {
            Log.Info($"{ev.Target.Nickname} is being added to {ev.Player.Nickname} targets!");
        }
    }
}