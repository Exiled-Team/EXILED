// -----------------------------------------------------------------------
// <copyright file="Translation.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example
{
    using Exiled.API.Interfaces;

    /// <inheritdoc cref="ITranslation"/>
    public sealed class Translation : ITranslation
    {
        /// <summary>
        /// Some Text That Goes In Your Translations Configuration File.
        /// </summary>
        public string ExampleTranslation { get; set; } = "Example Translation Text";
    }
}