// -----------------------------------------------------------------------
// <copyright file="GeneratorActivatedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    /// <summary>
    /// Contains all informations after activating a generator.
    /// </summary>
    public class GeneratorActivatedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratorActivatedEventArgs"/> class.
        /// </summary>
        /// <param name="generator"><inheritdoc cref="Generator"/></param>
        public GeneratorActivatedEventArgs(Generator079 generator) => Generator = generator;

        /// <summary>
        /// Gets the activated generator.
        /// </summary>
        public Generator079 Generator { get; private set; }
    }
}