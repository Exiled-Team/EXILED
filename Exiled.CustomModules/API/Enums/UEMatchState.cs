// -----------------------------------------------------------------------
// <copyright file="UEMatchState.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Enums
{
    using Exiled.API.Features.Core.Generics;

    /// <summary>
    /// All available match states.
    /// </summary>
    public class UEMatchState : UnmanagedEnumClass<byte, UEMatchState>
    {
        /// <summary>
        /// States the match has no defined state.
        /// </summary>
        public static readonly UEMatchState None = new(0);

        /// <summary>
        /// States the match is paused.
        /// </summary>
        public static readonly UEMatchState Paused = new(1);

        /// <summary>
        /// States the match is in progress.
        /// </summary>
        public static readonly UEMatchState InProgress = new(2);

        /// <summary>
        /// States the match has terminated.
        /// </summary>
        public static readonly UEMatchState Terminated = new(2);

        /// <summary>
        /// Initializes a new instance of the <see cref="UEMatchState"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        protected UEMatchState(byte value)
            : base(value)
        {
        }
    }
}