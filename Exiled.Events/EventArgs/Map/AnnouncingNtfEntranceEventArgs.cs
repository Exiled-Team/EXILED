// -----------------------------------------------------------------------
// <copyright file="AnnouncingNtfEntranceEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using Interfaces;

    /// <summary>
    /// Contains all information before C.A.S.S.I.E announces the NTF entrance.
    /// </summary>
    public class AnnouncingNtfEntranceEventArgs : IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnnouncingNtfEntranceEventArgs" /> class.
        /// </summary>
        /// <param name="scpsLeft">
        /// <inheritdoc cref="ScpsLeft" />
        /// </param>
        /// <param name="unitName">
        /// <inheritdoc cref="UnitName" />
        /// </param>
        /// <param name="unitNumber">
        /// <inheritdoc cref="UnitNumber" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public AnnouncingNtfEntranceEventArgs(int scpsLeft, string unitName, int unitNumber, bool isAllowed = true)
        {
            ScpsLeft = scpsLeft;
            UnitName = unitName;
            UnitNumber = unitNumber;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets the number of SCPs left.
        /// </summary>
        public int ScpsLeft { get; set; }

        /// <summary>
        /// Gets or sets the NTF unit name.
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// Gets or sets the NTF unit number.
        /// </summary>
        public int UnitNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the NTF spawn will be announced by C.A.S.S.I.E.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}