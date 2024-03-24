// -----------------------------------------------------------------------
// <copyright file="MicroHidProcessor.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Scp914Processors
{
    using Exiled.API.Interfaces;
    using global::Scp914;
    using global::Scp914.Processors;

    /// <summary>
    /// A processor for <see cref="ItemType.MicroHID"/>.
    /// </summary>
    public class MicroHidProcessor : Scp914Processor, IWrapper<MicroHidItemProcessor>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroHidProcessor"/> class.
        /// </summary>
        /// <param name="scp914ItemProcessor">The <see cref="MicroHidItemProcessor"/> instance.</param>
        public MicroHidProcessor(MicroHidItemProcessor scp914ItemProcessor)
            : base(scp914ItemProcessor)
        {
            Base = scp914ItemProcessor;
        }

        /// <inheritdoc/>
        public new MicroHidItemProcessor Base { get; }

        /// <inheritdoc/>
        public override ItemType GetRandomOutput(Scp914KnobSetting knobSetting, ItemType previousItem) => Base.GetOutput(knobSetting);
    }
}