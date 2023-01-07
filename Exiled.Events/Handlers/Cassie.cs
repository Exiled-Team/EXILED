// -----------------------------------------------------------------------
// <copyright file="Cassie.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs.Cassie;

    using Extensions;

    using static Events;

    /// <summary>
    ///     Cassie related events.
    /// </summary>
    public static class Cassie
    {
        /// <summary>
        ///     Invoked before sending a cassie message.
        /// </summary>
        public static event CustomEventHandler<SendingCassieMessageEventArgs> SendingCassieMessage;

        /// <summary>
        ///     Called before sending a cassie message.
        /// </summary>
        /// <param name="ev">The <see cref="SendingCassieMessageEventArgs" /> instance.</param>
        public static void OnSendingCassieMessage(SendingCassieMessageEventArgs ev) => SendingCassieMessage.InvokeSafely(ev);
    }
}