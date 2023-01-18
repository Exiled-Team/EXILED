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

    using Exiled.API.Features;
    using Exiled.API.Interfaces;

    using InventorySystem.Items.Firearms.Attachments;

    using UnityEngine;

    /// <inheritdoc cref="IConfig"/>
    public sealed class Config : IConfig
    {
        /// <inheritdoc/>
        public bool IsEnabled { get; set; } = true;

        /// <inheritdoc />
        public bool Debug { get; set; }

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
        public List<string> StringsList { get; private set; } = new() { "First element", "Second element", "Third element" };

        /// <summary>
        /// Gets the list of ints config.
        /// </summary>
        [Description("This is a list of ints config")]
        public List<int> IntsList { get; private set; } = new() { 1, 2, 3 };

        /// <summary>
        /// Gets the dictionary of string as key and int as value config.
        /// </summary>
        [Description("This is a dictionary of strings as key and int as value config")]
        public Dictionary<string, int> StringIntDictionary { get; private set; } = new()
        {
            { "First Key", 1 },
            { "Second Key", 2 },
            { "Third Key", 3 },
        };

        /// <summary>
        /// Gets the dictionary of string as key and <see cref="Dictionary{TKey, TValue}"/> as value config.
        /// </summary>
        [Description("This is a dictionary of strings as key and Dictionary<string, int> as value config")]
        public Dictionary<string, Dictionary<string, int>> NestedDictionaries { get; private set; } = new()
        {
            {
                "First Key", new Dictionary<string, int>()
                {
                    { "First Key", 1 },
                    { "Second Key", 2 },
                    { "Third Key", 3 },
                }
            },
            {
                "Second Key", new Dictionary<string, int>()
                {
                    { "First Key", 4 },
                    { "Second Key", 5 },
                    { "Third Key", 6 },
                }
            },
            {
                "Third Key", new Dictionary<string, int>()
                {
                    { "First Key", 7 },
                    { "Second Key", 8 },
                    { "Third Key", 9 },
                }
            },
        };

        /// <summary>
        /// Gets the joined broadcast.
        /// </summary>
        [Description("This is an example on how to use the Broadcast class, to send a broadcast to joined players")]
        public Broadcast JoinedBroadcast { get; private set; } = new("Welcome to the server!", 15);

        /// <summary>
        /// Gets the Vector3 config.
        /// </summary>
        [Description("This is a Vector3 config, the same can be done by using a Vector2 or a Vector4")]
        public Vector3 Vector3 { get; private set; } = new(1.3f, -2.5f, 3);

        /// <summary>
        /// Gets the <see cref="List{T}"/> of <see cref="AttachmentName"/> config.
        /// </summary>
        [Description("This is a list of AttachmentNameTranslation config")]
        public List<AttachmentName> Attachments { get; private set; } = new()
        {
            AttachmentName.AmmoCounter,
            AttachmentName.DotSight,
            AttachmentName.RifleBody,
            AttachmentName.RecoilReducingStock,
            AttachmentName.StandardMagAP,
        };
    }
}