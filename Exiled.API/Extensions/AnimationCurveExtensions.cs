// -----------------------------------------------------------------------
// <copyright file="AnimationCurveExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using UnityEngine;

    /// <summary>
    /// A set of extensions for animation curves.
    /// </summary>
    public static class AnimationCurveExtensions
    {
        /// <summary>
        /// Modify the curve with the amount used.
        /// </summary>
        /// <param name="curve">The AnimationCurve to modify.</param>
        /// <param name="amount">The multiplier number.</param>
        /// <returns>The new modified curve.</returns>
        public static AnimationCurve Multiply(this AnimationCurve curve, float amount)
        {
            for (int i = 0; i < curve.length; i++)
                curve.keys[i].value *= amount;

            return curve;
        }

        /// <summary>
        /// Modify the curve with the amount used.
        /// </summary>
        /// <param name="curve">The AnimationCurve to modify.</param>
        /// <param name="amount">The add number.</param>
        /// <returns>The new modified curve.</returns>
        public static AnimationCurve Add(this AnimationCurve curve, float amount)
        {
            for (int i = 0; i < curve.length; i++)
                curve.keys[i].value += amount;

            return curve;
        }
    }
}