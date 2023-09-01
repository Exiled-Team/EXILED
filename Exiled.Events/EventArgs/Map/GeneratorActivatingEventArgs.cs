// -----------------------------------------------------------------------
// <copyright file="GeneratorActivatingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using API.Features;

    using Interfaces;

    using MapGeneration.Distributors;

    /// <summary>
    ///     Contains all information after activating a generator.
    /// </summary>
    public class GeneratorActivatingEventArgs : IGeneratorEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="GeneratorActivatingEventArgs" /> class.
        /// </summary>
        /// <param name="generator">
        ///     <inheritdoc cref="Generator" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public GeneratorActivatingEventArgs(Scp079Generator generator, bool isAllowed = true)
        {
            Generator = Generator.Get(generator);
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the activated generator.
        /// </summary>
        public Generator Generator { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether the generator can be activated or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}