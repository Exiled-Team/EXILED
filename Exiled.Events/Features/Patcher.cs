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
    public class Patcher
    {
        private static readonly IEnumerable<Type> EventPatches = Assembly.GetExecutingAssembly().GetTypes().Where((type) => type.CustomAttributes.Any((cad) => cad.AttributeType == typeof(EventPatchAttribute)));

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
            foreach (Type type in EventPatches)
            {
                if (type.GetCustomAttribute<EventPatchAttribute>().Event == @event)
                {
                    try
                    {
                        MethodBase originalMethod = type.GetCustomAttribute<HarmonyPatch>().info.method;

                        MethodInfo prefixBase = type.GetMethod("Prefix");
                        HarmonyMethod prefix = prefixBase is null ? null : new HarmonyMethod(prefixBase);

                        MethodInfo postfixBase = type.GetMethod("Postfix");
                        HarmonyMethod postfix = prefixBase is null ? null : new HarmonyMethod(prefixBase);

                        MethodInfo transpilerBase = type.GetMethod("Transpiler");
                        HarmonyMethod transpiler = prefixBase is null ? null : new HarmonyMethod(prefixBase);

                        MethodInfo patchedMethod = Harmony.Patch(originalMethod, prefix, postfix, transpiler);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Patch for type {type} failed!\n{ex}");
                    }
                }
            }
        }

        /// <summary>
        /// Patches all events.
        /// </summary>
        public void PatchAll()
        {
            try
            {
#if DEBUG
                bool lastDebugStatus = Harmony.DEBUG;
                Harmony.DEBUG = true;
#endif
                if (PatchByAttributes())
                {
                    Log.Debug("Events patched successfully!", Loader.ShouldDebugBeShown);
                }
                else
                {
                    Log.Error($"Patching failed!");
                }
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

            Log.Debug("All events have been unpatched complete. Goodbye!", Loader.ShouldDebugBeShown);
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
