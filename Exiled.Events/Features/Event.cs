// -----------------------------------------------------------------------
// <copyright file="Event.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Features
{
    using System;

    using Exiled.API.Features;
    using Exiled.Events.Interfaces;

    /// <summary>
    /// The custom <see cref="EventHandler"/> delegate, with empty parameters.
    /// </summary>
    public delegate void CustomEventHandler();

    /// <summary>
    /// An implementation of <see cref="IEvent"/> that encapsulates a no-argument event.
    /// </summary>
    public class Event : IEvent
    {
        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="Event"/> class.
        /// </summary>
        /// <param name="name">The name of the event.</param>
        internal Event(string name)
        {
            this.name = name;
        }

        private event CustomEventHandler InnerEvent;

        /// <summary>
        /// Subscribes a <see cref="CustomEventHandler"/> to the inner event, and checks patches if dynamic patching is enabled.
        /// </summary>
        /// <param name="event">The <see cref="Event"/> to subscribe the <see cref="CustomEventHandler"/> to.</param>
        /// <param name="handler">The <see cref="CustomEventHandler"/> to subscribe to the <see cref="Event"/>.</param>
        /// <returns>The <see cref="Event"/> with the handler added to it.</returns>
        public static Event operator +(Event @event, CustomEventHandler handler)
        {
            if (Events.Instance.Config.UseDynamicPatching && @event.InnerEvent is null)
                Events.Instance.Patcher.Patch(@event);

            @event.InnerEvent += handler;

            return @event;
        }

        /// <summary>
        /// Unsubscribes a target <see cref="CustomEventHandler"/> from the inner event, and checks if unpatching is possible, if dynamic patching is enabled.
        /// </summary>
        /// <param name="event">The <see cref="Event"/> the <see cref="CustomEventHandler"/> will be unsubscribed from.</param>
        /// <param name="handler">The <see cref="CustomEventHandler"/> that will be unsubscribed from the <see cref="Event"/>.</param>
        /// <returns>The <see cref="Event"/> with the handler unsubscribed from it.</returns>
        public static Event operator -(Event @event, CustomEventHandler handler)
        {
            @event.InnerEvent -= handler;
            return @event;
        }

        /// <summary>
        /// Executes all <see cref="CustomEventHandler"/> listeners safely.
        /// </summary>
        public void InvokeSafely()
        {
            foreach (CustomEventHandler handler in InnerEvent.GetInvocationList())
            {
                try
                {
                    handler();
                }
                catch (Exception ex)
                {
                    Log.Error($"Method \"{handler.Method.Name}\" of the class \"{handler.Method.ReflectedType.FullName}\" caused an exception when handling the event \"{name}\"");
                    Log.Error(ex);
                }
            }
        }
    }
}
