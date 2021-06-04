// -----------------------------------------------------------------------
// <copyright file="Player.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using System;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Extensions;

    using static Exiled.Events.Events;

    /// <summary>
    /// Cassie related events.
    /// </summary>
    public static class Cassie
    {
        /// <summary>
        /// Invoked before authenticating a player.
        /// </summary>
        public static event CustomEventHandler<SendingMessageEventArgs> SendingMessage;

        /// <summary>
        /// Called before pre-authenticating a player.
        /// </summary>
        /// <param name="ev">The <see cref="PreAuthenticatingEventArgs"/> instance.</param>
        public static void OnSendingMessage(SendingMessageEventArgs ev) => SendingMessage.InvokeSafely(ev);
    }
}
