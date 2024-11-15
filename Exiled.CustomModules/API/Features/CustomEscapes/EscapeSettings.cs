// -----------------------------------------------------------------------
// <copyright file="EscapeSettings.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomEscapes
{
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Interfaces;
    using Exiled.CustomModules.API.Features.CustomRoles;
    using PlayerRoles;
    using UnityEngine;

    /// <summary>
    /// A tool to easily setup escapes.
    /// </summary>
    public sealed class EscapeSettings : TypeCastObject<EscapeSettings>, IAdditiveProperty
    {
#pragma warning disable SA1310
        /// <summary>
        /// The default distance tolerance value.
        /// </summary>
        public const float DEFAULT_MAX_DISTANCE_TOLERANCE = 3f;
#pragma warning restore SA1310

        /// <summary>
        /// Gets the default escape position.
        /// </summary>
        public static readonly Vector3 DefaultPosition = Escape.WorldPos;

        /// <summary>
        /// Gets the default <see cref="EscapeSettings"/> values.
        /// </summary>
        public static readonly EscapeSettings Default = new(true);

        /// <summary>
        /// Initializes a new instance of the <see cref="EscapeSettings"/> class.
        /// </summary>
        public EscapeSettings()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EscapeSettings"/> class.
        /// </summary>
        /// <param name="role"><inheritdoc cref="Role"/></param>
        /// <param name="position"><inheritdoc cref="Position"/></param>
        /// <param name="distanceThreshold"><inheritdoc cref="DistanceThreshold"/></param>
        public EscapeSettings(RoleTypeId role, Vector3 position = default, float distanceThreshold = DEFAULT_MAX_DISTANCE_TOLERANCE)
            : this(true, role: role, position: position, distanceThreshold: distanceThreshold)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EscapeSettings"/> class.
        /// </summary>
        /// <param name="customRole"><inheritdoc cref="CustomRole"/></param>
        /// <param name="position"><inheritdoc cref="Position"/></param>
        /// <param name="distanceThreshold"><inheritdoc cref="DistanceThreshold"/></param>
        public EscapeSettings(uint customRole, Vector3 position = default, float distanceThreshold = DEFAULT_MAX_DISTANCE_TOLERANCE)
            : this(true, customRole: customRole, position: position, distanceThreshold: distanceThreshold)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EscapeSettings"/> class.
        /// </summary>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        /// <param name="role"><inheritdoc cref="Role"/></param>
        /// <param name="customRole"><inheritdoc cref="CustomRole"/></param>
        /// <param name="position"><inheritdoc cref="Position"/></param>
        /// <param name="distanceThreshold"><inheritdoc cref="DistanceThreshold"/></param>
        public EscapeSettings(
            bool isAllowed = false,
            RoleTypeId role = RoleTypeId.None,
            uint customRole = 0,
            Vector3 position = default,
            float distanceThreshold = DEFAULT_MAX_DISTANCE_TOLERANCE)
        {
            IsAllowed = isAllowed;
            Role = role;
            CustomRole = CustomRole.Get(customRole);
            Position = position == default ? DefaultPosition : position;
            DistanceThreshold = distanceThreshold;
        }

        /// <summary>
        /// Gets or sets a value indicating whether escaping is allowed.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets or sets the role to be given when escaping.
        /// </summary>
        public RoleTypeId Role { get; set; }

        /// <summary>
        /// Gets or sets the custom role to be given when escaping.
        /// </summary>
        public CustomRole CustomRole { get; set; }

        /// <summary>
        /// Gets or sets the escape position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the maximum allowable distance for applying the specified settings.
        /// </summary>
        public float DistanceThreshold { get; set; }
    }
}