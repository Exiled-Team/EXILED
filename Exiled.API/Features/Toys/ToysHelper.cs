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
        private static PrimitiveObjectToy baseObject = null;

        /// <summary>
        /// Gets the base AdminToy object to instantiate when creating a new toy.
        /// </summary>
        public static PrimitiveObjectToy BaseObject
        {
            get
            {
                if (baseObject == null)
                {
                    foreach (var gameObject in NetworkClient.prefabs.Values)
                    {
                        if (gameObject.TryGetComponent<PrimitiveObjectToy>(out var component))
                        {
                            baseObject = component;
                        }
                    }
                }

                return baseObject;
            }
        }
    }
}
