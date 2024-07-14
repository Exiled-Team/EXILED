// -----------------------------------------------------------------------
// <copyright file="StandardProcessor.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Scp914Processors
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features.Pickups;
    using Exiled.API.Interfaces;
    using global::Scp914;
    using global::Scp914.Processors;
    using UnityEngine;

    /// <summary>
    /// A processor for most of items.
    /// </summary>
    public class StandardProcessor : Scp914Processor, IWrapper<StandardItemProcessor>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StandardProcessor"/> class.
        /// </summary>
        /// <param name="processor">The base <see cref="StandardItemProcessor"/> instance.</param>
        public StandardProcessor(StandardItemProcessor processor)
            : base(processor)
        {
            Base = processor;
        }

        /// <inheritdoc/>
        public new StandardItemProcessor Base { get; }

        /// <summary>
        /// Gets or sets a list of items that replace upgrading item when <see cref="Scp914KnobSetting"/> is <see cref="Scp914KnobSetting.Rough"/>.
        /// </summary>
        public IEnumerable<ItemType> RoughOutputs
        {
            get => Base._roughOutputs;
            set => Base._roughOutputs = value.ToArray();
        }

        /// <summary>
        /// Gets or sets a list of items that replace upgrading item when <see cref="Scp914KnobSetting"/> is <see cref="Scp914KnobSetting.Coarse"/>.
        /// </summary>
        public IEnumerable<ItemType> CoarseOutputs
        {
            get => Base._coarseOutputs;
            set => Base._coarseOutputs = value.ToArray();
        }

        /// <summary>
        /// Gets or sets a list of items that replace upgrading item when <see cref="Scp914KnobSetting"/> is <see cref="Scp914KnobSetting.OneToOne"/>.
        /// </summary>
        public IEnumerable<ItemType> OneToOneOutputs
        {
            get => Base._oneToOneOutputs;
            set => Base._oneToOneOutputs = value.ToArray();
        }

        /// <summary>
        /// Gets or sets a list of items that replace upgrading item when <see cref="Scp914KnobSetting"/> is <see cref="Scp914KnobSetting.Fine"/>.
        /// </summary>
        public IEnumerable<ItemType> FineOutputs
        {
            get => Base._fineOutputs;
            set => Base._fineOutputs = value.ToArray();
        }

        /// <summary>
        /// Gets or sets a list of items that replace upgrading item when <see cref="Scp914KnobSetting"/> is <see cref="Scp914KnobSetting.VeryFine"/>.
        /// </summary>
        public IEnumerable<ItemType> VeryFineOutputs
        {
            get => Base._veryFineOutputs;
            set => Base._veryFineOutputs = value.ToArray();
        }

        /// <summary>
        /// Gets or sets a value indicating whether item which has <see cref="InventorySystem.Items.IUpgradeTrigger"/> will execute method for updating.
        /// </summary>
        public bool FireTrigger
        {
            get => Base._fireUpgradeTrigger;
            set => Base._fireUpgradeTrigger = value;
        }

        /// <inheritdoc/>
        public override ItemType GetRandomOutput(Scp914KnobSetting knobSetting, ItemType previousItem) => Base.RandomOutput(knobSetting, previousItem);

        /// <summary>
        /// Handles pickup if new <see cref="ItemType"/> was None.
        /// </summary>
        /// <param name="pickup">Pickup to handle.</param>
        public void HandleNone(Pickup pickup) => Base.HandleNone(pickup.Base, Vector3.zero);

        /// <summary>
        /// Handles old pickup that has been upgraded.
        /// </summary>
        /// <param name="pickup">Pickup to handle.</param>
        public void HandleOldPickup(Pickup pickup) => Base.HandleOldPickup(pickup.Base, Vector3.zero);
    }
}