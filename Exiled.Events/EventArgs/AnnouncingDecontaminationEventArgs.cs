// -----------------------------------------------------------------------
// <copyright file="AnnouncingDecontaminationEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    /// <summary>
    /// Contains all informations before C.A.S.S.I.E announces light containment zone decontamination.
    /// </summary>
    public class AnnouncingDecontaminationEventArgs : EventArgs
    {
        private readonly int id;
        private readonly bool isGlobal;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnnouncingDecontaminationEventArgs"/> class.
        /// </summary>
        /// <param name="announcementId"><inheritdoc cref="Id"/></param>
        /// <param name="isGlobal"><inheritdoc cref="IsGlobal"/></param>
        public AnnouncingDecontaminationEventArgs(int announcementId, bool isGlobal)
        {
            id = announcementId;
            this.isGlobal = isGlobal;
        }

        /// <summary>
        /// Gets or sets the announcement id, from 0 to 6.
        /// </summary>
        public int Id
        {
            get => id;
            set { /* Setter will be removed */ }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the announcement is going to be global or not.
        /// </summary>
        public bool IsGlobal
        {
            get => isGlobal;
            set { /* Setter will be removed */ }
        }
    }
}
