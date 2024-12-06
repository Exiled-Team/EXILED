// -----------------------------------------------------------------------
// <copyright file="Scp914Processor.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Scp914Processors
{
    using System.Collections.Generic;

    using Exiled.API.Features.Core;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Interfaces;
    using global::Scp914;
    using global::Scp914.Processors;

    /// <summary>
    /// A wrapper for base class for all processors.
    /// </summary>
    public class Scp914Processor : TypeCastObject<Scp914Processor>, IWrapper<Scp914ItemProcessor>
    {
        /// <summary>
        /// Gets the <see cref="Dictionary{TKey,TValue}"/> with <see cref="Scp914ItemProcessor"/> and <see cref="Scp914Processor"/>.
        /// </summary>
        internal static readonly Dictionary<Scp914ItemProcessor, Scp914Processor> ProcessorToWrapper = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp914Processor"/> class.
        /// </summary>
        /// <param name="scp914ItemProcessor">The <see cref="Scp914ItemProcessor"/> instance.</param>
        public Scp914Processor(Scp914ItemProcessor scp914ItemProcessor)
        {
            Base = scp914ItemProcessor;

            ProcessorToWrapper.Add(scp914ItemProcessor, this);
        }

        /// <inheritdoc/>
        public Scp914ItemProcessor Base { get; }

        /// <summary>
        /// Gets a <see cref="StandardProcessor"/> from it's base <see cref="StandardItemProcessor"/>.
        /// </summary>
        /// <param name="scp914ItemProcessor">The <see cref="StandardItemProcessor"/> instance.</param>
        /// <returns>The <see cref="StandardProcessor"/> instance.</returns>
        public static Scp914Processor Get(Scp914ItemProcessor scp914ItemProcessor) => ProcessorToWrapper.TryGetValue(scp914ItemProcessor, out Scp914Processor processor) ? processor : scp914ItemProcessor switch
        {
            AmmoItemProcessor ammoItemProcessor => new AmmoProcessor(ammoItemProcessor),
            FirearmItemProcessor firearmItemProcessor => new FirearmProcessor(firearmItemProcessor),
            MicroHidItemProcessor microHidItemProcessor => new MicroHidProcessor(microHidItemProcessor),
            StandardItemProcessor standardItemProcessor => new StandardProcessor(standardItemProcessor),
            _ => new Scp914Processor(scp914ItemProcessor)
        };

        /// <summary>
        /// Upgrades an item from player's inventory.
        /// </summary>
        /// <param name="item">Item to update.</param>
        /// <param name="scp914KnobSetting">Setting to use.</param>
        /// <returns>A new upgraded item.</returns>
        public Scp914Result UpgradeInventoryItem(Item item, Scp914KnobSetting scp914KnobSetting) => Base.UpgradeInventoryItem(scp914KnobSetting, item.Base);

        /// <summary>
        /// Upgrades a pickup from player's inventory.
        /// </summary>
        /// <param name="pickup">Pickup to update.</param>
        /// <param name="scp914KnobSetting">Setting to use.</param>
        /// <returns>A new upgraded pickup.</returns>
        public Scp914Result UpgradePickup(Pickup pickup, Scp914KnobSetting scp914KnobSetting) => Base.UpgradePickup(scp914KnobSetting, pickup.Base);

        /// <summary>
        /// Gets a random output item.
        /// </summary>
        /// <param name="knobSetting">Selected <see cref="Scp914KnobSetting"/>.</param>
        /// <param name="previousItem">The item to be updated.</param>
        /// <returns>A new item.</returns>
        public virtual ItemType GetRandomOutput(Scp914KnobSetting knobSetting, ItemType previousItem) => previousItem;
    }
}