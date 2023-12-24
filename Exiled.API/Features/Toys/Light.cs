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
            : base(lightSourceToy, AdminToyType.LightSource)
        {
            Base = lightSourceToy;
        }

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
        /// Creates a new <see cref="Light"/>.
        /// </summary>
        /// <param name="position">The position of the <see cref="Light"/>.</param>
        /// <param name="rotation">The rotation of the <see cref="Light"/>.</param>
        /// <param name="scale">The scale of the <see cref="Light"/>.</param>
        /// <param name="spawn">Whether the <see cref="Light"/> should be initially spawned.</param>
        /// <returns>The new <see cref="Light"/>.</returns>
        public static Light Create(Vector3? position = null, Vector3? rotation = null, Vector3? scale = null, bool spawn = true)
            => Create(position, rotation, scale, spawn, null);

        /// <summary>
        /// Creates a new <see cref="Light"/>.
        /// </summary>
        /// <param name="position">The position of the <see cref="Light"/>.</param>
        /// <param name="rotation">The rotation of the <see cref="Light"/>.</param>
        /// <param name="scale">The scale of the <see cref="Light"/>.</param>
        /// <param name="spawn">Whether the <see cref="Light"/> should be initially spawned.</param>
        /// <param name="color">The color of the <see cref="Light"/>.</param>
        /// <returns>The new <see cref="Light"/>.</returns>
        public static Light Create(Vector3? position /*= null*/, Vector3? rotation /*= null*/, Vector3? scale /*= null*/, bool spawn /*= true*/, Color? color /*= null*/)
        {
            Light light = new(UnityEngine.Object.Instantiate(ToysHelper.LightBaseObject));

            light.Base.transform.position = position ?? Vector3.zero;
            light.Base.transform.eulerAngles = rotation ?? Vector3.zero;
            light.Base.transform.localScale = scale ?? Vector3.one;

            if (spawn)
                light.Spawn();

            light.Color = color ?? Color.gray;

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