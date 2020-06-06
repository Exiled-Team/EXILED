// -----------------------------------------------------------------------
// <copyright file="Scp106.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.API.Extensions;
    using Exiled.Events.EventArgs;
    using static Exiled.Events.Events;

    /// <summary>
    /// SCP-106 related events.
    /// </summary>
    public class Scp106
    {
        /// <summary>
        /// Invoked before creating an SCP-106 portal.
        /// </summary>
        public static event CustomEventHandler<CreatingPortalEventArgs> CreatingPortal;

        /// <summary>
        /// Invoked before teleporting with SCP-106.
        /// </summary>
        public static event CustomEventHandler<TeleportingEventArgs> Teleporting;

        /// <summary>
        /// Invoked before containing SCP-106.
        /// </summary>
        public static event CustomEventHandler<ContainingEventArgs> Containing;

        /// <summary>
        /// Invoked before creating an SCP-106 portal.
        /// </summary>
        /// <param name="ev">The <see cref="CreatingPortalEventArgs"/> instance.</param>
        public static void OnCreatingPortal(CreatingPortalEventArgs ev) => CreatingPortal.InvokeSafely(ev);

        /// <summary>
        /// Invoked before teleporting with SCP-106.
        /// </summary>
        /// <param name="ev">The <see cref="TeleportingEventArgs"/> instance.</param>
        public static void OnTeleporting(TeleportingEventArgs ev) => Teleporting.InvokeSafely(ev);

        /// <summary>
        /// Invoked before containing SCP-106.
        /// </summary>
        /// <param name="ev">The <see cref="ContainingEventArgs"/> instance.</param>
        public static void OnContaining(ContainingEventArgs ev) => Containing.InvokeSafely(ev);
    }
}
