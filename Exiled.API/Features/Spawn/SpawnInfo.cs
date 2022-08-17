// -----------------------------------------------------------------------
// <copyright file="SpawnInfo.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Spawn
{

    using Exiled.API.Extensions;

    using UnityEngine;

    /// <summary>
    /// A struct representing information about a spawn location.
    /// </summary>
    public struct SpawnInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpawnInfo"/> struct.
        /// </summary>
        /// <param name="go">The <see cref="UnityEngine.GameObject"/> of the spawn.</param>
        /// <param name="role">The <see cref="global::RoleType"/> of the spawn.</param>
        internal SpawnInfo(GameObject go, RoleType role)
        {
            RoleType = role;
            GameObject = go;
        }

        /// <summary>
        /// Gets the <see cref="global::RoleType"/> related to this spawn.
        /// </summary>
        public RoleType RoleType { get; }

        /// <summary>
        /// Gets the spawn's <see cref="UnityEngine.GameObject"/>.
        /// </summary>
        public GameObject GameObject { get; }

        /// <summary>
        /// Gets the spawn's <see cref="Vector3">position</see>.
        /// </summary>
        public Vector3 Position => GameObject.transform.position;

        /// <summary>
        /// Gets the spawn's <see cref="Quaternion">rotation</see>.
        /// </summary>
        public Quaternion Rotation => GameObject.transform.rotation;

        /// <summary>
        /// Gets the spawn's <see cref="UnityEngine.Transform"/>.
        /// </summary>
        public Transform Transform => GameObject.transform;
    }
}
