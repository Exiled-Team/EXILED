// -----------------------------------------------------------------------
// <copyright file="Group.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Permissions.Features
{
    using System.Collections.Generic;

    using System.Linq;

    using Exiled.API.Features;

    using YamlDotNet.Serialization;

    /// <summary>
    /// Represents a player's group.
    /// </summary>
    public class Group
    {
        /// <summary>
        /// Private field for the combined permissions of the group and inherited groups.
        /// </summary>
        private IEnumerable<string> inheritedPermissions = null;

        /// <summary>
        /// Gets or sets a value indicating whether group is the default one or not.
        /// </summary>
        [YamlMember(Alias = "default")]
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets the group inheritance.
        /// </summary>
        public List<string> Inheritance { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the group permissions.
        /// </summary>
        public List<string> Permissions { get; set; } = new List<string>();

        /// <summary>
        /// Gets the combined permissions of the group plus all inherited groups.
        /// </summary>
        [YamlIgnore]
        public List<string> CombinedPermissions
        {
            get
            {
                if (inheritedPermissions != null)
                    return Permissions.Union(inheritedPermissions).ToList();

                Log.Info("Generating inherited permissions field.");
                IEnumerable<string> inheritedPerms = new List<string>();
                inheritedPerms = Extensions.Permissions.Groups.Where(pair => Inheritance.Contains(pair.Key)).Aggregate(inheritedPerms, (current, pair) => current.Union(pair.Value.Permissions));

                inheritedPermissions = inheritedPerms.ToList();

                return Permissions.Union(inheritedPermissions).ToList();
            }
        }
    }
}
