// -----------------------------------------------------------------------
// <copyright file="TestRole2.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.CustomRoles.TestRole2
{
    using Exiled.API.Enums;
    using Exiled.CustomModules.API.Features.Attributes;
    using Exiled.CustomModules.API.Features.CustomRoles;
    using Exiled.Example.TestRole;
    using PlayerRoles;

    /// <inheritdoc />
    [ModuleIdentifier]
    public class TestRole2 : CustomRole<TestRole2Behaviour>
    {
        /// <inheritdoc />
        public override string Name { get; set; } = "TestRole2";

        /// <inheritdoc />
        public override uint Id { get; set; } = CustomRoleType.TestRole2;

        /// <inheritdoc />
        public override bool IsEnabled { get; set; } = true;

        /// <inheritdoc />
        public override string Description { get; set; } = "Custom role for testing purposes.";

        /// <inheritdoc />
        public override RoleTypeId Role { get; set; } = RoleTypeId.Scientist;

        /// <inheritdoc />
        public override int Probability { get; set; } = 100;

        /// <inheritdoc />
        public override int MaxInstances { get; set; } = 1;

        /// <inheritdoc />
        public override Team[] TeamsOwnership { get; set; } = { Team.Scientists };

        /// <inheritdoc />
        public override RoleTypeId AssignFromRole { get; set; } = RoleTypeId.Scientist;

        /// <inheritdoc />
        public override bool IsScp { get; set; } = true;

        /// <inheritdoc />
        public override RoleSettings Settings { get; set; } = new()
        {
            UseDefaultRoleOnly = true,
            UniqueRole = RoleTypeId.ClassD,

            Health = 300,
            MaxHealth = 400,
            Scale = 0.90f,
            CustomInfo = "Test Role",

            SpawnedText = new("You've been spawned as Test Role", 10, channel: TextChannelType.Broadcast),

            PreservePosition = true,

            PreserveInventory = true,

            CanActivateWarhead = true,

            CanBypassCheckpoints = true,
            CanActivateGenerators = false,
            CanPlaceBlood = false,
            CanBeHurtByScps = false,
            CanHurtScps = false,
            CanBeHandcuffed = false,

            DoesLookingAffectScp096 = false,
            DoesLookingAffectScp173 = false,
        };
    }
}