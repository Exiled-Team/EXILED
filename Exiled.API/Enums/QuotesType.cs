// -----------------------------------------------------------------------
// <copyright file="QuotesType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// String values in config quotes type.
    /// </summary>
    public enum QuotesType
    {
        /// <summary>
        /// String won't be wrapped in quotes.
        /// </summary>
        NoQuotes,

        /// <summary>
        /// String will be wrapped in single quotes.
        /// </summary>
        SingleQuoted,

        /// <summary>
        /// String will be wrapped in double quotes.
        /// </summary>
        DoubleQuoted,
    }
}