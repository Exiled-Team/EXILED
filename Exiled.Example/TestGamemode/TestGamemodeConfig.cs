// -----------------------------------------------------------------------
// <copyright file="TestGamemodeConfig.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.TestGamemode
{
    using Exiled.CustomModules.API.Features.Attributes;
    using Exiled.CustomModules.API.Features.CustomGameModes;
    using Exiled.CustomModules.API.Features.Generic;

    /// <inheritdoc />
    [ModuleIdentifier]
    public class TestGamemodeConfig : ModulePointer<CustomGameMode>
    {
        /// <inheritdoc />
        public override uint Id { get; set; } = CustomGamemodeType.TestGamemode;

        /// <summary>
        /// Gets or sets a integer value.
        /// </summary>
        public int Value { get; set; } = 10;
    }
}