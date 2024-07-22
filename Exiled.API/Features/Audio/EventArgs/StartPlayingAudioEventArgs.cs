// -----------------------------------------------------------------------
// <copyright file="StartPlayingAudioEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Audio.EventArgs
{
    using System;

    /// <summary>
    /// Contains all information before starting to play an audio file.
    /// </summary>
    public class StartPlayingAudioEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartPlayingAudioEventArgs"/> class.
        /// </summary>
        /// <param name="audioPlayer"><see cref="AudioPlayer"/>.</param>
        /// <param name="audioFile"><see cref="AudioFile"/>.</param>
        /// <param name="isAllowed"><see cref="IsAllowed"/>.</param>
        public StartPlayingAudioEventArgs(AudioPlayer audioPlayer, AudioFile audioFile, bool isAllowed = true)
        {
            AudioPlayer = audioPlayer;
            AudioFile = audioFile;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the Audio Player instance that is going to play the audio file.
        /// </summary>
        public AudioPlayer AudioPlayer { get; }

        /// <summary>
        /// Gets the Audio File that is going to be played by the audio player instance.
        /// </summary>
        public AudioFile AudioFile { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the audio file can be played or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}