// -----------------------------------------------------------------------
// <copyright file="AudioPlayer.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Audio
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Audio.EventArgs;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.DynamicEvents;
    using MEC;
    using Mirror;

    /// <summary>
    /// Represents a NPC that can play audios.
    /// </summary>
    public class AudioPlayer : GameEntity // TODO: Stop Handle & Update Methods
    {
        private CoroutineHandle playbackHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioPlayer"/> class.
        /// </summary>
        /// <param name="npc">The npc that will be able to play audio.</param>
        private AudioPlayer(Npc npc)
            : base(npc.GameObject)
        {
            Owner = npc;
            Dictionary.Add(npc, this);
        }

        /// <summary>
        /// Gets all the audio players present in the server in the form of dictionary.
        /// </summary>
        public static Dictionary<Npc, AudioPlayer> Dictionary { get; } = new();

        /// <summary>
        /// Gets all the audio players present in the server in the form of list.
        /// </summary>
        public static new IEnumerable<AudioPlayer> List => Dictionary.Values.ToList();

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all delegates to be fired before selecting an audio file.
        /// </summary>
        [DynamicEventDispatcher]
        public static TDynamicEventDispatcher<SelectingAudioEventArgs> SelectingAudio { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all delegates to be fired after selecting an audio file.
        /// </summary>
        [DynamicEventDispatcher]
        public static TDynamicEventDispatcher<AudioSelectedEventArgs> AudioSelected { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handlers all delegates to be fired after finishing and audio file.
        /// </summary>
        [DynamicEventDispatcher]
        public static TDynamicEventDispatcher<AudioFinishedEventArgs> AudioFinished { get; set; }

        /// <summary>
        /// Gets the NPC linked to this <see cref="AudioPlayer"/>.
        /// </summary>
        public Npc Owner { get; }

        /// <summary>
        /// Gets the current audio queue of the Audio player instance.
        /// </summary>
        public List<AudioFile> AudioQueue { get; } = new();

        /// <summary>
        /// Gets the audio file that is being player.
        /// </summary>
        public AudioFile CurrentAudio { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the audio player instance should be destroyed when finishing the current audio.
        /// </summary>
        public bool DestroyWhenFinishing { get; set; } = false;

        /// <summary>
        /// Gets the audio player of the specified npc or creates one for it.
        /// </summary>
        /// <param name="npc">The npc to get the audio player instance of or create one.</param>
        /// <returns>The Audio Player instance of the npc.</returns>
        public static AudioPlayer GetOrCreate(Npc npc)
        {
            return Dictionary.TryGetValue(npc, out AudioPlayer player) ? player : new AudioPlayer(npc);
        }

        /// <summary>
        /// Checks if the audio file is .ogg.
        /// </summary>
        /// <param name="audioFile">The <see cref="AudioFile"/>to check.</param>
        /// <returns>If the audio file is .ogg or not.</returns>
        public bool IsOggFile(AudioFile audioFile)
        {
            if (!File.Exists(audioFile.FilePath))
                return false;

            return Path.GetExtension(audioFile.FilePath) == ".ogg";
        }

        /// <summary>
        /// Checks if the audio file is .ogg.
        /// </summary>
        /// <param name="audioFile">The Audio File to check.</param>
        /// <returns>If the audio file is .ogg or not.</returns>
        public bool IsOggFile(string audioFile)
        {
            if (!File.Exists(audioFile))
                return false;

            return Path.GetExtension(audioFile) == ".ogg";
        }

        /// <summary>
        /// Enqueue an audio file.
        /// </summary>
        /// <param name="audioFile">The audio file to enqueue.</param>
        public void Enqueue(AudioFile audioFile)
        {
            if (audioFile == null)
                return;

            if (!audioFile.Enabled || !IsOggFile(audioFile))
                return;

            AudioQueue.Add(audioFile);
        }

        /// <summary>
        /// Enqueue an audio file.
        /// </summary>
        /// <param name="pathToFile">The audio file to enqueue.</param>
        public void Enqueue(string pathToFile)
        {
            if (string.IsNullOrEmpty(pathToFile))
                return;

            if (!IsOggFile(pathToFile))
                return;

            AudioQueue.Add(new AudioFile(pathToFile));
        }

        /// <summary>
        /// Plays a specific audio from the queue.
        /// </summary>
        /// <param name="queuePosition">The position of the audio in the queue.</param>
        /// <remarks>If the queue position is not specified, the first audio in the queue will be played.</remarks>
        public void Play(int queuePosition = -1)
        {
            if (AudioQueue.Count == 0)
                return;

            CurrentAudio = queuePosition == -1 ? AudioQueue.First() : AudioQueue.ElementAt(queuePosition);

            AudioQueue.Remove(CurrentAudio);

            if (playbackHandler.IsRunning)
                return;
        }

        /// <summary>
        /// Stops the audio that is being played.
        /// </summary>
        /// <param name="clearQueue">If the queue should be cleared or not.</param>
        public void Stop(bool clearQueue = false)
        {
            if (clearQueue)
                AudioQueue.Clear();

            // TODO: Handle Audio Stop Here.
        }

        /// <summary>
        /// Destroys the audio instance and the npc.
        /// </summary>
        public void Destroy()
        {
            if (playbackHandler.IsRunning)
                Timing.KillCoroutines(playbackHandler);

            Dictionary.Remove(Owner);

            NetworkServer.RemovePlayerForConnection(Owner.Connection, true);
        }
    }
}