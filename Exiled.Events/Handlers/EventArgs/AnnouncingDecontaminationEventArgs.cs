// -----------------------------------------------------------------------
// <copyright file="AnnouncingDecontaminationEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.EventArgs
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Contains all informations before starting the decontamination.
    /// </summary>
    public class AnnouncingDecontaminationEventArgs : EventArgs
    {
        private int id;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnnouncingDecontaminationEventArgs"/> class.
        /// </summary>
        /// <param name="announcementId"><inheritdoc cref="Id"/></param>
        /// <param name="isGlobal"><inheritdoc cref="IsGlobal"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public AnnouncingDecontaminationEventArgs(int announcementId, bool isGlobal, bool isAllowed = true)
        {
            Id = announcementId;
            IsGlobal = isGlobal;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets the announcement id, from 0 to 5.
        /// </summary>
        public int Id
        {
            get => id;
            set => id = Mathf.Clamp(value, 0, 5);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the announcement is going to be global or not.
        /// </summary>
        public bool IsGlobal { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}