// -----------------------------------------------------------------------
// <copyright file="CoroutineExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using MEC;

    /// <summary>
    /// Extensions focused at <see cref="CoroutineHandle"/>.
    /// </summary>
    public static class CoroutineExtensions
    {
        /// <summary>
        /// Kills a coroutine.
        /// </summary>
        /// <param name="coroutine">The coroutine to kill.</param>
        public static void Kill(this CoroutineHandle coroutine) => Timing.KillCoroutines(coroutine);

        /// <summary>
        /// Gets whether or not a coroutine is running.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <returns>A bool indicating whether or not the coroutine is running.</returns>
        public static bool IsRunning(this CoroutineHandle handle) => Timing.IsRunning(handle);

        /// <summary>
        /// Gets whether or not a coroutine is running or paused.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <returns>A bool indicating whether or not the coroutine is running or paused.</returns>
        public static bool IsRunningOrPaused(this CoroutineHandle handle) => Timing.IsRunning(handle) || Timing.IsAliveAndPaused(handle);

        /// <summary>
        /// Sets the tag of a coroutine.
        /// </summary>
        /// <param name="coroutine">The coroutine to set the tag of.</param>
        /// <param name="newTag">The new tag to set.</param>
        public static void SetTag(this CoroutineHandle coroutine, string newTag) => Timing.SetTag(coroutine, newTag);
    }
}