// -----------------------------------------------------------------------
// <copyright file="RadioRangeSettings.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Structs
{
    using System.Diagnostics;

    /// <summary>
    /// Settings for specific radio ranges.
    /// </summary>
    [DebuggerDisplay("IdleUsage = {IdleUsage} TalkingUsage = {TalkingUsage} MaxRange = {MaxRange}")]
    public struct RadioRangeSettings
    {
        /// <summary>
        /// The amount of battery usage per minute while idle.
        /// </summary>
        public float IdleUsage;

        /// <summary>
        /// The amount of battery usage per minute while talking.
        /// </summary>
        public int TalkingUsage;

        /// <summary>
        /// The maximum range in which this radio will pickup and send voice messages.
        /// </summary>
        public int MaxRange;
    }
}