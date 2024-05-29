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
    }
}