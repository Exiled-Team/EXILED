// -----------------------------------------------------------------------
// <copyright file="Event.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;
    using MEC;

    /// <summary>
    /// The custom <see cref="EventHandler"/> delegate, with empty parameters.
    /// </summary>
    public delegate void CustomEventHandler();

    /// <summary>
    /// THe custom <see cref="EventHandler"/> delegate, with empty parameters. Holds async events with <see cref="MEC"/>.
    /// </summary>
    /// <returns><see cref="IEnumerator{T}"/> of <see cref="float"/>.</returns>
    public delegate IEnumerator<float> CustomAsyncEventHandler();

    /// <summary>
    /// An implementation of <see cref="IExiledEvent"/> that encapsulates a no-argument event.
    /// </summary>
    public class Event : IExiledEvent
    {
        private static readonly List<Event> EventsValue = new();

        private bool patched;

        /// <summary>
        /// Initializes a new instance of the <see cref="Event"/> class.
        /// </summary>
        public Event()
        {
            EventsValue.Add(this);
        }

        private event CustomEventHandler InnerEvent;

        private event CustomAsyncEventHandler InnerAsyncEvent;

        /// <summary>
        /// Gets a <see cref="IReadOnlyList{T}"/> of <see cref="Event{T}"/> which contains all the <see cref="Event{T}"/> instances.
        /// </summary>
        public static IReadOnlyList<Event> List => EventsValue;

        /// <summary>
        /// Gets a <see cref="IReadOnlyCollection{T}"/> of delegates that are subscribed to the inner event.
        /// </summary>
        public IReadOnlyCollection<Delegate> SubscribedPlugins
        {
            get
            {
                List<Delegate> list = InnerEvent?.GetInvocationList().ToList() ?? new List<Delegate>();
                if (InnerAsyncEvent != null)
                    list.AddRange(InnerAsyncEvent.GetInvocationList().ToList());

                return list;
            }
        }

        /// <summary>
        /// Subscribes a <see cref="CustomEventHandler"/> to the inner event, and checks patches if dynamic patching is enabled.
        /// </summary>
        /// <param name="event">The <see cref="Event"/> to subscribe the <see cref="CustomEventHandler"/> to.</param>
        /// <param name="handler">The <see cref="CustomEventHandler"/> to subscribe to the <see cref="Event"/>.</param>
        /// <returns>The <see cref="Event"/> with the handler added to it.</returns>
        public static Event operator +(Event @event, CustomEventHandler handler)
        {
            @event.Subscribe(handler);
            return @event;
        }

        /// <summary>
        /// Subscribes a <see cref="CustomAsyncEventHandler"/> to the inner event, and checks patches if dynamic patching is enabled.
        /// </summary>
        /// <param name="event">The <see cref="Event"/> to subscribe the <see cref="CustomAsyncEventHandler"/> to.</param>
        /// <param name="asyncEventHandler">The <see cref="CustomAsyncEventHandler"/> to subscribe to the <see cref="Event"/>.</param>
        /// <returns>The <see cref="Event"/> with the handler added to it.</returns>
        public static Event operator +(Event @event, CustomAsyncEventHandler asyncEventHandler)
        {
            @event.Subscribe(asyncEventHandler);
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
            @event.Unsubscribe(handler);
            return @event;
        }

        /// <summary>
        /// Unsubscribes a target <see cref="CustomAsyncEventHandler"/> from the inner event, and checks if unpatching is possible, if dynamic patching is enabled.
        /// </summary>
        /// <param name="event">The <see cref="Event"/> the <see cref="CustomAsyncEventHandler"/> will be unsubscribed from.</param>
        /// <param name="asyncEventHandler">The <see cref="CustomAsyncEventHandler"/> that will be unsubscribed from the <see cref="Event"/>.</param>
        /// <returns>The <see cref="Event"/> with the handler unsubscribed from it.</returns>
        public static Event operator -(Event @event, CustomAsyncEventHandler asyncEventHandler)
        {
            @event.Unsubscribe(asyncEventHandler);
            return @event;
        }

        /// <summary>
        /// Subscribes a target <see cref="CustomEventHandler"/> to the inner event if the conditional is true.
        /// </summary>
        /// <param name="handler">The handler to add.</param>
        public void Subscribe(CustomEventHandler handler)
        {
            Log.Assert(Events.Instance is not null, $"{nameof(Events.Instance)} is null, please ensure you have exiled_events enabled!");

            if (Events.Instance.Config.UseDynamicPatching && !patched)
            {
                Events.Instance.Patcher.Patch(this);
                patched = true;
            }

            InnerEvent += handler;
        }

        /// <summary>
        /// Subscribes a target <see cref="CustomAsyncEventHandler"/> to the inner event if the conditional is true.
        /// </summary>
        /// <param name="handler">The handler to add.</param>
        public void Subscribe(CustomAsyncEventHandler handler)
        {
            Log.Assert(Events.Instance is not null, $"{nameof(Events.Instance)} is null, please ensure you have exiled_events enabled!");

            if (Events.Instance.Config.UseDynamicPatching && !patched)
            {
                Events.Instance.Patcher.Patch(this);
                patched = true;
            }

            InnerAsyncEvent += handler;
        }

        /// <summary>
        /// Unsubscribes a target <see cref="CustomEventHandler"/> from the inner event if the conditional is true.
        /// </summary>
        /// <param name="handler">The handler to add.</param>
        public void Unsubscribe(CustomEventHandler handler)
        {
            InnerEvent -= handler;
        }

        /// <summary>
        /// Unsubscribes a target <see cref="CustomAsyncEventHandler"/> from the inner event if the conditional is true.
        /// </summary>
        /// <param name="handler">The handler to add.</param>
        public void Unsubscribe(CustomAsyncEventHandler handler)
        {
            InnerAsyncEvent -= handler;
        }

        /// <summary>
        /// Executes all <see cref="CustomEventHandler"/> listeners safely.
        /// </summary>
        public void InvokeSafely()
        {
            InvokeNormal();
            InvokeAsync();
        }

        /// <inheritdoc cref="InvokeSafely"/>
        internal void InvokeNormal()
        {
            if (InnerEvent is null)
                return;

            foreach (CustomEventHandler handler in InnerEvent.GetInvocationList().Cast<CustomEventHandler>())
            {
                try
                {
                    handler();
                }
                catch (Exception ex)
                {
                    EventExceptionLogger.CaptureException(ex, GetType().FullName, handler.Method);
                }
            }
        }

        /// <inheritdoc cref="InvokeSafely"/>
        internal void InvokeAsync()
        {
            if (InnerAsyncEvent is null)
                return;

            foreach (CustomAsyncEventHandler handler in InnerAsyncEvent.GetInvocationList().Cast<CustomAsyncEventHandler>())
            {
                Timing.RunCoroutine(SafeCoroutineEnumerator(handler(), handler));
            }
        }

        /// <summary>
        /// Runs the coroutine manually so exceptions can be caught and logged.
        /// </summary>
        private IEnumerator<float> SafeCoroutineEnumerator(IEnumerator<float> coroutine, CustomAsyncEventHandler handler)
        {
            while (true)
            {
                float current;
                try
                {
                    if (!coroutine.MoveNext())
                        break;
                    current = coroutine.Current;
                }
                catch (Exception ex)
                {
                    Log.Error($"Method \"{handler.Method.Name}\" of the class \"{handler.Method.ReflectedType.FullName}\" caused an exception when handling the event \"{GetType().FullName}\"\n{ex}");
                    yield break;
                }

                yield return current;
            }
        }
    }
}
