// -----------------------------------------------------------------------
// <copyright file="ParsingEventBuffer.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.API.Features.Parsers
{
    using System.Collections.Generic;

    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;

    /// <inheritdoc />
    public class ParsingEventBuffer : IParser
    {
        private readonly LinkedList<ParsingEvent> buffer;

        private LinkedListNode<ParsingEvent> current;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParsingEventBuffer"/> class.
        /// </summary>
        /// <param name="events">The <see cref="LinkedList{T}"/> instance.</param>
        public ParsingEventBuffer(LinkedList<ParsingEvent> events)
        {
            buffer = events;
            current = events.First;
        }

        /// <inheritdoc/>
        public ParsingEvent Current
        {
            get => current?.Value;
        }

        /// <inheritdoc />
        public bool MoveNext()
        {
            current = current.Next;
            return current is not null;
        }

        /// <summary>
        /// Resets the buffer.
        /// </summary>
        public void Reset()
        {
            current = buffer.First;
        }
    }
}