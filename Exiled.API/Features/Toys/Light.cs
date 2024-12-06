// -----------------------------------------------------------------------
// <copyright file="Light.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Toys
{
    using System.Linq;

    using AdminToys;

    using Enums;
    using Exiled.API.Interfaces;
    using Exiled.API.Structs;
    using UnityEngine;

    /// <summary>
    /// A wrapper class for <see cref="LightSourceToy"/>.
    /// </summary>
    public class Light : AdminToy, IWrapper<LightSourceToy>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Light"/> class.
        /// </summary>
        /// <param name="lightSourceToy">The <see cref="LightSourceToy"/> of the toy.</param>
        internal Light(LightSourceToy lightSourceToy)
            : base(lightSourceToy, AdminToyType.LightSource) => Base = lightSourceToy;

        /// <summary>
        /// Gets the light prefab's type.
        /// </summary>
        public static PrefabType PrefabType => PrefabType.LightSourceToy;

        /// <summary>
        /// Gets the light prefab's object.
        /// </summary>
        public static GameObject PrefabObject => PrefabHelper.PrefabToGameObject[PrefabType];

        /// <summary>
        /// Gets the base <see cref="LightSourceToy"/>.
        /// </summary>
        public LightSourceToy Base { get; }

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
        /// Gets or sets the color of the light.
        /// </summary>
        public Color Color
        {
            get => Base.NetworkLightColor;
            set => Base.NetworkLightColor = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the light should cause shadows from other objects.
        /// </summary>
        public LightShadows Shadow
        {
            get => Base.NetworkShadowType;
            set => Base.NetworkShadowType = value;
        }

        /// <summary>
        /// Gets or sets the light shape.
        /// </summary>
        public LightShape Shape
        {
            get => Base.NetworkLightShape;
            set => Base.NetworkLightShape = value;
        }

        /// <summary>
        /// Gets or sets the shadow strength.
        /// </summary>
        public float ShadowStrength
        {
            get => Base.NetworkShadowStrength;
            set => Base.NetworkShadowStrength = value;
        }

        /// <summary>
        /// Gets or sets the Light Type of this light.
        /// </summary>
        public LightType LightType
        {
            get => Base.NetworkLightType;
            set => Base.NetworkLightType = value;
        }

        /// <summary>
        /// Gets or sets the spot angle of this light.
        /// </summary>
        public float SpotAngle
        {
            get => Base.NetworkSpotAngle;
            set => Base.NetworkSpotAngle = value;
        }

        /// <summary>
        /// Gets or sets the inner spot angle of this light.
        /// </summary>
        public float InnerSpotAngle
        {
            get => Base.NetworkInnerSpotAngle;
            set => Base.NetworkInnerSpotAngle = value;
        }

        /// <summary>
        /// Creates a new <see cref="Light"/>.
        /// </summary>
        /// <param name="position">The position of the <see cref="Light"/>.</param>
        /// <param name="rotation">The rotation of the <see cref="Light"/>.</param>
        /// <param name="scale">The scale of the <see cref="Light"/>.</param>
        /// <param name="color">The color of the <see cref="Light"/>.</param>
        /// <param name="spawn">Whether or not the <see cref="Light"/> should be initially spawned.</param>
        /// <returns>The newly created <see cref="Light"/>.</returns>
        public static Light Create(Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null, Color? color = null, bool spawn = true)
            => Create(new(position, rotation, scale, color, spawn));

        /// <summary>
        /// Creates a new <see cref="Light"/>.
        /// </summary>
        /// <param name="lightSettings">The settings of the <see cref="Light"/>.</param>
        /// <returns>The new <see cref="Light"/>.</returns>
        public static Light Create(LightSettings lightSettings)
        {
            Light light = new(Object.Instantiate(PrefabObject.GetComponent<LightSourceToy>()))
            {
                Position = lightSettings.Position,
                Rotation = lightSettings.Rotation,
                Scale = lightSettings.Scale,
                Color = lightSettings.Color,
            };

            if (lightSettings.ShouldSpawn)
                light.Spawn();

            return light;
        }

        /// <summary>
        /// Gets the <see cref="Light"/> belonging to the <see cref="LightSourceToy"/>.
        /// </summary>
        /// <param name="lightSourceToy">The <see cref="LightSourceToy"/> instance.</param>
        /// <returns>The corresponding <see cref="LightSourceToy"/> instance.</returns>
        public static Light Get(LightSourceToy lightSourceToy)
        {
            AdminToy adminToy = Map.Toys.FirstOrDefault(x => x.AdminToyBase == lightSourceToy);
            return adminToy is not null ? adminToy as Light : new Light(lightSourceToy);
        }
    }
}