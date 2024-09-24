// -----------------------------------------------------------------------
// <copyright file="TestEscapeConfig.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.TestEscape
{
    using Exiled.CustomModules.API.Features.Attributes;
    using Exiled.CustomModules.API.Features.CustomEscapes;
    using Exiled.CustomModules.API.Features.Generic;

    /// <inheritdoc />
    [ModuleIdentifier]
    public class TestEscapeConfig : ModulePointer<CustomEscape>
    {
        /// <inheritdoc />
        public override uint Id { get; set; } = CustomEscapeType.TestEscape;

        /// <summary>
        /// Gets or sets a integer value.
        /// </summary>
        public int Value { get; set; } = 10;
    }
}