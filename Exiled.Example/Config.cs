// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using Exiled.API.Interfaces;

    /// <inheritdoc cref="IConfig"/>
    public sealed class Config : IConfig
    {
        /// <inheritdoc/>
        [Description("Indicates whether the plugin is enabled or not")]
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets the string config.
        /// </summary>
        [Description("This is a string config")]
        public string String { get; private set; } = "I'm a string!";

        /// <summary>
        /// Gets the int config.
        /// </summary>
        [Description("This is an int config")]
        public int Int { get; private set; } = 1000;

        /// <summary>
        /// Gets the float config.
        /// </summary>
        [Description("This is a float config")]
        public float Float { get; private set; } = 28.2f;

        /// <summary>
        /// Gets the list of strings config.
        /// </summary>
        [Description("This is a list of strings config")]
        public List<string> StringsList { get; private set; } = new List<string>() { "First element", "Second element", "Third element" };

        /// <summary>
        /// Gets the list of ints config.
        /// </summary>
        [Description("This is a list of ints config")]
        public List<int> IntsList { get; private set; } = new List<int>() { 1, 2, 3 };

        /// <summary>
        /// Gets the dictionary of string as key and int as value config.
        /// </summary>
        [Description("This is a dictionary of string as key and int as value config")]
        public Dictionary<string, int> StringIntDictionary { get; private set; } = new Dictionary<string, int>()
        {
            { "First Key", 1 },
            { "Second Key", 2 },
            { "Third Key", 3 },
        };
    }
}
