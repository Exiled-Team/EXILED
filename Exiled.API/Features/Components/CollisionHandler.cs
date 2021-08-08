// -----------------------------------------------------------------------
// <copyright file="CollisionHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Components
{/*
    using System;

    using Exiled.API.Features;

    using Grenades;

    using UnityEngine;

    /// <summary>
    /// Collision Handler for grenades.
    /// </summary>
    public class CollisionHandler : MonoBehaviour
    {
        /// <summary>
        /// Gets the thrower of the grenade.
        /// </summary>
        public GameObject Owner { get; private set; }

        /// <summary>
        /// Gets the grenade itself.
        /// </summary>
        public Grenade Grenade { get; private set; }

        /// <summary>
        /// Inits the <see cref="CollisionHandler"/> object.
        /// </summary>
        /// <param name="owner">The grenade owner.</param>
        /// <param name="grenade">The grenade component.</param>
        public void Init(GameObject owner, Grenade grenade)
        {
            Owner = owner;
            Grenade = grenade;
        }

        private void OnCollisionEnter(Collision collision)
        {
            try
            {
                if (collision.gameObject == Owner || collision.gameObject.TryGetComponent<Grenade>(out _))
                    return;

                Grenade.NetworkfuseTime = 0.1f;
            }
            catch (Exception exception)
            {
                Log.Error($"{nameof(OnCollisionEnter)} error:\n{exception}");
            }
        }
    }*/
}
