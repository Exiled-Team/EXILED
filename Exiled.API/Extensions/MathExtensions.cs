// -----------------------------------------------------------------------
// <copyright file="MathExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using UnityEngine;

    /// <summary>
    /// A set of extensions for mathematical operations.
    /// </summary>
    public static class MathExtensions
    {
        /// <summary>Returns the square of the Euclidean distance between two specified points.</summary>
        /// <param name="value1">First point.</param>
        /// <param name="value2">Seconds point.</param>
        /// <returns>Square of the distance.</returns>
        public static float DistanceSquared(Vector3 value1, Vector3 value2) => (value1 - value2).sqrMagnitude;

        /// <summary>
        /// Evaluates a probability.
        /// </summary>
        /// <param name="probability">The probability to evaluate.</param>
        /// <param name="minInclusive">The minimum value to include in the range.</param>
        /// <param name="maxInclusive">The maximum value to include in the range.</param>
        /// <returns><see langword="true"/> if the probability occurred, otherwise <see langword="false"/>.</returns>
        public static bool EvaluateProbability(this int probability, int minInclusive = 0, int maxInclusive = 101) => probability == 100 || Random.Range(minInclusive, maxInclusive) <= probability;

        /// <summary>
        /// Evaluates a probability.
        /// </summary>
        /// <param name="probability">The probability to evaluate.</param>
        /// <param name="minInclusive">The minimum value to include in the range.</param>
        /// <param name="maxInclusive">The maximum value to include in the range.</param>
        /// <returns><see langword="true"/> if the probability occurred, otherwise <see langword="false"/>.</returns>
        public static bool EvaluateProbability(this float probability, float minInclusive = 0f, float maxInclusive = 100f) => Random.Range(minInclusive, maxInclusive) <= probability;
    }
}