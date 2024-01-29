// -----------------------------------------------------------------------
// <copyright file="SendingCassieMessageEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Cassie
{
    using Interfaces;

    /// <summary>
    /// Contains all the information after sending a C.A.S.S.I.E. message.
    /// </summary>
    public class SendingCassieMessageEventArgs : IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendingCassieMessageEventArgs" /> class.
        /// </summary>
        /// <param name="words">
        /// <inheritdoc cref="Words" />
        /// </param>
        /// <param name="makeHold">
        /// <inheritdoc cref="MakeHold" />
        /// </param>
        /// <param name="makeNoise">
        /// <inheritdoc cref="MakeNoise" />
        /// </param>
        /// <param name="isAllowed">Indicates whether the event can be executed or not.</param>
        public SendingCassieMessageEventArgs(string words, bool makeHold, bool makeNoise, bool isAllowed = true)
        {
            Words = words;
            MakeHold = makeHold;
            MakeNoise = makeNoise;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Words { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the message should be held.
        /// </summary>
        public bool MakeHold { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the message should make noise.
        /// </summary>
        public bool MakeNoise { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the message can be sent.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}