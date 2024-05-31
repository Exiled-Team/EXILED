// -----------------------------------------------------------------------
// <copyright file="PrefabHelper.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Enums;
    using Exiled.API.Features.Attributes;
    using Mirror;
    using UnityEngine;

    /// <summary>
    /// Helper for Prefabs.
    /// </summary>
    public static class PrefabHelper
    {
        private static readonly Dictionary<PrefabType, GameObject> Stored = new();

        /// <summary>
        /// Gets a dictionary of <see cref="PrefabType"/> to <see cref="GameObject"/>.
        /// </summary>
        public static ReadOnlyDictionary<PrefabType, GameObject> PrefabToGameObject => new(Stored);

        /// <summary>
        /// Gets the prefab attribute of a prefab type.
        /// </summary>
        /// <param name="prefabType">The prefab type.</param>
        /// <returns>The <see cref="PrefabAttribute" />.</returns>
        public static PrefabAttribute GetPrefabAttribute(this PrefabType prefabType)
        {
            Type type = prefabType.GetType();
            return type.GetField(Enum.GetName(type, prefabType)).GetCustomAttribute<PrefabAttribute>();
        }

        /// <summary>
        /// Tries to get a <see cref="GameObject"/> of the specified <see cref="PrefabType"/>.
        /// </summary>
        /// <param name="prefabType">The prefab type.</param>
        /// <param name="prefab">The corresponding <see cref="GameObject"/>.</param>
        /// <returns><see langword="true"/> if a <see cref="GameObject"/> of the specified <see cref="PrefabType"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetGameObject(PrefabType prefabType, out GameObject prefab)
        {
            return Stored.TryGetValue(prefabType, out prefab);
        }

        /// <summary>
        /// Spawns a prefab on server.
        /// </summary>
        /// <param name="prefabType">The prefab type.</param>
        /// <param name="position">The position to spawn the prefab.</param>
        /// <param name="rotation">The rotation of the prefab.</param>
        /// <returns>The <see cref="GameObject"/> instantied.</returns>
        public static GameObject Spawn(PrefabType prefabType, Vector3 position = default, Quaternion rotation = default)
        {
            if (!TryGetGameObject(prefabType, out GameObject gameObject))
                return null;
            GameObject newGameObject = UnityEngine.Object.Instantiate(gameObject, position, rotation);
            NetworkServer.Spawn(newGameObject);
            return newGameObject;
        }

        /// <summary>
        /// Loads all prefabs.
        /// </summary>
        internal static void LoadPrefabs()
        {
            Stored.Clear();

            foreach (PrefabType prefabType in Enum.GetValues(typeof(PrefabType)))
            {
                PrefabAttribute attribute = prefabType.GetPrefabAttribute();
                Stored.Add(prefabType, NetworkClient.prefabs.FirstOrDefault(prefab => prefab.Key == attribute.AssetId || prefab.Value.name.Contains(attribute.Name)).Value);
            }
        }
    }
}