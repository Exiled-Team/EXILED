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
    using Exiled.Events.Interfaces;
    using Exiled.Loader;

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
        public static HashSet<Type> UnpatchedPatches { get; private set; } = GetAllPatches();

        /// <summary>
        /// Gets a set of types and methods for which EXILED patches should not be run.
        /// </summary>
        public static HashSet<MethodBase> DisabledPatchesHashSet { get; } = new();

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
                foreach (Type type in UnpatchedPatches)
                {
                    if (type.GetCustomAttributes<EventPatchAttribute>().Any((epa) => epa.Event == @event))
                    {
                        new PatchClassProcessor(Harmony, type).Patch();
                        toRemove.Add(type);
                    }
                }

                UnpatchedPatches.RemoveWhere((type) => toRemove.Contains(type));
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
                foreach (Type type in UnpatchedPatches)
                {
                    if (!includeEvents && type.GetCustomAttributes<EventPatchAttribute>().Any())
                        continue;

                    new PatchClassProcessor(Harmony, type).Patch();
                    toRemove.Add(type);
                }

                UnpatchedPatches.RemoveWhere((type) => toRemove.Contains(type));

                Log.Debug("Events patched by attributes successfully!", Loader.ShouldDebugBeShown);
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
        /// Checks the <see cref="DisabledPatchesHashSet"/> list and un-patches any methods that have been defined there. Once un-patching has been done, they can be patched by plugins, but will not be re-patchable by Exiled until a server reboot.
        /// </summary>
        public void ReloadDisabledPatches()
        {
            foreach (MethodBase method in DisabledPatchesHashSet)
            {
                Harmony.Unpatch(method, HarmonyPatchType.All, Harmony.Id);

                Log.Info($"Unpatched {method.Name}");
            }
        }

        /// <summary>
        /// Unpatches all events.
        /// </summary>
        public void UnpatchAll()
        {
            Log.Debug("Unpatching events...", Loader.ShouldDebugBeShown);
            Harmony.UnpatchAll();
            UnpatchedPatches = GetAllPatches();

            Log.Debug("All events have been unpatched. Goodbye!", Loader.ShouldDebugBeShown);
        }

        private static HashSet<Type> GetAllPatches() => Assembly.GetExecutingAssembly().GetTypes().Where((type) => type.CustomAttributes.Any((customAtt) => customAtt.AttributeType == typeof(HarmonyPatch))).ToHashSet();
    }
}
