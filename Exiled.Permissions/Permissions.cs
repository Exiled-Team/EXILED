// -----------------------------------------------------------------------
// <copyright file="Permissions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Permissions
{
    using Exiled.API.Features;

    using MEC;

    /// <summary>
    /// Handles all plugin-related permissions, for executing commands, doing actions and so on.
    /// </summary>
    public sealed class Permissions : Plugin<Config>
    {
        private static Permissions instance;

        /// <summary>
        /// Gets the permissions instance.
        /// </summary>
        public static Permissions Instance => instance;

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            instance = this;

            base.OnEnabled();

            Timing.CallDelayed(
                5f,
                () =>
                {
                    Extensions.Permissions.Create();
                    Extensions.Permissions.Reload();
                });
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            base.OnDisabled();

            Extensions.Permissions.Groups.Clear();
        }
    }
}