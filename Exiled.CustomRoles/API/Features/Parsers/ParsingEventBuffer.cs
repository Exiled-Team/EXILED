// -----------------------------------------------------------------------
// <copyright file="ParsingEventBuffer.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.API.Features.Parsers
{
    using System;
    using System.Collections.Generic;

    using Exiled.CustomRoles.API.Features.Interfaces;

    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;

    /// <summary>
    ///     EventBuffer that allows <see cref="AbstractNodeNodeTypeResolver" /> to replay parsing events to multiple
    ///     <see cref="ITypeDiscriminator" />s and to the standard object node deserializer.
    /// </summary>
    public class ParsingEventBuffer : IParser
    {
        private readonly LinkedList<ParsingEvent> buffer;
        private LinkedListNode<ParsingEvent> current;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ParsingEventBuffer" /> class.
        /// </summary>
        /// <param name="events">A <see cref="LinkedList{T}" /> of <see cref="ParsingEvent" />.</param>
        public ParsingEventBuffer(LinkedList<ParsingEvent> events)
        {
            buffer = events;
            current = events.First;
        }

        /// <summary>
        ///     Gets the current <see cref="ParsingEvent" />.
        /// </summary>
        public ParsingEvent Current => current?.Value;

        /// <inheritdoc />
        public bool MoveNext()
        {
            current = current.Next;
            return current is not null;
        }

        /// <summary>
        ///     Resets the buffer to it's original state.
        /// </summary>
        public void Reset()
        {
            current = buffer.First;
        }

        /// <summary>
        ///     Finds the first <see cref="Scalar" /> in the buffer that matches the <paramref name="selector" />.
        /// </summary>
        /// <param name="selector">Predicate to test each element for a condition.</param>
        /// <param name="key"><see cref="Scalar" /> event that matched <paramref name="selector" />.</param>
        /// <param name="value">Next <see cref="ParsingEvent" /> in the buffer.</param>
        /// <returns><see langword="true" /> if a valid mapping entry is found; otherwise <see langword="false" />.</returns>
        public bool TryFindMappingEntry(Func<Scalar, bool> selector, out Scalar key, out ParsingEvent value)
        {
            this.Consume<MappingStart>();
            do
            {
                switch (Current)
                {
                    case Scalar scalar:
                        var keyMatched = selector(scalar);
                        MoveNext();
                        if (keyMatched)
                        {
                            key = scalar;
                            value = Current;
                            return true;
                        }

                        this.SkipThisAndNestedEvents();
                        break;
                    case MappingStart _:
                    case SequenceStart _:
                        this.SkipThisAndNestedEvents();
                        break;
                    default:
                        MoveNext();
                        break;
                }
            }
            while (Current != null);

            key = null;
            value = null;
            return false;
        }
    }
}
