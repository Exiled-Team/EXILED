// -----------------------------------------------------------------------
// <copyright file="TestRole2Config.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.CustomRoles.TestRole2
{
    using Exiled.CustomModules.API.Features.Attributes;
    using Exiled.CustomModules.API.Features.CustomRoles;
    using Exiled.CustomModules.API.Features.Generic;
    using Exiled.Example.TestRole;

    /// <inheritdoc />
    [ModuleIdentifier]
    public class TestRole2Config : ModulePointer<CustomRole>
    {
        /// <inheritdoc />
        public override uint Id { get; set; } = CustomRoleType.TestRole2;

        /// <summary>
        /// Gets or sets a integer value.
        /// </summary>
        public int Value { get; set; } = 10;
    }
}