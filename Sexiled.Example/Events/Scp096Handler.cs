// -----------------------------------------------------------------------
// <copyright file="Scp096Handler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Sexiled.API.Features;
using Sexiled.Events.EventArgs;

namespace Sexiled.Example.Events
{
    using Sexiled.API.Features;
    using Sexiled.Events.EventArgs;

    /// <summary>
    /// Handles SCP-096 events.
    /// </summary>
    internal sealed class Scp096Handler
    {
        /// <inheritdoc cref="Sexiled.Events.Handlers.Scp096.OnAddingTarget(AddingTargetEventArgs)"/>
        public void OnAddingTarget(AddingTargetEventArgs ev)
        {
            Log.Info($"{ev.Target.Nickname} is being added to {ev.Scp096.Nickname} targets! AHP to add: {ev.AhpToAdd}, Enrage time to add: {ev.EnrageTimeToAdd}");
        }
    }
}
