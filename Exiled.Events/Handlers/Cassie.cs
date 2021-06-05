// -----------------------------------------------------------------------
// <copyright file="Cassie.cs" company="Exiled Team">
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
        /// Invoked before sending a cassie message.
        /// </summary>
        public static event CustomEventHandler<SendingMessageEventArgs> SendingMessage;

        /// <summary>
        /// Called before sending a cassie message.
        /// </summary>
        /// <param name="ev">The <see cref="SendingMessageEventArgs"/> instance.</param>
        public static void OnSendingMessage(SendingMessageEventArgs ev) => SendingMessage.InvokeSafely(ev);
    }
}
