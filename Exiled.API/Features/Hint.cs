// -----------------------------------------------------------------------
// <copyright file="Hint.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System.ComponentModel;

    using Hints;

    using BaseHint = Hints.Hint;

    /// <summary>
    /// Useful class to save hint in a cleaner way.
    /// </summary>
    public class Hint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Hint"/> class.
        /// </summary>
        /// <param name="baseHint">The hint>.</param>
        public Hint(BaseHint baseHint)
        {
            if (baseHint is TextHint textHint)
            {
                Custom = true;
                Content = textHint.Text;
                Duration = textHint.DurationScalar;
            }
            else if (baseHint is TranslationHint translationhint)
            {
                Custom = false;
                Translations = translationhint.Translation;
                Duration = translationhint.DurationScalar;
            }
        }

        /// <summary>
        /// Gets or sets the broadcast content null if <see cref="Custom"/> is false.
        /// </summary>
        [Description("The hint content")]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the broadcast duration.
        /// </summary>
        [Description("The hint duration")]
        public float Duration { get; set; }

        /// <summary>
        /// Gets the HintTranslations type is null if <see cref="Custom"/> is true>.
        /// </summary>
        public HintTranslations Translations { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the broadcast should be shown or not.
        /// </summary>
        public bool Custom { get; internal set; }

        /// <summary>
        /// Returns the Hint in a human-readable format.
        /// </summary>
        /// <returns>A string containing Hint-related data.</returns>
        public override string ToString() => $"({(Custom ? Content : Translations)}) {Duration} {Custom}";
    }
}