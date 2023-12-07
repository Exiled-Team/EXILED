// -----------------------------------------------------------------------
// <copyright file="ParserExtension.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.Extensions
{
    using System;

    using Exiled.CustomModules.API.Features.Parsers;

    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;

    /// <summary>
    /// Extensions for <see cref="ParsingEventBuffer"/>.
    /// </summary>
    public static class ParserExtension
    {
        /// <summary>
        /// Tries to find a valid mapping entry.
        /// </summary>
        /// <param name="parser">The <see cref="ParsingEventBuffer"/> parser.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="key">The key found.</param>
        /// <param name="value">The value found.</param>
        /// <returns><see langword="true"/> when a valid mapping entry is found; otherwise, <see langword="false"/>.</returns>
        public static bool TryFindMappingEntry(this ParsingEventBuffer parser, Func<Scalar, bool> selector, out Scalar? key, out ParsingEvent? value)
        {
            parser.Consume<MappingStart>();
            do
            {
                switch (parser.Current)
                {
                    case Scalar scalar:
                        bool keyMatched = selector(scalar);
                        parser.MoveNext();
                        if (keyMatched)
                        {
                            value = parser.Current;
                            key = scalar;
                            return true;
                        }

                        parser.SkipThisAndNestedEvents();
                        break;
                    case MappingStart _:
                    case SequenceStart _:
                        parser.SkipThisAndNestedEvents();
                        break;
                    default:
                        parser.MoveNext();
                        break;
                }
            }
            while (parser.Current is not null);

            key = null;
            value = null;
            return false;
        }
    }
}