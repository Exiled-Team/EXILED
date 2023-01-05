// -----------------------------------------------------------------------
// <copyright file="DecontaminatingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using Interfaces;

    /// <summary>
    ///     Contains all information before decontaminating the light containment zone.
    /// </summary>
    public class DecontaminatingEventArgs : IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DecontaminatingEventArgs" /> class.
        /// </summary>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public DecontaminatingEventArgs(bool isAllowed = true)
        {
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether or not light containment zone decontamination can begin.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}