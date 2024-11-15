// -----------------------------------------------------------------------
// <copyright file="TestRoleConfig.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.CustomRoles.TestRole
{
    using Exiled.CustomModules.API.Features.Attributes;
    using Exiled.CustomModules.API.Features.CustomRoles;
    using Exiled.CustomModules.API.Features.Generic;
    using Exiled.Example.TestRole;

    /// <inheritdoc />
    [ModuleIdentifier]
    public class TestRoleConfig : ModulePointer<CustomRole>
    {
        /// <inheritdoc />
        public override uint Id { get; set; } = CustomRoleType.TestRole;

        /// <summary>
        /// Gets or sets a integer value.
        /// </summary>
        public int Value { get; set; } = 10;
    }
}