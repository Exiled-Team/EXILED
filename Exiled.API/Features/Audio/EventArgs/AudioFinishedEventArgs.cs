// -----------------------------------------------------------------------
// <copyright file="AudioFinishedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Audio.EventArgs
{
    using System;

    /// <summary>
    /// Contains all information after finishing the current audio file.
    /// </summary>
    public class AudioFinishedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioFinishedEventArgs"/> class.
        /// </summary>
        /// <param name="audioPlayer"><see cref="AudioPlayer"/>.</param>
        /// <param name="audioFile"><see cref="AudioFile"/>.</param>
        public AudioFinishedEventArgs(AudioPlayer audioPlayer, AudioFile audioFile)
        {
            AudioPlayer = audioPlayer;
            AudioFile = audioFile;
        }

        /// <summary>
        /// Gets the Audio Player instance that finished the current audio file.
        /// </summary>
        public AudioPlayer AudioPlayer { get; }

        /// <summary>
        /// Gets the Audio File that finished playing.
        /// </summary>
        public AudioFile AudioFile { get; }
    }
}