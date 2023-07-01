// -----------------------------------------------------------------------
// <copyright file="Scp0492.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs.Scp0492;
    using Exiled.Events.Extensions;

    /// <summary>
    /// <see cref="API.Features.Roles.Scp0492Role"/> related events.
    /// </summary>
    public class Scp0492
    {
        /// <summary>
        /// Called before a player triggers the bloodlust effect for 049-2.
        /// </summary>
        public static event Events.CustomEventHandler<TriggeringBloodlustEventArgs> TriggeringBloodlust;

        /// <summary>
        /// Called before a player triggers the bloodlust effect for 049-2.
        /// </summary>
        /// <param name="ev">The <see cref="TriggeringBloodlustEventArgs"/> instance.</param>
        public static void OnTriggeringBloodlust(TriggeringBloodlustEventArgs ev) => TriggeringBloodlust.InvokeSafely(ev);
    }
}