// -----------------------------------------------------------------------
// <copyright file="Scp096Handler.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Example.Events
{
    using SEXiled.API.Features;
    using SEXiled.Events.EventArgs;

    /// <summary>
    /// Handles SCP-096 events.
    /// </summary>
    internal sealed class Scp096Handler
    {
        /// <inheritdoc cref="SEXiled.Events.Handlers.Scp096.OnAddingTarget(AddingTargetEventArgs)"/>
        public void OnAddingTarget(AddingTargetEventArgs ev)
        {
            Log.Info($"{ev.Target.Nickname} is being added to {ev.Scp096.Nickname} targets! Enrage time to add: {ev.EnrageTimeToAdd}");
        }
    }
}
