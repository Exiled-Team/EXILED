namespace Exiled.API.Features
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Exiled.API.Enums;
    using Mirror;
    using UnityEngine;

    /// <summary>
    /// Helper for Prefabs.
    /// </summary>
    public static class PrefabHelper
    {
        private static Dictionary<PrefabType, GameObject> stored = new();

        /// <summary>
        /// Gets a dictionary of <see cref="PrefabType"/> to <see cref="GameObject"/>.
        /// </summary>
        public static ReadOnlyDictionary<PrefabType, GameObject> PrefabToGameObject => new(stored);

        /// <summary>
        /// Loads the prefabs.
        /// </summary>
        internal static void LoadPrefabs()
        {
            stored.Clear();

            stored.Add(PrefabType.Workstation, NetworkClient.prefabs.Values.First(x => x.name.Contains("Work Station")));
        }
    }
}