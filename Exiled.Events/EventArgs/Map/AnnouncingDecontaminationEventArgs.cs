// -----------------------------------------------------------------------
// <copyright file="AnnouncingDecontaminationEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using Exiled.API.Enums;
    using Exiled.Events.EventArgs.Interfaces;
    using LightContainmentZoneDecontamination;

    /// <summary>
    ///     Contains all information before C.A.S.S.I.E announces light containment zone decontamination.
    /// </summary>
    public class AnnouncingDecontaminationEventArgs : IExiledEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AnnouncingDecontaminationEventArgs" /> class.
        /// </summary>
        /// <param name="announcementId">
        ///     <inheritdoc cref="Id" />
        /// </param>
        public AnnouncingDecontaminationEventArgs(int announcementId)
        {
            Id = announcementId;
            IsGlobal = DecontaminationController.Singleton.IsAnnouncementHearable;
            CurrentPhase = (DecontaminationPhase)DecontaminationController.Singleton.DecontaminationPhases[announcementId].Function;
        }

        /// <summary>
        ///     Gets the announcement id, from 0 to 6.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Gets the <see cref="DecontaminationPhase"/>.
        /// </summary>
        public DecontaminationPhase CurrentPhase { get; }

        /// <summary>
        ///     Gets a value indicating whether the announcement is going to be global or not.
        /// </summary>
        public bool IsGlobal { get; }
    }
}