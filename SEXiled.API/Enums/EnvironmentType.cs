// -----------------------------------------------------------------------
// <copyright file="EnvironmentType.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.API.Enums
{
    /// <summary>
    /// A set of environment types.
    /// </summary>
    public enum EnvironmentType
    {
        /// <summary>
        /// The development environment, for developers.
        /// </summary>
        Development,

        /// <summary>
        /// The testing environment, for testing things.
        /// </summary>
        Testing,

        /// <summary>
        /// The production environment, for the public.
        /// </summary>
        Production,

        /// <summary>
        /// The ptb environment, for Public Test Builds.
        /// </summary>
        Ptb,
    }
}
