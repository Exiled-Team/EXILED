// -----------------------------------------------------------------------
// <copyright file="RadioRange.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums {
    /// <summary>
    /// All possible <see cref="Radio"/> ranges.
    /// </summary>
    public enum RadioRange {
        /// <summary>
        /// The radio is disabled.
        /// </summary>
        Disabled = -1,

        /// <summary>
        /// The shortest range with the least battery usage.
        /// </summary>
        Short,

        /// <summary>
        /// The standard, default range.
        /// </summary>
        Medium,

        /// <summary>
        /// A longer range with increased battery usage.
        /// </summary>
        Long,

        /// <summary>
        /// The longest range with the most battery usage.
        /// </summary>
        Ultra,
    }
}
