// -----------------------------------------------------------------------
// <copyright file="ChangingDurabilityEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using Exiled.API.Features.Items;

    /// <summary>
    /// Contains all informations before changing item durability.
    /// </summary>
    public class ChangingDurabilityEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingDurabilityEventArgs"/> class.
        /// </summary>
        /// <param name="firearm"><inheritdoc cref="Firearm"/></param>
        /// <param name="newDurability"><inheritdoc cref="NewDurability"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ChangingDurabilityEventArgs(Firearm firearm, float newDurability, bool isAllowed = true)
        {
            Firearm = firearm;
            NewDurability = newDurability;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="API.Features.Items.Firearm"/> the durability is being changed to.
        /// </summary>
        public Firearm Firearm { get; }

        /// <summary>
        /// Gets or sets the new durability to be used by the firearm.
        /// </summary>
        public float NewDurability { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the durability can be changed.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
