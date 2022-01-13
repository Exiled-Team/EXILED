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

    using YamlDotNet.Core.Events;

    /// <summary>
    ///     Provider interface for creating <see cref="Type"/> discriminators that are capable of resolving subtypes of
    ///     abstract and interface classes.
    /// </summary>
    public interface ITypeDiscriminator
    {
        /// <summary>
        ///     Gets an abstract/interface type that this Type Discriminator should resolve.
        /// </summary>
        Type BaseType { get; }

        /// <summary>
        ///     Tries to resolve an appropriate type.
        /// </summary>
        /// <param name="parser">An event parser containing <see cref="ParsingEvent" />s.</param>
        /// <param name="suggestedType"><see cref="Type" /> of the resolved type.</param>
        /// <returns><see langword="true" /> if resolution is successful; otherwise <see langword="false" />.</returns>
        bool TryResolve(ParsingEventBuffer parser, out Type suggestedType);
    }
}