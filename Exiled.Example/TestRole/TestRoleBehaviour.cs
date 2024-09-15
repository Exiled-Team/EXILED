// -----------------------------------------------------------------------
// <copyright file="TestRoleBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.TestRole
{
    using Exiled.API.Features;
    using Exiled.CustomModules.API.Features.CustomRoles;

    /// <inheritdoc />
    public class TestRoleBehaviour : RoleBehaviour
    {
        /// <summary>
        /// Gets or sets a integer value.
        /// </summary>
        public int Value { get; set; }

        /// <inheritdoc />
        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            Log.Info($"Value: {Value}");
        }
    }
}