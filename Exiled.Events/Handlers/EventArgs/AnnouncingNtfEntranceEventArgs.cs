// -----------------------------------------------------------------------
// <copyright file="AnnouncingNtfEntranceEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.EventArgs
{
    using System;

    /// <summary>
    /// Contains all informations before announcing the ntf entrance.
    /// </summary>
    public class AnnouncingNtfEntranceEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnnouncingNtfEntranceEventArgs"/> class.
        /// </summary>
        /// <param name="scpsLeft"><inheritdoc cref="ScpsLeft"/></param>
        /// <param name="ntfNumber"><inheritdoc cref="NtfNumber"/></param>
        /// <param name="ntfLetter"><inheritdoc cref="NtfLetter"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public AnnouncingNtfEntranceEventArgs(int scpsLeft, int ntfNumber, char ntfLetter, bool isAllowed = true)
        {
            ScpsLeft = scpsLeft;
            NtfNumber = ntfNumber;
            NtfLetter = ntfLetter;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets the number of scps left.
        /// </summary>
        public int ScpsLeft { get; set; }

        /// <summary>
        /// Gets or sets the number of ntf.
        /// </summary>
        public int NtfNumber { get; set; }

        /// <summary>
        /// Gets or sets the ntf letter.
        /// </summary>
        public char NtfLetter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}