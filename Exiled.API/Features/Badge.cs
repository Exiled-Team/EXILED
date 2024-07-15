// -----------------------------------------------------------------------
// <copyright file="Badge.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents an in-game badge.
    /// </summary>
    public readonly struct Badge
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Badge"/> struct.
        /// </summary>
        /// <param name="text">The badge text.</param>
        /// <param name="color">The badge color.</param>
        /// <param name="isGlobal">Indicates whether the badge is global or not.</param>
        public Badge(string text, string color, bool isGlobal = false)
        {
            Text = text;
            Color = color;
            IsGlobal = isGlobal;
        }

        /// <summary>
        /// Gets the badge text.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Gets the badge color.
        /// </summary>
        public string Color { get; }

        /// <summary>
        /// Gets a value indicating whether the badge is global or not.
        /// </summary>
        public bool IsGlobal { get; }

        /// <summary>
        /// Checks if the provided hex color can be used in badges.
        /// </summary>
        /// <param name="hex">The hex color code, including the <c>#</c>.</param>
        /// <param name="colorType">If the method returns <see langword="true"/>, this will be an enum representing the hex code. If the method returns <see langword="false"/>, this will be <see langword="null"/>.</param>
        /// <returns>Whether the provided hex color code can be used in badges.</returns>
        public static bool IsValidColor(string hex, out Misc.PlayerInfoColorTypes? colorType)
        {
            foreach (KeyValuePair<Misc.PlayerInfoColorTypes, string> option in Misc.AllowedColors)
            {
                if (option.Value.Equals(hex, StringComparison.OrdinalIgnoreCase))
                {
                    colorType = option.Key;
                    return true;
                }
            }

            colorType = null;
            return false;
        }

        /// <summary>
        /// Gets the hex color code of the provided <see cref="Misc.PlayerInfoColorTypes"/>.
        /// </summary>
        /// <param name="colorType">The <see cref="Misc.PlayerInfoColorTypes"/> to get the hex color code of.</param>
        /// <returns>The hex color code, including the <c>#</c>.</returns>
        public static string GetHexColor(Misc.PlayerInfoColorTypes colorType) => Misc.AllowedColors[colorType];

        /// <summary>
        /// Returns the Badge in a human-readable format.
        /// </summary>
        /// <returns>A string containing Badge-related data.</returns>
        public override string ToString() => $"{Text} ({Color}) [{IsGlobal}]";
    }
}