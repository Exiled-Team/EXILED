// -----------------------------------------------------------------------
// <copyright file="Scp106Role.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.API.Features.Roles
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
        internal Scp106Role(Player player)
        {
            Owner = player;
            script = player.ReferenceHub.scp106PlayerScript;
        }

        /// <inheritdoc/>
        public override Player Owner { get; }

        /// <summary>
        /// Gets a value indicating whether or not SCP-106 is currently inside of an object.
        /// </summary>
        public bool IsInsideObject => script.ObjectCurrentlyIn != null;

        /// <summary>
        /// Gets a value indicating whether or not SCP-106 is currently inside of a door.
        /// </summary>
        public bool IsInsideDoor => script.DoorCurrentlyIn != null;

        /// <summary>
        /// Gets the door that SCP-106 is currently inside of.
        /// </summary>
        public Door InsideDoor => Door.Get(script.DoorCurrentlyIn);

        /// <summary>
        /// Gets or sets the location of SCP-106's portal.
        /// </summary>
        /// <remarks>
        /// Note: Every alive SCP-106 uses the same portal.
        /// </remarks>
        public Vector3 PortalPosition
        {
            get => script.portalPosition;
            set => script.SetPortalPosition(PortalPosition, value);
        }

        /// <summary>
        /// Gets or sets the amount of time in between player captures.
        /// </summary>
        public float CaptureCooldown
        {
            get => script.captureCooldown;
            set => script.captureCooldown = value;
        }

        /// <inheritdoc/>
        internal override RoleType RoleType => RoleType.Scp106;

        /// <summary>
        /// Forces SCP-106 to use its portal, if one is placed.
        /// </summary>
        public void UsePortal() => script.UserCode_CmdUsePortal();

        /// <summary>
        /// Contains SCP-106.
        /// </summary>
        /// <param name="container">The player who recontained SCP-106.</param>
        public void Contain(Player container) => script.Contain(container.Footprint);
    }
}
