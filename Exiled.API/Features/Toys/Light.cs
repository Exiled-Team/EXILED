// -----------------------------------------------------------------------
// <copyright file="Light.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Toys
{
    using AdminToys;
    using Mirror;
    using UnityEngine;

    /// <summary>
    /// A wrapper class for light source Admin Toys.
    /// </summary>
    public class Light
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Light"/> class.
        /// </summary>
        public Light()
        {
            Base = UnityEngine.Object.Instantiate(ToysHelper.LightBaseObject);
        }

        /// <summary>
        /// Gets the base <see cref="LightSourceToy"/>.
        /// </summary>
        public LightSourceToy Base { get; } = null;

        /// <summary>
        /// Gets or sets the position of the light.
        /// </summary>
        public Vector3 Position { get; set; } = Vector3.zero;

        /// <summary>
        /// Gets or sets the intensity of the light.
        /// </summary>
        public float Intensity
        {
            get => Base.NetworkLightIntensity;
            set => Base.NetworkLightIntensity = value;
        }

        /// <summary>
        /// Gets or sets the range of the light.
        /// </summary>
        public float Range
        {
            get => Base.NetworkLightRange;
            set => Base.NetworkLightRange = value;
        }

        /// <summary>
        /// Gets or sets the color of the primitive.
        /// </summary>
        public Color Color
        {
            get => Base.NetworkLightColor;
            set => Base.NetworkLightColor = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the light should have shadows.
        /// </summary>
        public bool NetworkLightShadows
        {
            get => Base.NetworkLightShadows;
            set => Base.NetworkLightShadows = value;
        }

        /// <summary>
        /// Spawns the light.
        /// </summary>
        public void Spawn()
        {
            var transform = Base.transform;
            transform.position = Position;
            transform.localScale = Vector3.one;

            NetworkServer.Spawn(Base.gameObject);
        }

        /// <summary>
        /// Removes the light from the game. Use <see cref="Spawn"/> to bring it back.
        /// </summary>
        public void Destroy()
        {
            NetworkServer.UnSpawn(Base.gameObject);
        }
    }
}
