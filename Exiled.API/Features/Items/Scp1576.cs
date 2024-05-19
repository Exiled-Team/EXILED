// -----------------------------------------------------------------------
// <copyright file="Scp1576.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using Exiled.API.Features.Core;
    using Exiled.API.Interfaces;
    using InventorySystem.Items.Usables;
    using InventorySystem.Items.Usables.Scp1576;

    /// <summary>
    /// A wrapper class for <see cref="Scp1576Item"/>.
    /// </summary>
    public class Scp1576 : Usable, IWrapper<Scp1576Item>
    {
        private readonly ConstProperty<float> cooldown = new(Scp1576Item.UseCooldown, new[] { typeof(Scp1576Item) });
        private readonly ConstProperty<double> warningDuration = new(Scp1576Item.WarningDuration, new[] { typeof(Scp1576Item) });

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp1576"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="Scp1576Item"/> class.</param>
        public Scp1576(Scp1576Item itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp1576"/> class.
        /// </summary>
        internal Scp1576()
            : this((Scp1576Item)Server.Host.Inventory.CreateItemInstance(new(ItemType.SCP1576, 0), false))
        {
        }

        /// <summary>
        /// Gets the <see cref="UsableItem"/> that this class is encapsulating.
        /// </summary>
        public new Scp1576Item Base { get; }

        /// <summary>
        /// Gets Scp1576Playback.
        /// </summary>
        public Scp1576Playback PlaybackTemplate => Base.PlaybackTemplate;

        /// <summary>
        /// Gets or sets the cooldown for using the SCP-1576.
        /// </summary>
        public float Cooldown
        {
            get => cooldown;
            set => cooldown.Value = value;
        }

        /// <summary>
        /// Gets or sets the warning duration.
        /// </summary>
        public double WarningDuration
        {
            get => warningDuration;
            set => warningDuration.Value = value;
        }

        /// <summary>
        /// Forcefully stops the transmission of SCP-1576.
        /// </summary>
        public void StopTransmitting() => Base.ServerStopTransmitting();
    }
}
