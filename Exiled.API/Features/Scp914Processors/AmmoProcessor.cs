// -----------------------------------------------------------------------
// <copyright file="AmmoProcessor.cs" company="Exiled Team">
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
    /// A processor for <see cref="ItemCategory.Ammo"/>.
    /// </summary>
    public class AmmoProcessor : Scp914Processor, IWrapper<AmmoItemProcessor>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AmmoProcessor"/> class.
        /// </summary>
        /// <param name="scp914ItemProcessor">The <see cref="AmmoItemProcessor"/> instance.</param>
        public AmmoProcessor(AmmoItemProcessor scp914ItemProcessor)
            : base(scp914ItemProcessor)
        {
            Base = scp914ItemProcessor;
        }

        /// <inheritdoc/>
        public new AmmoItemProcessor Base { get; }

        /// <summary>
        /// Gets or sets a new <see cref="ItemType"/> to which item will be upgraded on <see cref="Scp914KnobSetting.Coarse"/> or <see cref="Scp914KnobSetting.Rough"/>.
        /// </summary>
        public ItemType PreviousAmmo
        {
            get => Base._previousAmmo;
            set => Base._previousAmmo = value;
        }

        /// <summary>
        /// Gets or sets a new <see cref="ItemType"/> to which item will be upgraded on <see cref="Scp914KnobSetting.Fine"/>.
        /// </summary>
        public ItemType OneToOneAmmo
        {
            get => Base._oneToOne;
            set => Base._oneToOne = value;
        }

        /// <summary>
        /// Gets or sets a new <see cref="ItemType"/> to which item will be upgraded on <see cref="Scp914KnobSetting.Fine"/> or <see cref="Scp914KnobSetting.VeryFine"/>.
        /// </summary>
        public ItemType NextAmmo
        {
            get => Base._nextAmmo;
            set => Base._nextAmmo = value;
        }

        /// <inheritdoc/>
        public override ItemType GetRandomOutput(Scp914KnobSetting knobSetting, ItemType previousItem) => knobSetting switch
        {
            Scp914KnobSetting.Rough or Scp914KnobSetting.Coarse => PreviousAmmo,
            Scp914KnobSetting.OneToOne => OneToOneAmmo,
            _ => NextAmmo
        };
    }
}