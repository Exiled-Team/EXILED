// -----------------------------------------------------------------------
// <copyright file="AudioStream.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Audio
{
    /// <summary>
    /// This class is an audio player extension that will allow the use of URLs in the audio player.
    /// </summary>
    public class AudioStream : AudioPlayer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioStream"/> class.
        /// </summary>
        /// <param name="npc">The npc that will be allowed to play audios from urls.</param>
        internal AudioStream(Npc npc)
            : base(npc)
        {
        }
    }
}