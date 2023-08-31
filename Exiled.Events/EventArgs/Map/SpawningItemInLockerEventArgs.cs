using Exiled.API.Features.Lockers;
using Exiled.API.Features.Pickups;
using Exiled.Events.EventArgs.Interfaces;
using InventorySystem.Items.Pickups;
using MapGeneration.Distributors;

namespace Exiled.Events.EventArgs.Map
{
    public class SpawningItemInLockerEventArgs : IPickupEvent, IDeniableEvent
    {
        public SpawningItemInLockerEventArgs(LockerChamber chamber, ItemPickupBase pickupBase, bool shouldInitiallySpawn, bool isAllowed = true)
        {
            Chamber = Chamber.Get(chamber);
            Pickup = Pickup.Get(pickupBase);
            ShouldInitiallySpawn = shouldInitiallySpawn;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="API.Features.Lockers.Chamber"/> where is spawning <see cref="Pickup"/>.
        /// </summary>
        public Chamber Chamber { get; }

        /// <summary>
        /// Gets or sets a <see cref="API.Features.Pickups.Pickup"/> which is spawning.
        /// </summary>
        public Pickup Pickup { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not item should be spawned now.
        /// </summary>
        public bool ShouldInitiallySpawn { get; set; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }
    }
}