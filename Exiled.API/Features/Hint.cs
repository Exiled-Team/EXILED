// -----------------------------------------------------------------------
// <copyright file="Hint.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    /// <summary>
    /// Useful class to save hint configs in a cleaner way.
    /// </summary>
    public class Hint
    {
        /// <summary>
        /// Gets or sets the content of the hint.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the duration of the hint.
        /// </summary>
        public float Duration { get; set; }
    }
}
