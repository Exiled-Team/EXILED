// -----------------------------------------------------------------------
// <copyright file="AddingUnitNameEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Server
{
    using Interfaces;

    using Respawning.NamingRules;

    /// <summary>
    /// Contains all information before adding a new unit name.
    /// </summary>
    public class AddingUnitNameEventArgs : IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddingUnitNameEventArgs"/> class.
        /// </summary>
        /// <param name="unitNamingRule">The generated unit name.</param>
        /// <param name="isAllowed">The value indicating whether the unit name can be added.</param>
        public AddingUnitNameEventArgs(UnitNamingRule unitNamingRule, bool isAllowed = true)
        {
            UnitNamingRule = unitNamingRule;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the generated <see cref="UnitNamingRule"/>.
        /// </summary>
        public UnitNamingRule UnitNamingRule { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the unit name can be added.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}