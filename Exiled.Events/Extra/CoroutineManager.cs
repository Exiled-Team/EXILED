// -----------------------------------------------------------------------
// <copyright file="CoroutineManager.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Extra
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    using MEC;

    /// <summary>
    /// A simple coroutine manager wrapping MEC.
    /// </summary>
    public static class CoroutineManager
    {
        private static readonly Dictionary<string, CoroutineHandle> RunningCoroutines = new();

        /// <summary>
        /// Starts a single coroutine and keeps track of it.
        /// </summary>
        /// <param name="coroutine">The coroutine to start.</param>
        /// <param name="key">A unique key to identify the coroutine. Defaults to the name of the caller method.</param>
        public static void StartCoroutine(Func<IEnumerator<float>> coroutine, [CallerMemberName] string key = "")
        {
            if (RunningCoroutines.ContainsKey(key))
                StopCoroutine(key);
            CoroutineHandle handle = Timing.RunCoroutine(coroutine());
            RunningCoroutines[key] = handle;
        }

        /// <summary>
        /// Starts multiple coroutines and keeps track of them.
        /// </summary>
        /// <param name="coroutines">A list of tuples containing coroutines and their unique keys.</param>
        public static void StartCoroutines(params (Func<IEnumerator<float>> coroutine, string key)[] coroutines)
        {
            foreach ((Func<IEnumerator<float>> coroutine, string key) in coroutines) 
                StartCoroutine(coroutine, key);
        }

        /// <summary>
        /// Stops a running coroutine.
        /// </summary>
        /// <param name="key">The unique key of the coroutine to stop.</param>
        public static void StopCoroutine(string key)
        {
            if (!RunningCoroutines.TryGetValue(key, out CoroutineHandle handle))
                return;
            Timing.KillCoroutines(handle);
            RunningCoroutines.Remove(key);
        }

        /// <summary>
        /// Stops all running coroutines.
        /// </summary>
        public static void StopAllCoroutines()
        {
            foreach (CoroutineHandle handle in RunningCoroutines.Values)
                Timing.KillCoroutines(handle);

            RunningCoroutines.Clear();
        }

        /// <summary>
        /// Checks if a coroutine is running.
        /// </summary>
        /// <param name="key">The unique key of the coroutine to check.</param>
        /// <returns>True if the coroutine is running, otherwise false.</returns>
        public static bool IsCoroutineRunning(string key)
        {
            return RunningCoroutines.ContainsKey(key);
        }
    }
}
