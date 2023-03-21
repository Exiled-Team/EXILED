// -----------------------------------------------------------------------
// <copyright file="ITypeDiscriminator.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.API.Features.Interfaces
{
    using System;

    using Exiled.CustomRoles.API.Features.Parsers;

    /// <summary>
    /// A <see cref="Type"/> discriminator.
    /// </summary>
    public interface ITypeDiscriminator
    {
        /// <summary>
        /// Gets the base <see cref="Type"/>.
        /// </summary>
        Type BaseType { get; }

        /// <summary>
        /// Tries to resolve a mapping into a specific <see cref="Type"/>.
        /// </summary>
        /// <param name="buffer">The <see cref="ParsingEventBuffer"/> parser buffer.</param>
        /// <param name="suggestedType">The <see cref="Type"/> to resolve the mapping key.</param>
        /// <returns><see langword="true"/> if resolution is successful; otherwise, <see langword="false"/>.</returns>
        bool TryResolve(ParsingEventBuffer buffer, out Type? suggestedType);
    }
}