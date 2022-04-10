// -----------------------------------------------------------------------
// <copyright file="HeavilyModdedOperationException.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Exceptions
{
    using System;
    using Exiled.API.Features;

    /// <summary>
    /// Exception that should be thrown when people try to use Heavily Modded features without HeavilyModded.
    /// </summary>
    public class HeavilyModdedOperationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeavilyModdedOperationException"/> class.
        /// </summary>
        /// <param name="info">The information about the exception which will be shown in the message.</param>
        public HeavilyModdedOperationException(string info)
            : base(info)
        {
            Log.Warn("The HeavilyModded flag should be set automatically by plugins or manually by users.");
            Log.Warn("You should never set heavily modded to true on mid-round.");
        }
    }
}
