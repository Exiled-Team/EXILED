// -----------------------------------------------------------------------
// <copyright file="GeneratorActivatedEventArgs.cs" company="Exiled Team">
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
    public class GeneratorActivatedEventArgs : IGeneratorEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="GeneratorActivatedEventArgs" /> class.
        /// </summary>
        /// <param name="generator">
        ///     <inheritdoc cref="Generator" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public GeneratorActivatedEventArgs(Scp079Generator generator, bool isAllowed = true)
        {
            Generator = Generator.Get(generator);
            IsAllowed = isAllowed;
        }

        /// <inheritdoc />
        public Generator Generator { get; }

        /// <inheritdoc />
        public bool IsAllowed { get; set; }
    }
}