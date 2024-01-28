// -----------------------------------------------------------------------
// <copyright file="UUKeypressTriggerType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using Exiled.API.Features.Core.Generic;

#pragma warning disable SA1310 // Field names should not contain underscore

    /// <summary>
    /// The action type that should be triggered from a keypress trigger.
    /// </summary>
    public class UUKeypressTriggerType : UniqueUnmanagedEnumClass<uint, UUKeypressTriggerType>
    {
        /// <summary>
        /// No action.
        /// </summary>
        public static readonly UUKeypressTriggerType None = new();

        /// <summary>
        /// Declares a keypress trigger input.
        /// </summary>
        public static readonly UUKeypressTriggerType KT_INPUT_0 = new();

        /// <summary>
        /// Declares a keypress trigger input.
        /// </summary>
        public static readonly UUKeypressTriggerType KT_INPUT_1 = new();

        /// <summary>
        /// Declares a keypress trigger input.
        /// </summary>
        public static readonly UUKeypressTriggerType KT_INPUT_2 = new();

        /// <summary>
        /// Declares a keypress trigger input.
        /// </summary>
        public static readonly UUKeypressTriggerType KT_INPUT_3 = new();
    }
}