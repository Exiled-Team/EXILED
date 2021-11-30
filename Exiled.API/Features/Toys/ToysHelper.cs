// -----------------------------------------------------------------------
// <copyright file="ToysHelper.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Toys
{
    using AdminToys;
    using Mirror;

    /// <summary>
    /// A helper class for interacting with toys.
    /// </summary>
    public class ToysHelper
    {
        private static PrimitiveObjectToy primitiveBaseObject = null;
        private static LightSourceToy lightBaseObject = null;

        /// <summary>
        /// Gets the base AdminToy object to instantiate when creating a new primitive.
        /// </summary>
        public static PrimitiveObjectToy PrimitiveBaseObject
        {
            get
            {
                if (primitiveBaseObject == null)
                {
                    foreach (var gameObject in NetworkClient.prefabs.Values)
                    {
                        if (gameObject.TryGetComponent<PrimitiveObjectToy>(out var component))
                        {
                            primitiveBaseObject = component;
                        }
                    }
                }

                return primitiveBaseObject;
            }
        }

        /// <summary>
        /// Gets the base AdminToy object to instantiate when creating a new light.
        /// </summary>
        public static LightSourceToy LightBaseObject
        {
            get
            {
                if (lightBaseObject == null)
                {
                    foreach (var gameObject in NetworkClient.prefabs.Values)
                    {
                        if (gameObject.TryGetComponent<LightSourceToy>(out var component))
                        {
                            lightBaseObject = component;
                        }
                    }
                }

                return lightBaseObject;
            }
        }
    }
}
