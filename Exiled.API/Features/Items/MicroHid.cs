// -----------------------------------------------------------------------
// <copyright file="MicroHid.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using System.Diagnostics;

    using Exiled.API.Features.Core;
    using Exiled.API.Interfaces;
    using InventorySystem.Items.MicroHID;

    /// <summary>
    /// A wrapper class for <see cref="MicroHIDItem"/>.
    /// </summary>
    [DebuggerDisplay("MicroHid")]
    public class MicroHid : Item, IWrapper<MicroHIDItem>
    {
        private readonly ConstProperty<double> preFire = new(1.7000000476837158, new[] { typeof(MicroHIDItem) });
        private readonly ConstProperty<double> minTimeToSwitch = new(0.3499999940395355, new[] { typeof(MicroHIDItem) });
        private readonly ConstProperty<double> powerdownTime = new(3.0999999046325684, new[] { typeof(MicroHIDItem) });

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroHid"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="MicroHIDItem"/> class.</param>
        public MicroHid(MicroHIDItem itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroHid"/> class, as well as a new Micro HID item.
        /// </summary>
        internal MicroHid()
            : this((MicroHIDItem)Server.Host.Inventory.CreateItemInstance(new(ItemType.MicroHID, 0), false))
        {
        }

        /// <summary>
        /// Gets or sets the remaining energy in the MicroHID.
        /// </summary>
        public float Energy
        {
            get => Base.RemainingEnergy;
            set => Base.RemainingEnergy = value;
        }

        /// <summary>
        /// Gets the <see cref="MicroHIDItem"/> base of the item.
        /// </summary>
        public new MicroHIDItem Base { get; }

        /// <summary>
        /// Gets or sets the <see cref="HidState"/>.
        /// </summary>
        public HidState State
        {
            get => Base.State;
            set => Base.State = value;
        }

        /// <summary>
        /// Gets or sets the time it takes to start firing from the MicroHID.
        /// </summary>
        public double PreFireTime
        {
            get => preFire.Value;
            set => preFire.Value = value;
        }

        /// <summary>
        /// Gets or sets the time it takes to switch the MicroHID to other mode.
        /// </summary>
        public double MinTimeToSwitch
        {
            get => minTimeToSwitch.Value;
            set => minTimeToSwitch.Value = value;
        }

        /// <summary>
        /// Gets or sets the time it takes to power down the MicroHID.
        /// </summary>
        public double PowerDownTime
        {
            get => powerdownTime.Value;
            set => powerdownTime.Value = value;
        }

        /// <summary>
        /// Starts firing the MicroHID.
        /// </summary>
        public void Fire() => Base.Fire();

        /// <summary>
        /// Recharges the MicroHID.
        /// </summary>
        public void Recharge() => Base.Recharge();

        /// <summary>
        /// Clones current <see cref="MicroHid"/> object.
        /// </summary>
        /// <returns> New <see cref="MicroHid"/> object. </returns>
        public override Item Clone() => new MicroHid()
        {
            State = State,
            Energy = Energy,
        };

        /// <summary>
        /// Returns the MicroHid in a human readable format.
        /// </summary>
        /// <returns>A string containing MicroHid-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Energy}| -{State}-";
    }
}