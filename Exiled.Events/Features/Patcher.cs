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
        private static readonly Dictionary<Type, EventPatchAttribute> EventPatches;

        /// <summary>
        /// The below variable is used to increment the name of the harmony instance, otherwise harmony will not work upon a plugin reload.
        /// </summary>
        private static int patchesCounter;

        static Patcher()
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                EventPatchAttribute epa = type.GetCustomAttribute<EventPatchAttribute>();

                if (epa != null)
                    EventPatches.Add(type, epa);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Patcher"/> class.
        /// </summary>
        internal Patcher()
        {
            Harmony = new($"exiled.events.{++patchesCounter}");
        }

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
            Type currentPatch = null;
            try
            {
                foreach (KeyValuePair<Type, EventPatchAttribute> kvp in EventPatches)
                {
                    if (kvp.Value.Event == @event)
                    {
                        currentPatch = kvp.Key;
                        Patch(kvp.Key, kvp.Value.PatchedMethod);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Event patch for type {currentPatch} failed!\n{ex}");
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
                if (includeEvents)
                {
                    foreach (KeyValuePair<Type, EventPatchAttribute> eventPatch in EventPatches)
                    {
                        Patch(eventPatch.Key, eventPatch.Value.PatchedMethod);
                    }
                }

                if (PatchByAttributes())
                    Log.Debug("Patched successfully!", Loader.ShouldDebugBeShown);
                else
                    Log.Error($"Patching failed!");
#if DEBUG
                Harmony.DEBUG = lastDebugStatus;
#endif
            }
            catch (Exception exception)
            {
                Log.Error($"Patching failed!\n{exception}");
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

            Log.Debug("All events have been unpatched. Goodbye!", Loader.ShouldDebugBeShown);
        }

        private void Patch(Type type, MethodInfo method)
        {
            HarmonyMethod prefix = GetHarmonyMethod(type, "Prefix");
            HarmonyMethod postfix = GetHarmonyMethod(type, "Postfix");
            HarmonyMethod transpiler = GetHarmonyMethod(type, "Transpiler");

            Harmony.Patch(method, prefix, postfix, transpiler);
        }

        private HarmonyMethod GetHarmonyMethod(Type type, string name)
        {
            MethodInfo method = type.GetMethod(name);
            return method is null ? null : new HarmonyMethod(method);
        }

        private bool PatchByAttributes()
        {
            try
            {
                Harmony.PatchAll();

                Log.Debug("Events patched by attributes successfully!", Loader.ShouldDebugBeShown);
                return true;
            }
            catch (Exception exception)
            {
                Log.Error($"Patching by attributes failed!\n{exception}");
                return false;
            }
        }
    }
}
