// -----------------------------------------------------------------------
// <copyright file="AssetRegistry.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a registry for managing entity assets.
    /// </summary>
    /// <remarks>
    /// This class allows for storing and accessing entity assets, providing methods to retrieve assets by various criteria.
    /// </remarks>
    public sealed class AssetRegistry
    {
#pragma warning disable SA1310
        /// <summary>
        /// The default category name.
        /// </summary>
        public const string DEFAULT_CATEGORY = "default_category";

        /// <summary>
        /// The default exact name.
        /// </summary>
        public const string DEFAULT_EXACT_NAME = "default_name";

        /// <summary>
        /// The default asset name.
        /// </summary>
        public const string DEFAULT_ASSET_NAME = "default_asset";
#pragma warning restore SA1310

        private readonly List<EntityAsset> assets = new();

        /// <summary>
        /// Gets all entity assets stored in the registry.
        /// </summary>
        public IEnumerable<EntityAsset> Assets => assets;

        /// <summary>
        /// Gets all entity assets stored in the registry with the specified identifier and category.
        /// </summary>
        /// <typeparam name="T">The type of entity asset to retrieve.</typeparam>
        /// <param name="identifier">The identifier of the asset.</param>
        /// <param name="category">The category of the asset.</param>
        /// <returns>All entity assets matching the specified identifier and category.</returns>
        public IEnumerable<T> GetAllFromRegistry<T>(string identifier = DEFAULT_ASSET_NAME, string category = DEFAULT_CATEGORY)
        {
            foreach (EntityAsset entityAsset in Assets)
            {
                if ((!string.IsNullOrEmpty(category) && entityAsset.Class.Category != category) ||
                    entityAsset.Name != identifier)
                {
                    continue;
                }

                if (typeof(T) == typeof(EntityAsset))
                    yield return (T)(object)entityAsset;
                else if (entityAsset.Class.Entity is T comp)
                    yield return comp;
            }
        }

        /// <summary>
        /// Gets the first entity asset stored in the registry with the specified identifier and category.
        /// </summary>
        /// <typeparam name="T">The type of entity asset to retrieve.</typeparam>
        /// <param name="identifier">The identifier of the asset.</param>
        /// <param name="category">The category of the asset.</param>
        /// <returns>The first entity asset matching the specified identifier and category.</returns>
        public T GetFirstFromRegistry<T>(string identifier = DEFAULT_ASSET_NAME, string category = DEFAULT_CATEGORY)
        {
            foreach (EntityAsset entityAsset in Assets)
            {
                if ((!string.IsNullOrEmpty(category) && entityAsset.Class.Category != category) ||
                    entityAsset.Name != identifier)
                {
                    continue;
                }

                if (typeof(T) == typeof(EntityAsset))
                    return (T)(object)entityAsset;

                if (entityAsset.Class.Entity is T comp)
                    return comp;
            }

            return default;
        }

        /// <summary>
        /// Gets the last entity asset stored in the registry with the specified identifier and category.
        /// </summary>
        /// <typeparam name="T">The type of entity asset to retrieve.</typeparam>
        /// <param name="identifier">The identifier of the asset.</param>
        /// <param name="category">The category of the asset.</param>
        /// <returns>The last entity asset matching the specified identifier and category.</returns>
        public T GetLastFromRegistry<T>(string identifier = DEFAULT_ASSET_NAME, string category = DEFAULT_CATEGORY)
        {
            T lastObject = default;

            foreach (EntityAsset entityAsset in Assets.Reverse())
            {
                if ((!string.IsNullOrEmpty(category) && entityAsset.Class.Category != category) ||
                    entityAsset.Name != identifier)
                {
                    continue;
                }

                if (typeof(T) == typeof(EntityAsset))
                    lastObject = (T)(object)entityAsset;
                else if (entityAsset.Class.Entity is T comp)
                    lastObject = comp;
            }

            return lastObject;
        }

        /// <summary>
        /// Gets all entity assets stored in the registry with the specified identifier and category.
        /// </summary>
        /// <param name="identifier">The identifier of the asset.</param>
        /// <param name="category">The category of the asset.</param>
        /// <returns>All entity assets matching the specified identifier and category.</returns>
        public IEnumerable<object> GetAllFromRegistry(string identifier = DEFAULT_ASSET_NAME, string category = DEFAULT_CATEGORY)
        {
            foreach (EntityAsset entityAsset in Assets)
            {
                if ((!string.IsNullOrEmpty(category) && entityAsset.Class.Category != category) ||
                    entityAsset.Name != identifier)
                    continue;

                yield return entityAsset.Class.Entity;
            }
        }

        /// <summary>
        /// Gets the first entity asset stored in the registry with the specified identifier and category.
        /// </summary>
        /// <param name="identifier">The identifier of the asset.</param>
        /// <param name="category">The category of the asset.</param>
        /// <returns>The first entity asset matching the specified identifier and category.</returns>
        public object GetFirstFromRegistry(string identifier = DEFAULT_ASSET_NAME, string category = DEFAULT_CATEGORY)
        {
            foreach (EntityAsset entityAsset in Assets)
            {
                if ((!string.IsNullOrEmpty(category) && entityAsset.Class.Category != category) ||
                    entityAsset.Name != identifier)
                    continue;

                return entityAsset.Class.Entity;
            }

            return null;
        }

        /// <summary>
        /// Gets the last entity asset stored in the registry with the specified identifier and category.
        /// </summary>
        /// <param name="identifier">The identifier of the asset.</param>
        /// <param name="category">The category of the asset.</param>
        /// <returns>The last entity asset matching the specified identifier and category.</returns>
        public object GetLastFromRegistry(string identifier = DEFAULT_ASSET_NAME, string category = DEFAULT_CATEGORY)
        {
            object lastObject = null;
            foreach (EntityAsset entityAsset in Assets)
            {
                if ((!string.IsNullOrEmpty(category) && entityAsset.Class.Category != category) ||
                    entityAsset.Name != identifier)
                    continue;

                lastObject = entityAsset.Class.Entity;
            }

            return lastObject;
        }

        /// <summary>
        /// Removes the specified entity asset from the registry.
        /// </summary>
        /// <param name="entityAsset">The entity asset to remove.</param>
        /// <returns><see langword="true"/> if the entity asset was successfully removed; otherwise, <see langword="false"/>.</returns>
        public bool Remove(EntityAsset entityAsset)
        {
            bool result = assets.Remove(entityAsset);
            entityAsset.Destroy();
            return result;
        }

        /// <summary>
        /// Removes all entity assets with the specified identifier and category from the registry.
        /// </summary>
        /// <param name="identifier">The identifier of the assets to remove.</param>
        /// <param name="category">The category of the assets to remove.</param>
        public void RemoveAll(string identifier = DEFAULT_ASSET_NAME, string category = DEFAULT_CATEGORY)
        {
            foreach (EntityAsset entityAsset in GetAllFromRegistry<EntityAsset>(identifier, category))
                Remove(entityAsset);
        }

        /// <summary>
        /// Removes the first entity asset with the specified identifier and category from the registry.
        /// </summary>
        /// <param name="identifier">The identifier of the asset to remove.</param>
        /// <param name="category">The category of the asset to remove.</param>
        public void RemoveFirst(string identifier = DEFAULT_ASSET_NAME, string category = DEFAULT_CATEGORY) =>
            Remove(GetFirstFromRegistry<EntityAsset>(identifier, category));

        /// <summary>
        /// Removes the last entity asset with the specified identifier and category from the registry.
        /// </summary>
        /// <param name="identifier">The identifier of the asset to remove.</param>
        /// <param name="category">The category of the asset to remove.</param>
        public void RemoveLast(string identifier = DEFAULT_ASSET_NAME, string category = DEFAULT_CATEGORY) =>
            Remove(GetLastFromRegistry<EntityAsset>(identifier, category));

        /// <summary>
        /// Adds the specified entity asset to the registry.
        /// </summary>
        /// <param name="entityAsset">The entity asset to add.</param>
        /// <returns><see langword="true"/> if the entity asset was successfully added; otherwise, <see langword="false"/>.</returns>
        internal bool Add(EntityAsset entityAsset)
        {
            if (!entityAsset || assets.Contains(entityAsset))
                return false;

            assets.Add(entityAsset);
            return true;
        }
    }
}