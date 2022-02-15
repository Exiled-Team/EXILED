// -----------------------------------------------------------------------
// <copyright file="Cassie.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.Handlers
{
    using SEXiled.Events.EventArgs;
    using SEXiled.Events.Extensions;

    using static SEXiled.Events.Events;

    /// <summary>
    /// Cassie related events.
    /// </summary>
    public static class Cassie
    {
        /// <summary>
        /// Invoked before sending a cassie message.
        /// </summary>
        public static event CustomEventHandler<SendingCassieMessageEventArgs> SendingCassieMessage;

        /// <summary>
        /// Called before sending a cassie message.
        /// </summary>
        /// <param name="ev">The <see cref="SendingCassieMessageEventArgs"/> instance.</param>
        public static void OnSendingCassieMessage(SendingCassieMessageEventArgs ev) => SendingCassieMessage.InvokeSafely(ev);
    }
}
