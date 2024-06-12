// -----------------------------------------------------------------------
// <copyright file="VerifiedPlugin.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Features
{
#pragma warning disable SA1300

    /// <summary>
    /// A class with information about a verified plugin.
    /// </summary>
    internal class VerifiedPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VerifiedPlugin"/> class.
        /// </summary>
        public VerifiedPlugin()
        {
        }

        /// <summary>
        /// Gets or sets the plugin id.
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Gets or sets the plugin name.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Gets or sets the plugin author.
        /// </summary>
        public string author { get; set; }

        /// <summary>
        /// Gets or sets the plugin description.
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// Gets or sets the URL to plugin repository.
        /// </summary>
        public string repository { get; set; }

        /// <summary>
        /// Gets or sets name of the file that we should download.
        /// </summary>
        public string fileName { get; set; }

        /// <summary>
        /// Returns a verified plugin in a human-readable format.
        /// </summary>
        /// <returns>Human-readable string.</returns>
        public override string ToString() => $"Plugin ID: {id}\nPlugin Name: {name}\nPlugin Author: {author}\nPlugin Description: {description}\n";
    }
}