// -----------------------------------------------------------------------
// <copyright file="TestTeamConfig.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.TestTeam
{
    using Exiled.CustomModules.API.Features.Attributes;
    using Exiled.CustomModules.API.Features.CustomRoles;
    using Exiled.CustomModules.API.Features.Generic;

    /// <inheritdoc />
    [ModuleIdentifier]
    public class TestTeamConfig : ModulePointer<CustomTeam>
    {
        /// <inheritdoc />
        public override uint Id { get; set; } = CustomTeamType.TestTeam;

        /// <summary>
        /// Gets or sets a integer value.
        /// </summary>
        public int Value { get; set; } = 10;
    }
}