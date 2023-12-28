// -----------------------------------------------------------------------
// <copyright file="ItemTrackerActor.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.ItemAbilities
{
    using System.Collections.Generic;

    using Exiled.API.Extensions;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Items;
    using Exiled.CustomModules.API.Features.CustomAbilities;
    using Exiled.Events.EventArgs.Player;

    /// <summary>
    /// The actor which handles all tracking-related tasks for items.
    /// </summary>
    public class ItemTrackerActor : StaticActor
    {
        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> containing all serials and their corresponding abilities.
        /// </summary>
        internal static Dictionary<uint, HashSet<IAbilityBehaviour>> TrackedSerials { get; } = new();

        /// <summary>
        /// Handles the event when a player is dropping an item.
        /// </summary>
        /// <param name="ev">The <see cref="DroppingItemEventArgs"/> containing information about the dropping item.</param>
        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnDroppingItem(DroppingItemEventArgs)"/>
        public void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (!ev.Item.HasComponent<IAbilityBehaviour>())
                return;

            AddOrTrack(ev.Item);
        }

        /// <summary>
        /// Adds or tracks the abilities of an item based on its serial.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> whose abilities are to be added or tracked.</param>
        public void AddOrTrack(Item item)
        {
            IEnumerable<IAbilityBehaviour> abilityBehaviours = item.GetComponents<IAbilityBehaviour>();

            if (TrackedSerials.ContainsKey(item.Serial))
            {
                TrackedSerials[item.Serial].AddRange(abilityBehaviours);
                return;
            }

            TrackedSerials.Add(item.Serial, new HashSet<IAbilityBehaviour>());
            TrackedSerials[item.Serial].AddRange(abilityBehaviours);

            return;
        }
    }
}