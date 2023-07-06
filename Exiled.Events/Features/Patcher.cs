// -----------------------------------------------------------------------
// <copyright file="Patcher.cs" company="Exiled Team">
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
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Interfaces;
    using Exiled.Events.Interfaces;
    using HarmonyLib;

    /// <summary>
    /// A tool for patching.
    /// </summary>
    internal class Patcher
    {
        /// <summary>
        /// The below variable is used to increment the name of the harmony instance, otherwise harmony will not work upon a plugin reload.
        /// </summary>
        private static int patchesCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="Patcher"/> class.
        /// </summary>
        internal Patcher()
        {
            Harmony = new($"exiled.events.{++patchesCounter}");
        }

        /// <summary>
        /// Gets a <see cref="HashSet{T}"/> that contains all patch types that haven't been patched.
        /// </summary>
        public static HashSet<Type> UnpatchedTypes { get; private set; } = GetAllPatchTypes();

        /// <summary>
        /// Gets the <see cref="HarmonyLib.Harmony"/> instance.
        /// </summary>
        public Harmony Harmony { get; }

        /// <summary>
        /// Patches all events that target a specific <see cref="IEvent"/>.
        /// </summary>
        /// <param name="event">The <see cref="IEvent"/> all matching patches should target.</param>
        public void Patch(IEvent @event)
        {
            try
            {
                List<Type> toRemove = new();
                foreach (Type type in UnpatchedTypes)
                {
                    if (type.GetCustomAttributes<EventPatchAttribute>().Any((epa) => epa.Event == @event))
                    {
                        new PatchClassProcessor(Harmony, type).Patch();
                        toRemove.Add(type);
                    }
                }

                UnpatchedTypes.RemoveWhere((type) => toRemove.Contains(type));
            }
            catch (Exception ex)
            {
                Log.Error($"Patching by event failed!\n{ex}");
            }
        }

        /// <summary>
        /// Patches all events.
        /// </summary>
        /// <param name="includeEvents">Whether to patch events as well as all required patches.</param>
        public void PatchAll(bool includeEvents)
        {
            try
            {
#if DEBUG
                bool lastDebugStatus = Harmony.DEBUG;
                Harmony.DEBUG = true;
#endif
                List<Type> toRemove = new();
                IEnumerable<Type> toPatch = includeEvents ? UnpatchedTypes : UnpatchedTypes.Where((type) => !type.GetCustomAttributes<EventPatchAttribute>().Any());
                foreach (Type patch in toPatch)
                {
                    new PatchClassProcessor(Harmony, patch).Patch();
                    toRemove.Add(patch);
                }

                UnpatchedTypes.RemoveWhere((type) => toRemove.Contains(type));

                Log.Debug("Events patched by attributes successfully!");
#if DEBUG
                Harmony.DEBUG = lastDebugStatus;
#endif
            }
            catch (Exception exception)
            {
                Log.Error($"Patching by attributes failed!\n{exception}");
            }
        }

        /// <summary>
        /// Unpatches all events.
        /// </summary>
        public void UnpatchAll()
        {
            Log.Debug("Unpatching events...");
            Harmony.UnpatchAll(Harmony.Id);
            UnpatchedTypes = GetAllPatchTypes();

            Log.Debug("All events have been unpatched. Goodbye!");
        }

        /// <summary>
        /// Gets all types that have a <see cref="HarmonyPatch"/> attributed to them.
        /// </summary>
        /// <returns>A <see cref="HashSet{T}"/> of all patch types.</returns>
        internal static HashSet<Type> GetAllPatchTypes() => Assembly.GetExecutingAssembly().GetTypes().Where((type) => type.CustomAttributes.Any((customAtt) => customAtt.AttributeType == typeof(HarmonyPatch))).ToHashSet();
    }
}
