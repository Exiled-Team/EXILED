// -----------------------------------------------------------------------
// <copyright file="Light.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Toys
{
    using AdminToys;
    using Exiled.API.Enums;
    using UnityEngine;

    /// <summary>
    /// A wrapper class for <see cref="AdminToys.LightSourceToy"/>.
    /// </summary>
    public class Light : AdminToy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Light"/> class.
        /// </summary>
        /// <param name="lightSourceToy">The <see cref="LightSourceToy"/> of the toy.</param>
        public Light(LightSourceToy lightSourceToy)
            : base(lightSourceToy, AdminToyType.LightSource)
        {
            LightSourceBase = lightSourceToy;
        }

        /// <summary>
        /// Gets the base <see cref="AdminToys.LightSourceToy"/>.
        /// </summary>
        public LightSourceToy LightSourceBase { get; }

        /// <summary>
        /// Gets or sets the intensity of the light.
        /// </summary>
        public float Intensity
        {
            get => LightSourceBase.NetworkLightIntensity;
            set => LightSourceBase.NetworkLightIntensity = value;
        }

        /// <summary>
        /// Gets or sets the range of the light.
        /// </summary>
        public float Range
        {
            get => LightSourceBase.NetworkLightRange;
            set => LightSourceBase.NetworkLightRange = value;
        }

        /// <summary>
        /// Gets or sets the color of the primitive.
        /// </summary>
        public Color Color
        {
            get => LightSourceBase.NetworkLightColor;
            set => LightSourceBase.NetworkLightColor = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the light should cause shadows from other objects.
        /// </summary>
        public bool ShadowEmission
        {
            get => LightSourceBase.NetworkLightShadows;
            set => LightSourceBase.NetworkLightShadows = value;
        }

        /// <summary>
        /// Creates a new <see cref="Light"/>.
        /// </summary>
        /// <returns>The new light.</returns>
        public static Light Create() => new Light(Object.Instantiate(ToysHelper.LightBaseObject));
    }
}
