// -----------------------------------------------------------------------
// <copyright file="CustomEscapeType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.TestEscape
{
    using Exiled.CustomModules.API.Enums;

    /// <summary>
    /// The custom escape type.
    /// </summary>
    public class CustomEscapeType : UUCustomEscapeType
    {
        /// <summary>
        /// Initializes a new custom escape id.
        /// </summary>
        public static readonly CustomEscapeType TestEscape = new();
    }
}