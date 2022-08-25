// -----------------------------------------------------------------------
// <copyright file="Badge.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    /// <summary>
    /// Represents an in-game badge.
    /// </summary>
    public struct Badge
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Badge"/> struct.
        /// </summary>
        /// <param name="text">The badge text.</param>
        /// <param name="color">The badge color.</param>
        /// <param name="type">The badge type.</param>
        /// <param name="isGlobal">Indicates whether the badge is global or not.</param>
        public Badge(string text, string color, int type, bool isGlobal = false)
        {
            Text = text;
            Color = color;
            Type = type;
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
        /// Gets the badge type.
        /// </summary>
        public int Type { get; }

        /// <summary>
        /// Gets a value indicating whether the badge is global or not.
        /// </summary>
        public bool IsGlobal { get; }

        /// <summary>
        /// Returns the Badge in a human-readable format.
        /// </summary>
        /// <returns>A string containing Badge-related data.</returns>
        public override string ToString() => $"{Text} ({Color}) [{Type}] *{IsGlobal}*";
    }
}