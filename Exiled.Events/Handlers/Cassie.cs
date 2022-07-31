// -----------------------------------------------------------------------
// <copyright file="Cassie.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs.Cassie;
    using Exiled.Events.Extensions;

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
        ///     Invoked before announcing the light containment zone decontamination.
        /// </summary>
        public static event CustomEventHandler<AnnouncingDecontaminationEventArgs> AnnouncingDecontamination;

        /// <summary>
        ///     Invoked before announcing an SCP termination.
        /// </summary>
        public static event CustomEventHandler<AnnouncingScpTerminationEventArgs> AnnouncingScpTermination;

        /// <summary>
        ///     Invoked before announcing the NTF entrance.
        /// </summary>
        public static event CustomEventHandler<AnnouncingNtfEntranceEventArgs> AnnouncingNtfEntrance;

        /// <summary>
        ///     Called before sending a cassie message.
        /// </summary>
        /// <param name="ev">The <see cref="SendingCassieMessageEventArgs" /> instance.</param>
        public static void OnSendingCassieMessage(SendingCassieMessageEventArgs ev)
        {
            SendingCassieMessage.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before announcing the light containment zone decontamination.
        /// </summary>
        /// <param name="ev">The <see cref="AnnouncingDecontaminationEventArgs" /> instance.</param>
        public static void OnAnnouncingDecontamination(AnnouncingDecontaminationEventArgs ev)
        {
            AnnouncingDecontamination.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before announcing an SCP termination.
        /// </summary>
        /// <param name="ev">The <see cref="AnnouncingScpTerminationEventArgs" /> instance.</param>
        public static void OnAnnouncingScpTermination(AnnouncingScpTerminationEventArgs ev)
        {
            AnnouncingScpTermination.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before announcing the NTF entrance.
        /// </summary>
        /// <param name="ev">The <see cref="AnnouncingNtfEntranceEventArgs" /> instance.</param>
        public static void OnAnnouncingNtfEntrance(AnnouncingNtfEntranceEventArgs ev)
        {
            AnnouncingNtfEntrance.InvokeSafely(ev);
        }
    }
}
