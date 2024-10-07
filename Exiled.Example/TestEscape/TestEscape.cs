// -----------------------------------------------------------------------
// <copyright file="TestEscape.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.TestEscape
{
    using System.Collections.Generic;

    using Exiled.API.Features;
    using Exiled.CustomModules.API.Features.Attributes;
    using Exiled.CustomModules.API.Features.CustomEscapes;
    using PlayerRoles;

    /// <inheritdoc />
    [ModuleIdentifier]
    public class TestEscape : CustomEscape<TestEscapeBehaviour>
    {
        /// <inheritdoc />
        public override uint Id { get; set; } = CustomEscapeType.TestEscape;

        /// <inheritdoc />
        public override string Name { get; set; } = "Test Escape";

        /// <inheritdoc />
        public override bool IsEnabled { get; set; } = true;

        /// <inheritdoc />
        public override Dictionary<byte, Hint> Scenarios { get; set; } = new()
        {
            [1] = new Hint("Test Scenario"),
        };

        /// <inheritdoc />
        public override List<EscapeSettings> Settings { get; set; } = new()
        {
            new EscapeSettings(true, RoleTypeId.FacilityGuard),
        };
    }
}