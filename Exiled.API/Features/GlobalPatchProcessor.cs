// -----------------------------------------------------------------------
// <copyright file="GlobalPatchProcessor.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    using Attributes;

    using HarmonyLib;

    /// <summary>
    /// A <see cref="Harmony"/> alternative detour tool which adds more ways to manage patches and external assemblies.
    /// </summary>
    public class GlobalPatchProcessor
    {
        private static readonly Dictionary<MethodBase, HashSet<string>> PatchedGroupMethodsValue = new();

        /// <summary>
        /// Gets all the patched methods.
        /// </summary>
        public static IEnumerable<MethodBase> PatchedMethods => Harmony.GetAllPatchedMethods();

        /// <summary>
        /// Gets all the patched methods and their relative patch group.
        /// </summary>
        public static IReadOnlyDictionary<MethodBase, HashSet<string>> PatchedGroupMethods => PatchedGroupMethodsValue;

        /// <summary>
        /// Patch all your Harmony and return you the number of failed patch.
        /// </summary>
        /// <param name="harmony">The Harmony instance to Patch.</param>
        /// <param name="failedPatch">The number of failed patch.</param>
        public static void PatchAll(Harmony harmony, out int failedPatch)
        {
            failedPatch = 0;
            foreach (Type type in AccessTools.GetTypesFromAssembly(Assembly.GetCallingAssembly()))
            {
                try
                {
                    harmony.CreateClassProcessor(type).Patch();
                }
                catch (HarmonyException exception)
                {
                    Log.Error($"Patching by attributes failed!\n{exception}");

                    failedPatch++;
                    continue;
                }
            }

#if DEBUG
            Log.DebugWithContext("Events patched by attributes successfully!");
#endif
        }

        /// <summary>
        /// Searches the current assembly for Harmony annotations and uses them to create patches.
        /// <br>It supports target-patching using <see cref="PatchGroupAttribute"/> and the relative <paramref name="groupId"/>.</br>
        /// </summary>
        /// <param name="id">The Harmony instance id.</param>
        /// <param name="groupId">The target group to include.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="PatchGroupAttribute.GroupId"/> is <see langword="null"/> or empty.</exception>
        /// <returns>The <see cref="Harmony"/> instance.</returns>
        public static Harmony PatchAll(string id = "", string groupId = null)
        {
            try
            {
                Harmony harmony = new(id);
                bool isPatchGroup = false;
                foreach (Type type in Assembly.GetCallingAssembly().GetTypes())
                {
                    PatchClassProcessor processor = harmony.CreateClassProcessor(type);
                    PatchGroupAttribute patchGroup = type.GetCustomAttribute<PatchGroupAttribute>();

                    if (patchGroup is null)
                    {
                        processor.Patch();
                        continue;
                    }

                    if (string.IsNullOrEmpty(patchGroup.GroupId))
                        throw new ArgumentNullException(nameof(groupId));

                    if (string.IsNullOrEmpty(groupId) || patchGroup.GroupId != groupId)
                        continue;

                    isPatchGroup = true;
                    processor.Patch();
                }

                if (!isPatchGroup)
                    return harmony;

                foreach (MethodBase methodBase in harmony.GetPatchedMethods())
                {
                    if (PatchedGroupMethods.TryGetValue(methodBase, out HashSet<string> ids))
                        ids.Add(groupId);
                    else
                        PatchedGroupMethodsValue.Add(methodBase, new HashSet<string> { groupId });

#if DEBUG
                    Log.Debug($"Target method ({methodBase.Name}) has been successfully patched.");
#endif
                }

#if DEBUG
                Log.DebugWithContext($"Patching completed.");
#endif
                return harmony;
            }
            catch (Exception ex)
            {
                Log.ErrorWithContext($"Patching failed!\n" + ex);
            }

            return null;
        }

        /// <summary>
        /// Unpatches methods by patching them with zero patches.
        /// </summary>
        /// <param name="id">The Harmony instance id.</param>
        /// <param name="groupId">The target group to include.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="PatchGroupAttribute.GroupId"/> is <see langword="null"/> or empty.</exception>
        public static void UnpatchAll(string id = "", string groupId = null)
        {
            Harmony harmony = new(id);
            foreach (MethodBase methodBase in Harmony.GetAllPatchedMethods().ToList())
            {
                harmony.CreateProcessor(methodBase);

                Patches patchInfo = Harmony.GetPatchInfo(methodBase);
                if (!patchInfo.Owners.Contains(id))
                    continue;

                PatchGroupAttribute patchGroup = methodBase.GetCustomAttribute<PatchGroupAttribute>();
                if (patchGroup is null)
                    goto Unpatch;

                if (string.IsNullOrEmpty(patchGroup.GroupId))
                    throw new ArgumentNullException(nameof(groupId));

                if (string.IsNullOrEmpty(groupId) || patchGroup.GroupId != groupId)
                    continue;

                Unpatch:
                bool hasMethodBody = methodBase.HasMethodBody();
                if (hasMethodBody)
                {
                    patchInfo.Postfixes.Do(
                        delegate(Patch patchInfo)
                        {
                            harmony.Unpatch(methodBase, patchInfo.PatchMethod);
                        });
                    patchInfo.Prefixes.Do(
                        delegate(Patch patchInfo)
                        {
                            harmony.Unpatch(methodBase, patchInfo.PatchMethod);
                        });
                }

                patchInfo.Transpilers.Do(
                    delegate(Patch patchInfo)
                    {
                        harmony.Unpatch(methodBase, patchInfo.PatchMethod);
                    });

                if (hasMethodBody)
                {
                    patchInfo.Finalizers.Do(
                        delegate(Patch patchInfo)
                        {
                            harmony.Unpatch(methodBase, patchInfo.PatchMethod);
                        });
                }

                PatchedGroupMethodsValue.Remove(methodBase);
            }
        }
    }
}
