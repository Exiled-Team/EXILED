// -----------------------------------------------------------------------
// <copyright file="Scp096.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs;
    using Exiled.Events.Extensions;

    using static Exiled.Events.Events;

    /// <summary>
    /// SCP-096 related events.
    /// </summary>
    public static class Scp096
    {
        /// <summary>
        /// Invoked before enraging with SCP-096.
        /// </summary>
        public static event CustomEventHandler<EnragingEventArgs> Enraging;

        /// <summary>
        /// Invoked before calming down with SCP-096.
        /// </summary>
        public static event CustomEventHandler<CalmingDownEventArgs> CalmingDown;

        /// <summary>
        /// Invoked before enraging with SCP-096.
        /// </summary>
        /// <param name="ev">The <see cref="EnragingEventArgs"/> instance.</param>
        public static void OnEnraging(EnragingEventArgs ev) => Enraging.InvokeSafely(ev);

        /// <summary>
        /// Invoked before calming down with SCP-096.
        /// </summary>
        /// <param name="ev">The <see cref="CalmingDownEventArgs"/> instance.</param>
        public static void OnCalmingDown(CalmingDownEventArgs ev) => CalmingDown.InvokeSafely(ev);
    }
}
