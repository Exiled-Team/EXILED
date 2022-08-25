// -----------------------------------------------------------------------
// <copyright file="Scp106Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using UnityEngine;

    /// <summary>
    /// Defines a role that represents SCP-106.
    /// </summary>
    public class Scp106Role : Role
    {
        private Scp106PlayerScript script;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp106Role"/> class.
        /// </summary>
        /// <param name="player">The encapsulated player.</param>
        internal Scp106Role(Player player) => Owner = player;

        /// <inheritdoc/>
        public override Player Owner { get; }

        /// <summary>
        /// Gets the <see cref="Scp106PlayerScript"/> script for the role.
        /// </summary>
        public Scp106PlayerScript Script => script ??= Owner.ReferenceHub.scp106PlayerScript;

        /// <summary>
        /// Gets a value indicating whether or not SCP-106 is currently inside of an object.
        /// </summary>
        public bool IsInsideObject => Script.ObjectCurrentlyIn is not null;

        /// <summary>
        /// Gets a value indicating whether or not SCP-106 is currently inside of a door.
        /// </summary>
        public bool IsInsideDoor => Script.DoorCurrentlyIn is not null;

        /// <summary>
        /// Gets the door that SCP-106 is currently inside of.
        /// </summary>
        public Door InsideDoor => Door.Get(Script.DoorCurrentlyIn);

        /// <summary>
        /// Gets or sets the location of SCP-106's portal.
        /// </summary>
        /// <remarks>
        /// Note: Every alive SCP-106 uses the same portal.
        /// </remarks>
        public Vector3 PortalPosition
        {
            get => Script.portalPosition;
            set => Script.SetPortalPosition(PortalPosition, value);
        }

        /// <summary>
        /// Gets or sets the amount of time in between player captures.
        /// </summary>
        public float CaptureCooldown
        {
            get => Script.captureCooldown;
            set => Script.captureCooldown = value;
        }

        /// <inheritdoc/>
        internal override RoleType RoleType => RoleType.Scp106;

        /// <summary>
        /// Forces SCP-106 to use its portal, if one is placed.
        /// </summary>
        public void UsePortal() => Script.UserCode_CmdUsePortal();

        /// <summary>
        /// Contains SCP-106.
        /// </summary>
        /// <param name="container">The player who recontained SCP-106.</param>
        /// <exception cref="System.ArgumentException">Container cannot be <see langword="null"/>.</exception>
        public void Contain(Player container)
        {
            if (container is null)
                throw new System.ArgumentException("Container cannot be null.", nameof(container));

            Script.Contain(container.Footprint);
        }
    }
}
