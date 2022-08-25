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

    using UnityEngine;

    /// <summary>
    /// A helper class for interacting with toys.
    /// </summary>
    public static class ToysHelper
    {
        private static PrimitiveObjectToy primitiveBaseObject;
        private static LightSourceToy lightBaseObject;
        private static ShootingTarget sportShootingTargetObject;
        private static ShootingTarget dboyShootingTargetObject;
        private static ShootingTarget binaryShootingTargetObject;

        /// <summary>
        /// Gets the base <see cref="PrimitiveObjectToy"/> to instantiate when creating a new primitive.
        /// </summary>
        public static PrimitiveObjectToy PrimitiveBaseObject
        {
            get
            {
                if (primitiveBaseObject is null)
                {
                    foreach (GameObject gameObject in NetworkClient.prefabs.Values)
                    {
                        if (gameObject.TryGetComponent(out PrimitiveObjectToy component))
                        {
                            primitiveBaseObject = component;
                            break;
                        }
                    }
                }

                return primitiveBaseObject;
            }
        }

        /// <summary>
        /// Gets the base <see cref="LightSourceToy"/> to instantiate when creating a new light.
        /// </summary>
        public static LightSourceToy LightBaseObject
        {
            get
            {
                if (lightBaseObject is null)
                {
                    foreach (GameObject gameObject in NetworkClient.prefabs.Values)
                    {
                        if (gameObject.TryGetComponent(out LightSourceToy component))
                        {
                            lightBaseObject = component;
                            break;
                        }
                    }
                }

                return lightBaseObject;
            }
        }

        /// <summary>
        /// Gets the base <see cref="ShootingTarget"/> to instantiate when creating a new sport shooting target.
        /// </summary>
        public static ShootingTarget SportShootingTargetObject
        {
            get
            {
                if (sportShootingTargetObject is null)
                {
                    foreach (GameObject gameObject in NetworkClient.prefabs.Values)
                    {
                        if ((gameObject.name == "sportTargetPrefab") && gameObject.TryGetComponent(out ShootingTarget shootingTarget))
                        {
                            sportShootingTargetObject = shootingTarget;
                            break;
                        }
                    }
                }

                return sportShootingTargetObject;
            }
        }

        /// <summary>
        /// Gets the base <see cref="ShootingTarget"/> to instantiate when creating a new dboy shooting target.
        /// </summary>
        public static ShootingTarget DboyShootingTargetObject
        {
            get
            {
                if (dboyShootingTargetObject is null)
                {
                    foreach (GameObject gameObject in NetworkClient.prefabs.Values)
                    {
                        if ((gameObject.name == "dboyTargetPrefab") && gameObject.TryGetComponent(out ShootingTarget shootingTarget))
                        {
                            dboyShootingTargetObject = shootingTarget;
                            break;
                        }
                    }
                }

                return dboyShootingTargetObject;
            }
        }

        /// <summary>
        /// Gets the base <see cref="ShootingTarget"/> to instantiate when creating a new binary shooting target.
        /// </summary>
        public static ShootingTarget BinaryShootingTargetObject
        {
            get
            {
                if (binaryShootingTargetObject is null)
                {
                    foreach (GameObject gameObject in NetworkClient.prefabs.Values)
                    {
                        if ((gameObject.name == "binaryTargetPrefab") && gameObject.TryGetComponent(out ShootingTarget shootingTarget))
                        {
                            binaryShootingTargetObject = shootingTarget;
                            break;
                        }
                    }
                }

                return binaryShootingTargetObject;
            }
        }
    }
}