// -----------------------------------------------------------------------
// <copyright file="DynamicEventDispatcher.cs" company="Exiled Team">
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
    /// The class which handles delegates dynamically acting as multicast listener.
    /// </summary>
    public class DynamicEventDispatcher : TypeCastObject<DynamicEventDispatcher>, IDynamicEventDispatcher
    {
        private readonly Dictionary<object, List<Action>> boundDelegates = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicEventDispatcher"/> class.
        /// </summary>
        public DynamicEventDispatcher()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicEventDispatcher"/> class.
        /// </summary>
        /// <param name="delegates">The delegates to be bound.</param>
        public DynamicEventDispatcher(Dictionary<object, List<Action>> delegates) => boundDelegates = delegates;

        /// <summary>
        /// Gets all the bound delegates.
        /// </summary>
        public IReadOnlyDictionary<object, List<Action>> BoundDelegates => boundDelegates;

        /// <summary>
        /// This indexer allows access to bound listeners using an <see cref="object"/> reference.
        /// </summary>
        /// <param name="object">The listener to look for.</param>
        /// <returns>The obund listener corresponding to the specified reference.</returns>
        public KeyValuePair<object, List<Action>> this[object @object] => boundDelegates.FirstOrDefault(kvp => kvp.Key == @object);

        /// <summary>
        /// Binds a delegate the event dispatcher.
        /// </summary>
        /// <param name="left">The <see cref="DynamicEventDispatcher"/> to bind the listener to.</param>
        /// <param name="right">The delegate to bind.</param>
        /// <returns>The left-hand <see cref="DynamicEventDispatcher"/> operator.</returns>
        public static DynamicEventDispatcher operator +(DynamicEventDispatcher left, Action right)
        {
            left.Bind(right.Target, right);
            return left;
        }

        /// <summary>
        /// Binds a listener from the event dispatcher.
        /// </summary>
        /// <param name="left">The <see cref="DynamicEventDispatcher"/> to bind the listener from.</param>
        /// <param name="right">The <see cref="TDynamicDelegate{T}"/> containing the listener to bind.</param>
        /// <returns>The left-hand <see cref="DynamicEventDispatcher"/> operator.</returns>
        public static DynamicEventDispatcher operator +(DynamicEventDispatcher left, DynamicDelegate right)
        {
            left.Bind(right.Target, right.Delegate);
            return left;
        }

        /// <summary>
        /// Binds all bound listeners to a <see cref="DynamicEventDispatcher"/> to the event dispatcher.
        /// </summary>
        /// <param name="left">The <see cref="DynamicEventDispatcher"/> to bind the listeners from.</param>
        /// <param name="right">The <see cref="DynamicEventDispatcher"/> containing the listeners to bind.</param>
        /// <returns>The left-hand <see cref="DynamicEventDispatcher"/> operator.</returns>
        public static DynamicEventDispatcher operator +(DynamicEventDispatcher left, DynamicEventDispatcher right)
        {
            foreach ((KeyValuePair<object, List<Action>> kvp, Action action) in right.BoundDelegates
                .SelectMany(kvp => kvp.Value.Select(action => (kvp, action))))
                left.Bind(kvp.Key, action);

            return left;
        }

        /// <summary>
        /// Unbinds a delegate the event dispatcher.
        /// </summary>
        /// <param name="left">The <see cref="DynamicEventDispatcher"/> to unbind the listener from.</param>
        /// <param name="right">The delegate to bind.</param>
        /// <returns>The left-hand <see cref="DynamicEventDispatcher"/> operator.</returns>
        public static DynamicEventDispatcher operator -(DynamicEventDispatcher left, Action right)
        {
            left.Unbind(right.Target);
            return left;
        }

        /// <summary>
        /// Unbinds a listener from the event dispatcher.
        /// </summary>
        /// <param name="left">The <see cref="DynamicEventDispatcher"/> to unbind the listener from.</param>
        /// <param name="right">The <see cref="TDynamicDelegate{T}"/> containing the listener to unbind.</param>
        /// <returns>The left-hand <see cref="DynamicEventDispatcher"/> operator.</returns>
        public static DynamicEventDispatcher operator -(DynamicEventDispatcher left, DynamicDelegate right)
        {
            left.Unbind(right.Target);
            return left;
        }

        /// <summary>
        /// Unbinds all bound listeners to a <see cref="DynamicEventDispatcher"/> from the event dispatcher.
        /// </summary>
        /// <param name="left">The <see cref="DynamicEventDispatcher"/> to unbind the listeners from.</param>
        /// <param name="right">The <see cref="DynamicEventDispatcher"/> containing the listeners to unbind.</param>
        /// <returns>The left-hand <see cref="DynamicEventDispatcher"/> operator.</returns>
        public static DynamicEventDispatcher operator -(DynamicEventDispatcher left, DynamicEventDispatcher right)
        {
            foreach ((KeyValuePair<object, List<Action>> kvp, Action action) in right.BoundDelegates
                .SelectMany(kvp => kvp.Value.Select(action => (kvp, action))))
                left.Unbind(kvp.Key);

            return left;
        }

        /// <summary>
        /// Binds a listener to the event dispatcher.
        /// </summary>
        /// <param name="obj">The listener instance.</param>
        /// <param name="del">The delegate to be bound.</param>
        public virtual void Bind(object obj, Action del)
        {
            if (!boundDelegates.ContainsKey(obj))
                boundDelegates.Add(obj, new List<Action>() { del });
            else
                boundDelegates[obj].Add(del);
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
        public virtual void Unbind(object obj, Action del)
        {
            if (!boundDelegates.ContainsKey(obj))
                return;

            boundDelegates[obj].Remove(del);
        }

        /// <summary>
        /// Invokes the delegates from the specified listener.
        /// </summary>
        /// <param name="obj">The listener instance.</param>
        public virtual void Invoke(object obj)
        {
            if (boundDelegates.TryGetValue(obj, out List<Action> delegates))
                delegates.ForEach(del => del());
        }

        /// <summary>
        /// Invokes all the delegates from all the bound delegates.
        /// </summary>
        public virtual void InvokeAll()
        {
            foreach (KeyValuePair<object, List<Action>> kvp in boundDelegates)
                kvp.Value.ForEach(del => del());
        }

        /// <inheritdoc/>
        public virtual void UnbindAll() => boundDelegates.Clear();
    }
}