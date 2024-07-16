// -----------------------------------------------------------------------
// <copyright file="FeatureGroup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Extra
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Events;
    using Exiled.API.Features;

    /// <summary>
    /// A feature group for a plugin.
    /// </summary>
    public class FeatureGroup
    {
        /// <summary>
        /// A dictionary containing all feature groups identified by a key.
        /// </summary>
        public static readonly Dictionary<string, FeatureGroup> Features = new();

        /// <summary>
        /// The key for this feature group.
        /// </summary>
#pragma warning disable SA1401
        public readonly string Key;
#pragma warning restore SA1401

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureGroup"/> class.
        /// Create a new feature group and add it to the dictionary.
        /// </summary>
        /// <param name="key">The key the feature group should be identified by.</param>
        public FeatureGroup(string key)
        {
            Key = key;
            Features.Add(Key, this);
        }

        /// <summary>
        /// Gets the EventGroup handling registration.
        /// </summary>
        public EventGroup EventGroup { get; } = new();

        /// <summary>
        /// Gets a list of events pertaining to the EventGroup.
        /// </summary>
        public List<object> Events { get; private set; } = new();

        /// <summary>
        /// Gets a list of MEC coroutines added to this FeatureGroup.
        /// </summary>
        public List<Func<IEnumerator<float>>> Coroutines { get; } = new();

        /// <summary>
        /// Gets a value indicating whether the features in this group are registered or not.
        /// </summary>
        public bool IsRegistered { get; private set; }

        /// <summary>
        /// Register all supplied features.
        /// </summary>
        public void Register()
        {
            Log.Info("Registering Feature Group: " + Key);
            foreach (object @event in Events)
                EventGroup.AddEventHandlers(@event);
            foreach (Func<IEnumerator<float>> coroutine in Coroutines)
                CoroutineManager.StartCoroutine(coroutine, Key + coroutine.Method.Name);

            IsRegistered = true;
        }

        /// <summary>
        /// Unregisters all supplied features.
        /// </summary>
        public void Unregister()
        {
            Log.Info("Unregistering Feature Group: " + Key);
            EventGroup.RemoveEvents();
            foreach (Func<IEnumerator<float>> coroutine in Coroutines)
                CoroutineManager.StopCoroutine(Key + coroutine.Method.Name);

            IsRegistered = false;
        }

        /// <summary>
        /// Supplies a list of events to this feature group.
        /// </summary>
        /// <param name="classes">A params list that contains all event classes wanting to be added.</param>
        /// <returns>Itself.</returns>
        public FeatureGroup Supply(params object[] classes)
        {
            Events = Events.Concat(classes).ToList();
            return this;
        }

        /// <summary>
        /// Supplies a coroutine to this feature group.
        /// </summary>
        /// <param name="function">The coroutine that should be added.</param>
        /// <returns>Itself.</returns>
        public FeatureGroup Supply(Func<IEnumerator<float>> function)
        {
            Coroutines.Add(function);
            return this;
        }

        /// <summary>
        /// Supplies coroutines to this feature group.
        /// </summary>
        /// <param name="functions">The coroutines that should be added.</param>
        /// <returns>Itself.</returns>
        public FeatureGroup Supply(params Func<IEnumerator<float>>[] functions)
        {
            foreach (Func<IEnumerator<float>> function in functions)
                Coroutines.Add(function);
            return this;
        }
    }
}