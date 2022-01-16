// -----------------------------------------------------------------------
// <copyright file="Cassie.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.API.Utils;

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
        public static event CustomEventHandler<SendingCassieMessageEventArgs> SendingCassieMessage;

        /// <summary>
        /// Called before sending a cassie message.
        /// </summary>
        /// <param name="ev">The <see cref="SendingCassieMessageEventArgs"/> instance.</param>
        public static void OnSendingCassieMessage(SendingCassieMessageEventArgs ev) => EventManager.Instance.Invoke<SendingCassieMessageEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnSendingCassieMessage(SendingCassieMessageEventArgs ev) => SendingCassieMessage.InvokeSafely(ev);

    }
}
