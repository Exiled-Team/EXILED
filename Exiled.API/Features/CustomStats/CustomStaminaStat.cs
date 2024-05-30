// -----------------------------------------------------------------------
// <copyright file="CustomStaminaStat.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.CustomStats
{
    using Mirror;
    using PlayerStatsSystem;
    using UnityEngine;

    /// <summary>
    /// A custom version of <see cref="StaminaStat"/> which allows the player's maximum amount of Stamina to be changed.
    /// </summary>
    public class CustomStaminaStat : StaminaStat
    {
        /// <inheritdoc/>
        public override float MaxValue => CustomMaxValue == default ? base.MaxValue : CustomMaxValue;

        /// <summary>
        /// Gets or sets the maximum amount of stamina the player will have.
        /// </summary>
        public float CustomMaxValue { get; set; }

        /// <summary>
        /// Clamps a float to fit the current stamina bar.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <returns>The clamped num.</returns>
        public float Clamp(float value) => CustomMaxValue == default ? Mathf.Clamp01(value) : Mathf.Clamp(value, 0, MaxValue);

        /// <summary>
        /// Overiding NW Method to sync Player percentage of Stamina.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void WriteValue(NetworkWriter writer)
        {
            if (CustomMaxValue == default)
            {
                base.WriteValue(writer);
                return;
            }

            writer.WriteByte(ToByte(CurValue / CustomMaxValue));
        }

        /// <summary>
        /// Overriding NW Method to sync Player percentage of Stamina.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>the float value sync to server.</returns>
        public override float ReadValue(NetworkReader reader) => CustomMaxValue == default ? base.ReadValue(reader) : CurValue * CustomMaxValue;
    }
}