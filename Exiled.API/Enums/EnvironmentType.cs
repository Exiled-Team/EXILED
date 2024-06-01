// -----------------------------------------------------------------------
// <copyright file="EnvironmentType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
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

        /// <summary>
        /// The Production environment, for the public, with debugging features.
        /// </summary>
        ProductionDebug,
    }
}