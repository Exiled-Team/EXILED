// -----------------------------------------------------------------------
// <copyright file="Hint.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System.ComponentModel;

    using Exiled.API.Features.Core.Attributes;
    using Hints;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Useful class to save hint configs in a cleaner way.
    /// </summary>
    [EClass(assetRegistrySearchable: false, category: nameof(Hint))]
    public class Hint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Hint"/> class.
        /// </summary>
        public Hint()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hint"/> class.
        /// </summary>
        /// <param name="content">The content of the hint>.</param>
        /// <param name="duration">The duration of the hint, in seconds.</param>
        /// <param name="show">Whether or not the hint should be shown.</param>
        public Hint(string content, float duration = 3, bool show = true)
        {
            Content = content;
            Duration = duration;
            Show = show;
            Parameters = parameters ?? new HintParameter[] { new StringHintParameter(Content) };
            Effects = effects;
        }

        /// <summary>
        /// Gets or sets the hint content.
        /// </summary>
        [Description("The hint content")]
        [EProperty(registrySearchable: true, category: nameof(Hint))]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the hint duration.
        /// </summary>
        [Description("The hint duration")]
        [EProperty(registrySearchable: true, category: nameof(Hint))]
        public float Duration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the hint should be shown or not.
        /// </summary>
        [Description("Indicates whether the hint should be shown or not")]
        [EProperty(registrySearchable: true, category: nameof(Hint))]
        public bool Show { get; set; }

        /// <summary>
        /// Returns the hint in a human-readable format.
        /// </summary>
        /// <returns>A string containing hint-related data.</returns>
        public override string ToString() => $"({Content}) {Duration}";
    }
}