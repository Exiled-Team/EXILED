namespace Exiled.CustomItems.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.Events.EventArgs.Scp330;
    using InventorySystem.Items.Usables.Scp330;
    using MEC;

    /// <summary>
    /// THe custom candy base class.
    /// </summary>
    public abstract class CustomCandy : CustomItem
    {
        internal static new readonly Dictionary<ushort, List<int>> TrackedSerials = new();

        /// <inheritdoc/>
        /// <exception cref="NotSupportedException">Throws when trying to set value to property.</exception>
        public override ItemType Type
        {
            get => ItemType.SCP330;
            set => throw new NotSupportedException("Cannot set ItemType for CustomCandy. To set candy use CandyType.");
        }

        /// <summary>
        /// Gets or sets <see cref="CandyKindID"/> of custom candy.
        /// </summary>
        public abstract CandyKindID CandyType { get; set; }

        /// <summary>
        /// Applies an effects to player who eaten a <see cref="CustomCandy"/>.
        /// </summary>
        /// <param name="player">Player who eaten <see cref="CustomCandy"/>.</param>
        public abstract void ApplyEffect(Player player);

        /// <inheritdoc/>
        public override bool Check(Item? item) => item != null && TrackedSerials.ContainsKey(item!.Serial);

        /// <inheritdoc/>
        public override bool Check(Pickup? pickup) => pickup != null && TrackedSerials.ContainsKey(pickup!.Serial);

        /// <summary>
        /// Checks the specified inventory item to see if it is a custom item.
        /// </summary>
        /// <param name="scp330"></param>
        /// <param name="candyKindID"></param>
        /// <returns></returns>
        public bool Check(Scp330? scp330, CandyKindID candyKindID) => scp330 != null && TrackedSerials.TryGetValue(scp330.Serial, out List<int> candies) && candies.Contains(scp330.Candies.ToList().IndexOf(candyKindID));

        private void OnInternalUsingScp330(InteractingScp330EventArgs ev)
        {
            if (ev.Candy == CandyType)
            {
                Timing.CallDelayed(.5f, () =>
                {
                    if (!ev.Player.TryGetItem(ItemType.SCP330, out Item item) || !item.Is(out Scp330 scp330))
                        return;

                    if (TrackedSerials.ContainsKey(scp330.Serial))
                        TrackedSerials[scp330.Serial].Add(scp330.Candies.Count - 1);
                    else
                        TrackedSerials.Add(scp330.Serial, new List<int> { 0 });
                });
            }
        }

        private void OnInternalUsingCandy(EatingScp330EventArgs ev)
        {
            if (Check(ev.Scp330Bag, ev.Candy.Kind))
            {
                ev.IsAllowed = false;
                TrackedSerials[ev.Scp330Bag.Serial].Remove(ev.Scp330Bag.Candies.ToList().IndexOf(ev.Candy.Kind));
                ApplyEffect(ev.Player);
            }
        }
    }
}