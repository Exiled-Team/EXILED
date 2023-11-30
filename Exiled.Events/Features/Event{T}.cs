// -----------------------------------------------------------------------
// <copyright file="Event{T}.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;
    using MEC;

    /// <summary>
    /// The custom <see cref="EventHandler"/> delegate.
    /// </summary>
    /// <typeparam name="TEventArgs">The <see cref="EventHandler{TEventArgs}"/> type.</typeparam>
    /// <param name="ev">The <see cref="EventHandler{TEventArgs}"/> instance.</param>
    public delegate void CustomEventHandler<TEventArgs>(TEventArgs ev);

    /// <summary>
    /// The custom <see cref="EventHandler"/> delegate.
    /// </summary>
    /// <typeparam name="TEventArgs">The <see cref="EventHandler{TEventArgs}"/> type.</typeparam>
    /// <param name="ev">The <see cref="EventHandler{TEventArgs}"/> instance.</param>
    /// <returns><see cref="IEnumerator{T}"/> of <see cref="float"/>.</returns>
    public delegate IEnumerator<float> CustomAsyncEventHandler<TEventArgs>(TEventArgs ev);

    /// <summary>
    /// An implementation of the <see cref="IExiledEvent"/> interface that encapsulates an event with arguments.
    /// </summary>
    /// <typeparam name="T">The specified <see cref="EventArgs"/> that the event will use.</typeparam>
    public class Event<T> : IExiledEvent
    {
        private static readonly Dictionary<Type, Event<T>> TypeToEvent = new();

        private bool patched;

        /// <summary>
        /// Initializes a new instance of the <see cref="Event{T}"/> class.
        /// </summary>
        public Event()
        {
            TypeToEvent.Add(typeof(T), this);
        }

        private event CustomEventHandler<T> InnerEvent;

        private event CustomAsyncEventHandler<T> InnerAsyncEvent;

        /// <summary>
        /// Gets a <see cref="IReadOnlyCollection{T}"/> of <see cref="Event{T}"/> which contains all the <see cref="Event{T}"/> instances.
        /// </summary>
        public static IReadOnlyDictionary<Type, Event<T>> Dictionary => TypeToEvent;

        /// <summary>
        /// Subscribes a target <see cref="CustomEventHandler{TEventArgs}"/> to the inner event and checks if patching is possible, if dynamic patching is enabled.
        /// </summary>
        /// <param name="event">The <see cref="Event{T}"/> the <see cref="CustomEventHandler{T}"/> will be subscribed to.</param>
        /// <param name="handler">The <see cref="CustomEventHandler{T}"/> that will be subscribed to the <see cref="Event{T}"/>.</param>
        /// <returns>The <see cref="Event{T}"/> with the handler subscribed to it.</returns>
        public static Event<T> operator +(Event<T> @event, CustomEventHandler<T> handler)
        {
            @event.Subscribe(handler);
            return @event;
        }

        /// <summary>
        /// Subscribes a <see cref="CustomAsyncEventHandler"/> to the inner event, and checks patches if dynamic patching is enabled.
        /// </summary>
        /// <param name="event">The <see cref="Event{T}"/> to subscribe the <see cref="CustomAsyncEventHandler{T}"/> to.</param>
        /// <param name="asyncEventHandler">The <see cref="CustomAsyncEventHandler{T}"/> to subscribe to the <see cref="Event{T}"/>.</param>
        /// <returns>The <see cref="Event{T}"/> with the handler added to it.</returns>
        public static Event<T> operator +(Event<T> @event, CustomAsyncEventHandler<T> asyncEventHandler)
        {
            @event.Subscribe(asyncEventHandler);
            return @event;
        }

        /// <summary>
        /// Unsubscribes a target <see cref="CustomEventHandler{TEventArgs}"/> from the inner event and checks if unpatching is possible, if dynamic patching is enabled.
        /// </summary>
        /// <param name="event">The <see cref="Event{T}"/> the <see cref="CustomEventHandler{T}"/> will be unsubscribed from.</param>
        /// <param name="handler">The <see cref="CustomEventHandler{T}"/> that will be unsubscribed from the <see cref="Event{T}"/>.</param>
        /// <returns>The <see cref="Event{T}"/> with the handler unsubscribed from it.</returns>
        public static Event<T> operator -(Event<T> @event, CustomEventHandler<T> handler)
        {
            @event.Unsubscribe(handler);
            return @event;
        }

        /// <summary>
        /// Unsubscribes a target <see cref="CustomAsyncEventHandler{TEventArgs}"/> from the inner event, and checks if unpatching is possible, if dynamic patching is enabled.
        /// </summary>
        /// <param name="event">The <see cref="Event"/> the <see cref="CustomAsyncEventHandler{T}"/> will be unsubscribed from.</param>
        /// <param name="asyncEventHandler">The <see cref="CustomAsyncEventHandler{T}"/> that will be unsubscribed from the <see cref="Event{T}"/>.</param>
        /// <returns>The <see cref="Event{T}"/> with the handler unsubscribed from it.</returns>
        public static Event<T> operator -(Event<T> @event, CustomAsyncEventHandler<T> asyncEventHandler)
        {
            @event.Unsubscribe(asyncEventHandler);
            return @event;
        }

        /// <summary>
        /// Subscribes a target <see cref="CustomEventHandler{T}"/> to the inner event if the conditional is true.
        /// </summary>
        /// <param name="handler">The handler to add.</param>
        public void Subscribe(CustomEventHandler<T> handler)
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
        /// Subscribes a target <see cref="CustomAsyncEventHandler{T}"/> to the inner event if the conditional is true.
        /// </summary>
        /// <param name="handler">The handler to add.</param>
        public void Subscribe(CustomAsyncEventHandler<T> handler)
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
        /// Unsubscribes a target <see cref="CustomEventHandler{T}"/> from the inner event if the conditional is true.
        /// </summary>
        /// <param name="handler">The handler to add.</param>
        public void Unsubscribe(CustomEventHandler<T> handler)
        {
            InnerEvent -= handler;
        }

        /// <summary>
        /// Unsubscribes a target <see cref="CustomEventHandler{T}"/> from the inner event if the conditional is true.
        /// </summary>
        /// <param name="handler">The handler to add.</param>
        public void Unsubscribe(CustomAsyncEventHandler<T> handler)
        {
            InnerAsyncEvent -= handler;
        }

        /// <summary>
        /// Executes all <see cref="CustomEventHandler{TEventArgs}"/> listeners safely.
        /// </summary>
        /// <param name="arg">The event argument.</param>
        /// <exception cref="ArgumentNullException">Event or its arg is <see langword="null"/>.</exception>
        public void InvokeSafely(T arg)
        {
            InvokeNormal(arg);
            InvokeAsync(arg);
        }

        internal void InvokeNormal(T arg)
        {
            if (InnerEvent is null)
                return;

            foreach (CustomEventHandler<T> handler in InnerEvent.GetInvocationList().Cast<CustomEventHandler<T>>())
            {
                try
                {
                    handler(arg);
                }
                catch (Exception ex)
                {
                    Log.Error($"Method \"{handler.Method.Name}\" of the class \"{handler.Method.ReflectedType.FullName}\" caused an exception when handling the event \"{GetType().FullName}\"\n{ex}");
                }
            }
        }

        internal void InvokeAsync(T arg)
        {
            if (InnerAsyncEvent is null)
                return;

            foreach (CustomAsyncEventHandler<T> handler in InnerAsyncEvent.GetInvocationList().Cast<CustomAsyncEventHandler<T>>())
            {
                try
                {
                    Timing.RunCoroutine(handler(arg));
                }
                catch (Exception ex)
                {
                    Log.Error($"Method \"{handler.Method.Name}\" of the class \"{handler.Method.ReflectedType.FullName}\" caused an exception when handling the event \"{GetType().FullName}\"\n{ex}");
                }
            }
        }
    }
}
