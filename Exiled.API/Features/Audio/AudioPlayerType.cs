// -----------------------------------------------------------------------
// <copyright file="AudioPlayerType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Audio
{
    /// <summary>
    /// Unique identifier for the different modes of audio playing.
    /// </summary>
    /// <seealso cref="AudioPlayer.PlayerType"/>
    public enum AudioPlayerType
    {
        /// <summary>
        /// The audio player is not playing anything.
        /// </summary>
        None,

        /// <summary>
        /// The audio player is playing audios from files.
        /// </summary>
        File,

        /// <summary>
        /// The audio player is playing audios from URLs.
        /// </summary>
        URL,
    }
}