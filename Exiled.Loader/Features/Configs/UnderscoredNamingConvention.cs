// -----------------------------------------------------------------------
// <copyright file="UnderscoredNamingConvention.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Loader.Features.Configs
{
    using System.Collections.Generic;

    using Exiled.API.Extensions;
    using YamlDotNet.Serialization;

    /// <inheritdoc cref="YamlDotNet.Serialization.NamingConventions.UnderscoredNamingConvention"/>
    public class UnderscoredNamingConvention : INamingConvention
    {
        /// <inheritdoc cref="YamlDotNet.Serialization.NamingConventions.UnderscoredNamingConvention.Instance"/>
        public static UnderscoredNamingConvention Instance { get; } = new();

        /// <summary>
        /// Gets the list.
        /// </summary>
        public List<object> Properties { get; } = new();

        /// <inheritdoc/>
        public string Apply(string value)
        {
            string newValue = value.ToSnakeCase();
            Properties.Add(newValue);
            return newValue;
        }
    }
}