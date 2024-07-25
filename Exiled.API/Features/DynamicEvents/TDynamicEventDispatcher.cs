// -----------------------------------------------------------------------
// <copyright file="TDynamicEventDispatcher.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.DynamicEvents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features.Core;

    /// <summary>
    /// The <see cref="DynamicEventDispatcher"/>'s generic version which accepts a type parameter.
    /// </summary>
    /// <typeparam name="T">The event type parameter.</typeparam>
    public class TDynamicEventDispatcher<T> : TypeCastObject<DynamicEventDispatcher>, IDynamicEventDispatcher
    {
        private readonly Dictionary<object, List<Action<T>>> boundDelegates = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="TDynamicEventDispatcher{T}"/> class.
        /// </summary>
        public TDynamicEventDispatcher()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TDynamicEventDispatcher{T}"/> class.
        /// </summary>
        /// <param name="delegates">The delegates to be bound.</param>
        public TDynamicEventDispatcher(Dictionary<object, List<Action<T>>> delegates) => boundDelegates = delegates;

        /// <summary>
        /// Gets all the bound delegates.
        /// </summary>
        public IReadOnlyDictionary<object, List<Action<T>>> BoundDelegates => boundDelegates;

        /// <summary>
        /// This indexer allows access to bound listeners using an <see cref="object"/> reference.
        /// </summary>
        /// <param name="object">The listener to look for.</param>
        /// <returns>The bound listener corresponding to the specified reference.</returns>
        public KeyValuePair<object, List<Action<T>>> this[object @object] => boundDelegates.FirstOrDefault(kvp => kvp.Key == @object);

        /// <summary>
        /// Binds a delegate the event dispatcher.
        /// </summary>
        /// <param name="left">The <see cref="TDynamicEventDispatcher{T}"/> to bind the listener to.</param>
        /// <param name="right">The delegate to bind.</param>
        /// <returns>The left-hand <see cref="TDynamicEventDispatcher{T}"/> operator.</returns>
        public static TDynamicEventDispatcher<T> operator +(TDynamicEventDispatcher<T> left, Action<T> right)
        {
            left.Bind(right.Target, right);
            return left;
        }

        /// <summary>
        /// Binds a listener to the event dispatcher.
        /// </summary>
        /// <param name="left">The <see cref="TDynamicEventDispatcher{T}"/> to bind the listener to.</param>
        /// <param name="right">The <see cref="TDynamicDelegate{T}"/> containing the listener to bind.</param>
        /// <returns>The left-hand <see cref="TDynamicEventDispatcher{T}"/> operator.</returns>
        public static TDynamicEventDispatcher<T> operator +(TDynamicEventDispatcher<T> left, TDynamicDelegate<T> right)
        {
            left.Bind(right.Target, right.Delegate);
            return left;
        }

        /// <summary>
        /// Binds all bound listeners to a <see cref="TDynamicEventDispatcher{T}"/> to the event dispatcher.
        /// </summary>
        /// <param name="left">The <see cref="TDynamicEventDispatcher{T}"/> to bind the listeners to.</param>
        /// <param name="right">The <see cref="TDynamicEventDispatcher{T}"/> containing the listeners to bind.</param>
        /// <returns>The left-hand <see cref="TDynamicEventDispatcher{T}"/> operator.</returns>
        public static TDynamicEventDispatcher<T> operator +(TDynamicEventDispatcher<T> left, TDynamicEventDispatcher<T> right)
        {
            foreach ((KeyValuePair<object, List<Action<T>>> kvp, Action<T> action) in right.BoundDelegates
                .SelectMany(kvp => kvp.Value.Select(action => (kvp, action))))
                left.Bind(kvp.Key, action);

            return left;
        }

        /// <summary>
        /// Unbinds a delegate the event dispatcher.
        /// </summary>
        /// <param name="left">The <see cref="TDynamicEventDispatcher{T}"/> to unbind the listener from.</param>
        /// <param name="right">The delegate to unbind.</param>
        /// <returns>The left-hand <see cref="TDynamicEventDispatcher{T}"/> operator.</returns>
        public static TDynamicEventDispatcher<T> operator -(TDynamicEventDispatcher<T> left, Action<T> right)
        {
            left.Unbind(right.Target);
            return left;
        }

        /// <summary>
        /// Unbinds a delegate from the event dispatcher.
        /// </summary>
        /// <param name="left">The <see cref="TDynamicEventDispatcher{T}"/> to unbind the delegate from.</param>
        /// <param name="right">The <see cref="TDynamicDelegate{T}"/> containing the delegate to unbind.</param>
        /// <returns>The left-hand <see cref="TDynamicEventDispatcher{T}"/> operator.</returns>
        public static TDynamicEventDispatcher<T> operator -(TDynamicEventDispatcher<T> left, TDynamicDelegate<T> right)
        {
            left.Unbind(right.Target);
            return left;
        }

        /// <summary>
        /// Unbinds all bound listeners to a <see cref="TDynamicEventDispatcher{T}"/> from the event dispatcher.
        /// </summary>
        /// <param name="left">The <see cref="TDynamicEventDispatcher{T}"/> to unbind the listeners from.</param>
        /// <param name="right">The <see cref="TDynamicEventDispatcher{T}"/> containing the listeners to unbind.</param>
        /// <returns>The left-hand <see cref="TDynamicEventDispatcher{T}"/> operator.</returns>
        public static TDynamicEventDispatcher<T> operator -(TDynamicEventDispatcher<T> left, TDynamicEventDispatcher<T> right)
        {
            foreach ((KeyValuePair<object, List<Action<T>>> kvp, Action<T> _) in right.BoundDelegates
                .SelectMany(kvp => kvp.Value.Select(action => (kvp, action))))
                left.Unbind(kvp.Key);

            return left;
        }

        /// <summary>
        /// Binds a listener to the event dispatcher.
        /// </summary>
        /// <param name="obj">The listener instance.</param>
        /// <param name="del">The delegate to be bound.</param>
        public virtual void Bind(object obj, Action<T> del)
        {
            if (obj is null)
            {
                Log.Warn($"Couldn't subscribe to the delegate {del.Method.Name} due to null instance.");
                return;
            }

            if (!boundDelegates.TryGetValue(obj, out List<Action<T>> @delegate))
                boundDelegates.Add(obj, new List<Action<T>> { del });
            else
                @delegate.Add(del);
        }

        /// <summary>
        /// Unbinds a listener from the event dispatcher.
        /// </summary>
        /// <param name="obj">The listener instance.</param>
        public virtual void Unbind(object obj) => boundDelegates.Remove(obj);

        /// <summary>
        /// Unbinds a listener from the event dispatcher.
        /// </summary>
        /// <param name="obj">The listener instance.</param>
        /// <param name="del">The delegate to be unbound.</param>
        public virtual void Unbind(object obj, Action<T> del)
        {
            if (!boundDelegates.TryGetValue(obj, out List<Action<T>> @delegate))
                return;

            @delegate.Remove(del);
        }

        /// <summary>
        /// Invokes the delegates from the specified listener.
        /// </summary>
        /// <param name="obj">The listener instance.</param>
        /// <param name="instance">The .</param>
        public virtual void Invoke(object obj, T instance)
        {
            if (boundDelegates.TryGetValue(obj, out List<Action<T>> delegates))
                delegates.ForEach(del => del(instance));
        }

        /// <summary>
        /// Invokes all the delegates from all the bound delegates.
        /// </summary>
        /// <param name="instance">The parameter instance.</param>
        public virtual void InvokeAll(T instance)
        {
            foreach (KeyValuePair<object, List<Action<T>>> kvp in boundDelegates)
                kvp.Value.ForEach(del => del(instance));
        }

        /// <inheritdoc/>
        public virtual void UnbindAll() => boundDelegates.Clear();
    }
}