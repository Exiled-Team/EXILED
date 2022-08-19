// -----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Metrics;

using Exiled.API.Features;

using MapEvents = Exiled.Events.Handlers.Map;
using PlayerEvents = Exiled.Events.Handlers.Player;
using Scp049Events = Exiled.Events.Handlers.Scp049;
using Scp079Events = Exiled.Events.Handlers.Scp079;
using Scp096Events = Exiled.Events.Handlers.Scp096;
using Scp106Events = Exiled.Events.Handlers.Scp106;
using Scp914Events = Exiled.Events.Handlers.Scp914;
using ServerEvents = Exiled.Events.Handlers.Server;
using WarheadEvents = Exiled.Events.Handlers.Warhead;

/// <inheritdoc />
public class Plugin : Plugin<Config>
{
    /// <summary>
    /// Gets the class reference for general methods.
    /// </summary>
    public Methods Methods { get; private set; }

    /// <summary>
    /// Gets the class reference for general event handlers.
    /// </summary>
    public EventHandlers EventHandlers { get; private set; }

    /// <inheritdoc/>
    public override void OnEnabled()
    {
        EventHandlers = new EventHandlers(this);
        Methods = new Methods(this);

        ServerEvents.RoundStarted += EventHandlers.OnRoundStarted;
        ServerEvents.RoundEnded += EventHandlers.OnRoundEnded;

        base.OnEnabled();
    }

    /// <inheritdoc/>
    public override void OnDisabled()
    {
        ServerEvents.RoundStarted -= EventHandlers.OnRoundStarted;
        ServerEvents.RoundEnded -= EventHandlers.OnRoundEnded;

        EventHandlers = null;
        Methods = null;

        base.OnDisabled();
    }
}
