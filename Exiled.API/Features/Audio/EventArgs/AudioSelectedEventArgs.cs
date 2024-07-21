// -----------------------------------------------------------------------
// <copyright file="AudioSelectedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Audio.EventArgs
{
    using System;

    /// <summary>
    /// Contains all information after selecting an audio file.
    /// </summary>
    public class AudioSelectedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioSelectedEventArgs"/> class.
        /// </summary>
        /// <param name="audioPlayer"><see cref="AudioPlayer"/>.</param>
        /// <param name="audioFile"><see cref="AudioFile"/>.</param>
        public AudioSelectedEventArgs(AudioPlayer audioPlayer, AudioFile audioFile)
        {
            AudioPlayer = audioPlayer;
            AudioFile = audioFile;
        }

        /// <summary>
        /// Gets the Audio Player instance that selected the audio file.
        /// </summary>
        public AudioPlayer AudioPlayer { get; }

        /// <summary>
        /// Gets the Audio File that has been selected by the Audio Player instance.
        /// </summary>
        public AudioFile AudioFile { get; }
    }
}