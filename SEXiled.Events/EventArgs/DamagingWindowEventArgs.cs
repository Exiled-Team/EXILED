// -----------------------------------------------------------------------
// <copyright file="DamagingWindowEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using System;

    /// <summary>
    /// Contains all informations before damage is dealt to a <see cref="BreakableWindow"/>.
    /// </summary>
    public class DamagingWindowEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DamagingWindowEventArgs"/> class.
        /// </summary>
        /// <param name="window"><inheritdoc cref="Window"/></param>
        /// <param name="damage"><inheritdoc cref="Damage"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public DamagingWindowEventArgs(BreakableWindow window, float damage, bool isAllowed = true)
        {
            Window = window;
            Damage = damage;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="BreakableWindow"/> object that is damaged.
        /// </summary>
        public BreakableWindow Window { get; }

        /// <summary>
        /// Gets or sets the damage the window will receive.
        /// </summary>
        public float Damage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the window can be broken.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
