// -----------------------------------------------------------------------
// <copyright file="AddingUnitNameEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Server
{
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before adding a new unit name.
    /// </summary>
    public class AddingUnitNameEventArgs : IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddingUnitNameEventArgs"/> class.
        /// </summary>
        /// <param name="unitName">The generated unit name.</param>
        /// <param name="isAllowed">The value indicating whether or not the unit name can be added.</param>
        public AddingUnitNameEventArgs(string unitName, bool isAllowed = true)
        {
            UnitName = unitName;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets the generated unit name.
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the unit name can be added.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
