// -----------------------------------------------------------------------
// <copyright file="CandyItemTracker.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomItems.Items.Candies
{
    using System.Collections.Generic;

    using Exiled.API.Features.Items;

    /// <summary>
    /// Defines a custom tracker for custom candies.
    /// </summary>
    public class CandyItemTracker : ItemTracker
    {
        /// <summary>
        /// Gets or sets a dictionary with tracked item's serials and list of indexes that are custom candies.
        /// </summary>
        public Dictionary<ushort, List<int>> TrackedIndexes { get; set; }

        /// <summary>
        /// Adds or tracks the trackables of an item based on its serial.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> whose trackables are to be added or tracked.</param>
        /// <param name="candyIndex">The index of candy that need to be marked as a custom.</param>
        /// <returns><see langword="true"/> if the item was added or tracked successfully; otherwise, <see langword="false"/>.</returns>
        public bool AddOrTrack(Item item, int candyIndex)
        {
            if (!item.Is(out Scp330 _) || !AddOrTrack(item))
                return false;

            if (!TrackedIndexes.ContainsKey(item.Serial))
                TrackedIndexes.Add(item.Serial, new List<int> { candyIndex });
            else
                TrackedIndexes[item.Serial].Add(candyIndex);

            return true;
        }

        /// <summary>
        /// Checks if an item is being tracked and if candy on index is custom..
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to check.</param>
        /// <param name="candyIndex">Index to check.</param>
        /// <returns><see langword="true"/> if the item is being tracked; otherwise, <see langword="false"/>.</returns>
        public bool IsTracked(Item item, int candyIndex)
        {
            if (!item.Is(out Scp330 _) || !IsTracked(item))
                return false;

            return TrackedIndexes.TryGetValue(item.Serial, out List<int> indexes) && indexes.Contains(candyIndex);
        }
    }
}