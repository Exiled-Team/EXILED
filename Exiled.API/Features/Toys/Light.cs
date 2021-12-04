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
            Base = Object.Instantiate(ToysHelper.LightBaseObject);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Light"/> class from a <see cref="LightSourceToy"/>.
        /// </summary>
        /// <param name="toy">The toy to be wrapped.</param>
        public Light(LightSourceToy toy)
        {
            Base = toy;
        }

        /// <summary>
        /// Gets the base <see cref="LightSourceToy"/>.
        /// </summary>
        public LightSourceToy Base { get; }

        /// <summary>
        /// Gets or sets the position of the light.
        /// </summary>
        public Vector3 Position
        {
            get => Base.NetworkPosition;
            set
            {
                Base.transform.position = value;
                Base.NetworkPosition = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount of movement smoothening on the light.
        /// You can assign it as a byte (0-255), where higher values mean less smooth movement.
        /// Use 60 for an stable smooth movement => 60 times per second.
        /// </summary>
        public byte Smoothing
        {
            get => Base.NetworkMovementSmoothing;
            set => Base.NetworkMovementSmoothing = value;
        }

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
        /// Gets or sets a value indicating whether the light should cause shadows from other objects.
        /// </summary>
        public bool ShadowEmission
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
        public void UnSpawn()
        {
            NetworkServer.UnSpawn(Base.gameObject);
        }
    }
}
