// -----------------------------------------------------------------------
// <copyright file="ChangingAmmoEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Item
{
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Interfaces;
    using InventorySystem.Items.Firearms;

    using BaseFirearm = InventorySystem.Items.Firearms.Firearm;
    using Firearm = API.Features.Items.Firearm;

    /// <summary>
    /// Contains all information before changing firearm ammo.
    /// </summary>
    public class ChangingAmmoEventArgs : IPlayerEvent, IFirearmEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingAmmoEventArgs"/> class.
        /// </summary>
        /// <param name="baseFirearm"><inheritdoc cref="Firearm"/></param>
        /// <param name="oldStatus"><inheritdoc cref="OldAmmo"/></param>
        /// <param name="newStatus"><inheritdoc cref="NewAmmo"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ChangingAmmoEventArgs(BaseFirearm baseFirearm, FirearmStatus oldStatus, FirearmStatus newStatus, bool isAllowed = true)
        {
            Firearm = Item.Get(baseFirearm).As<Firearm>();
            Player = Firearm.Owner;
            OldStatus = oldStatus;
            NewStatus = newStatus;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="API.Features.Player"/> who's changing the <see cref="Firearm"/>'s ammo.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="API.Features.Items.Firearm"/> whose ammo is being changed.
        /// </summary>
        public Firearm Firearm { get; }

        /// <inheritdoc/>
        public Item Item => Firearm;

        /// <summary>
        /// Gets the old firearm status.
        /// </summary>
        public FirearmStatus OldStatus { get; }

        /// <summary>
        /// Gets the old ammo count of the firearm.
        /// </summary>
        public byte OldAmmo => OldStatus.Ammo;

        /// <summary>
        /// Gets or sets the new status that will be used by the firearm.
        /// </summary>
        public FirearmStatus NewStatus { get; set; }

        /// <summary>
        /// Gets or sets the new ammo count of the firearm.
        /// </summary>
        public byte NewAmmo
        {
            get => NewStatus.Ammo;
            set => NewStatus = new(value, NewStatus.Flags, NewStatus.Attachments);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the ammo can be changed.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
