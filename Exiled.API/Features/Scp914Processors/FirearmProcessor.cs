// -----------------------------------------------------------------------
// <copyright file="FirearmProcessor.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Scp914Processors
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Extensions;
    using Exiled.API.Interfaces;
    using global::Scp914;
    using global::Scp914.Processors;

    /// <summary>
    /// A processor for <see cref="ItemCategory.Firearm"/>.
    /// </summary>
    public class FirearmProcessor : Scp914Processor, IWrapper<FirearmItemProcessor>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FirearmProcessor"/> class.
        /// </summary>
        /// <param name="scp914ItemProcessor">The <see cref="FirearmItemProcessor"/> instance.</param>
        public FirearmProcessor(FirearmItemProcessor scp914ItemProcessor)
            : base(scp914ItemProcessor)
        {
            Base = scp914ItemProcessor;
        }

        /// <inheritdoc/>
        public new FirearmItemProcessor Base { get; }

        /// <summary>
        /// Gets or sets a list of items that replace upgrading item when <see cref="Scp914KnobSetting"/> is <see cref="Scp914KnobSetting.Rough"/>.
        /// </summary>
        public IEnumerable<FirearmItemProcessor.FirearmOutput> RoughOutputs
        {
            get => Base._roughOutputs;
            set => Base._roughOutputs = value.ToArray();
        }

        /// <summary>
        /// Gets or sets a list of items that replace upgrading item when <see cref="Scp914KnobSetting"/> is <see cref="Scp914KnobSetting.Coarse"/>.
        /// </summary>
        public IEnumerable<FirearmItemProcessor.FirearmOutput> CoarseOutputs
        {
            get => Base._coarseOutputs;
            set => Base._coarseOutputs = value.ToArray();
        }

        /// <summary>
        /// Gets or sets a list of items that replace upgrading item when <see cref="Scp914KnobSetting"/> is <see cref="Scp914KnobSetting.OneToOne"/>.
        /// </summary>
        public IEnumerable<FirearmItemProcessor.FirearmOutput> OneToOneOutputs
        {
            get => Base._oneToOneOutputs;
            set => Base._oneToOneOutputs = value.ToArray();
        }

        /// <summary>
        /// Gets or sets a list of items that replace upgrading item when <see cref="Scp914KnobSetting"/> is <see cref="Scp914KnobSetting.Fine"/>.
        /// </summary>
        public IEnumerable<FirearmItemProcessor.FirearmOutput> FineOutputs
        {
            get => Base._fineOutputs;
            set => Base._fineOutputs = value.ToArray();
        }

        /// <summary>
        /// Gets or sets a list of items that replace upgrading item when <see cref="Scp914KnobSetting"/> is <see cref="Scp914KnobSetting.VeryFine"/>.
        /// </summary>
        public IEnumerable<FirearmItemProcessor.FirearmOutput> VeryFineOutputs
        {
            get => Base._veryFineOutputs;
            set => Base._veryFineOutputs = value.ToArray();
        }

        /// <inheritdoc/>
        public override ItemType GetRandomOutput(Scp914KnobSetting knobSetting, ItemType previousItem) => Base.GetItems(knobSetting, previousItem).Random();
    }
}