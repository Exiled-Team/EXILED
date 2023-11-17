// -----------------------------------------------------------------------
// <copyright file="RadioRange.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using Features.Items;

    /// <summary>
    /// All possible <see cref="Radio"/> ranges.
    /// </summary>
    /// <seealso cref="Radio.Range"/>
    public enum RadioRange : byte
    {
        /// <summary>
        /// The shortest range with the lowest battery usage.
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