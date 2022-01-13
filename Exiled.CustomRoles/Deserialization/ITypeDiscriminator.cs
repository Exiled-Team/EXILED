// -----------------------------------------------------------------------
// <copyright file="ITypeDiscriminator.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.Deserialization
{
    using System;
    using YamlDotNet.Core.Events;

    /// <summary>
    ///     Provider interface for creating TypeDiscriminators that are capable of resolving subtypes of abstract and interface
    ///     classes.
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
        /// <returns><see langword="true" /> if successful, <see langword="false" /> otherwise.</returns>
        bool TryResolve(ParsingEventBuffer parser, out Type suggestedType);
    }
}