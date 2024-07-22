// -----------------------------------------------------------------------
// <copyright file="AudioFile.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Audio
{
    using System.ComponentModel;

    using VoiceChat;

    /// <summary>
    /// Represents an audio file that can be used by the <see cref="AudioPlayer"/>.
    /// </summary>
    public class AudioFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioFile"/> class.
        /// </summary>
        public AudioFile()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioFile"/> class.
        /// </summary>
        /// <param name="filePath"><see cref="FilePath"/>.</param>
        /// <param name="enabled"><see cref="Enabled"/>.</param>
        /// <param name="loop"><see cref="Loop"/>.</param>
        /// <param name="volume"><see cref="Volume"/>.</param>
        /// <param name="channel"><see cref="Channel"/>.</param>
        public AudioFile(string filePath, bool enabled = true, bool loop = false, int volume = 100, VoiceChatChannel channel = VoiceChatChannel.Intercom)
        {
            FilePath = filePath;
            Enabled = enabled;
            Loop = loop;
            Volume = volume;
            Channel = channel;
        }

        /// <summary>
        /// Gets or sets the path to the audio file.
        /// </summary>
        [Description("Indicates the location of the audio file.")]
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this audio file is enabled or not.
        /// </summary>
        [Description("Indicates if the audio file is enabled or not.")]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether if the audio that is being played should be looped or not.
        /// </summary>
        [Description("Indicates if the audio file that is being played should be looped or not.")]
        public bool Loop { get; set; }

        /// <summary>
        /// Gets or sets value the volume of the audio.
        /// </summary>
        [Description("The volume of the audio file.")]
        public int Volume { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="VoiceChatChannel"/> of the <see cref="AudioPlayer"/>.
        /// </summary>
        [Description("The Voice Channel of the AudioPlayer.")]
        public VoiceChatChannel Channel { get; set; }
    }
}